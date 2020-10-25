using System;
using System.Linq;
using GameCanvas;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public sealed class BoyActor : GcActor
{
    private static readonly GcImage[] Image =
    {
        GcImage.Boy_1,
        GcImage.Boy_2,
        GcImage.Boy_3,
        GcImage.Boy_4,
        GcImage.Boy_5,
        GcImage.Boy_6,
    };

    private int _faceID = 0;
    private float _lastFaceUpdateTime;

    public BoyActor()
    {
        _lastFaceUpdateTime = gc.TimeSinceStartup;
    }

    public override void Update()
    {
        // 10秒ごとに画像更新
        if (gc.TimeSinceStartup - _lastFaceUpdateTime < 10f) return;

        // 違う画像からランダムで選ぶ
        var nextID = _faceID;
        while (nextID == _faceID)
        {
            nextID = gc.Random(0, 5);
        }

        // 更新処理
        _faceID = nextID;
        _lastFaceUpdateTime = gc.TimeSinceStartup;
    }

    public override void Draw()
    {
        gc.DrawImageWithAnchor(Image[_faceID], GcAnchor.LowerRight);
    }
}

public sealed class BombCountActor : GcActor
{
    private readonly int _count;
    private readonly int2 _padding = new int2(0);

    public BombCountActor(int count)
    {
        _count = count;
    }

    public override void Draw()
    {
        // 爆弾の描画
        gc.DrawImage(GcImage.Bomb, _padding);

        // 数字の描画
        var countStr = _count.ToString();
        for (var i = 0; i < countStr.Length; i++)
        {
            var numStr = countStr[i];
            var num = int.Parse(numStr.ToString());
            gc.DrawImage(NumberImage.Image[num], _padding + new int2(NumberImage.Size.x * (i + 1), 0));
        }
    }
}

public sealed class GamePlayScene : GcScene
{
    // ゲーム設定
    private float _startTime;
    private static readonly int2 BoardSize = new int2(10, 8);
    private const int BombCount = 15; // 爆弾の数
    private MinesweeperBoard _minesweeperBoard;

    // 描画設定
    private readonly int2 _cellImageSize = new int2(36, 36);
    private readonly float2 _boardPadding = new float2(18, 18 + 36);
    private TimerActor _timerActor;

    // 入力関係
    private ButtonActor[,] _cellButton;
    private bool _isProcessedPointerEvent = false;

    public override void EnterScene(object state)
    {
        // BGM再生
        gc.PlaySound(GcSound.Bgm_game, loop: true);

        // 時間初期化
        _startTime = gc.TimeSinceStartup;

        // 男の子の追加
        gc.CreateActor<BoyActor>();

        // タイマーの追加
        _timerActor = new TimerActor(0, new int2(gc.CanvasWidth, 0));
        gc.AddActor(_timerActor);

        // 爆弾個数表示の追加
        gc.AddActor(new BombCountActor(BombCount));

        // ゲームの状態の初期化
        _minesweeperBoard = new MinesweeperBoard(BoardSize, BombCount);

        // セルボタンの初期化
        _cellButton = new ButtonActor[BoardSize.x, BoardSize.y];
        for (var i = 1; i < BoardSize.x + 1; i++)
        {
            for (var j = 1; j < BoardSize.y + 1; j++)
            {
                var x = i - 1;
                var y = j - 1;
                var pos = _boardPadding + new float2(_cellImageSize.x * i, _cellImageSize.y * j);
                var button = new ButtonActor(GcImage.Cell_dummy, GcAnchor.UpperLeft, pos);

                _cellButton[x, y] = button;
                gc.AddActor(button);
            }
        }
    }

    public override void UpdateScene()
    {
        _timerActor.Second = (int) (gc.TimeSinceStartup - _startTime);

        for (var x = 0; x < BoardSize.x; x++)
        {
            for (var y = 0; y < BoardSize.y; y++)
            {
                gc.TryGetPointerEvent(0, out var e);

                // ホールド処理とタッチ処理が2回処理されるのを防ぐため、PointerEventを見て離上されたかをチェックする
                // TODO ボタン処理の中に移動した方が良いのでは
                if (e.Frame == 0 && e.Phase == GcPointerEventPhase.Begin) // イベントがない時の既定値
                {
                    _isProcessedPointerEvent = false;
                }
                else if (!_isProcessedPointerEvent && !_minesweeperBoard.IsOpened(x, y))
                {
                    if (_cellButton[x, y].IsHolding(0.5f)) // 旗の設置処理
                    {
                        _isProcessedPointerEvent = true;

                        gc.PlaySE(GcSound.Se_flagged);
                        _minesweeperBoard.TurnFlag(x, y);
                    }
                    else if (!_minesweeperBoard.IsFlag(x, y) && _cellButton[x, y].IsReleased()) // セルの開封処理
                    {
                        _isProcessedPointerEvent = true;

                        gc.PlaySE(GcSound.Se_open);
                        _minesweeperBoard.Open(x, y);

                        // 終了判定
                        if (_minesweeperBoard.IsGameover())
                        {
                            gc.ChangeScene<GameOverScene>();
                        }
                        else if (_minesweeperBoard.IsClear())
                        {
                            gc.ChangeScene<GameClearScene>(new GameClearSceneState(_timerActor.Second));
                        }
                    }
                }
            }
        }
    }

    public override void DrawScene()
    {
        gc.ClearScreen();

        // 背景
        gc.DrawImageWithAnchor(GcImage.Airport, GcAnchor.LowerRight);

        // 下地の描画
        for (var i = 0; i < BoardSize.x + 2; i++)
        {
            for (var j = 0; j < BoardSize.y + 2; j++)
            {
                gc.DrawImage(
                    GcImage.Cell_blank,
                    _boardPadding + new float2(_cellImageSize.x * i, _cellImageSize.y * j)
                );
            }
        }

        // ボードに合わせて描画する
        // 基本的に隠れるけどデバッグモードとかで見れるような感じにしたいので全部書いておく
        for (var i = 1; i < BoardSize.x + 1; i++)
        {
            for (var j = 1; j < BoardSize.y + 1; j++)
            {
                var x = i - 1;
                var y = j - 1;
                var pos = _boardPadding + new float2(_cellImageSize.x * i, _cellImageSize.y * j);

                // 爆弾か数字
                if (_minesweeperBoard.IsBomb(x, y))
                {
                    gc.DrawImage(GcImage.Bomb, pos);
                }
                else if (_minesweeperBoard.CntAroundBomb(x, y) > 0)
                {
                    gc.DrawImage(NumberImage.Image[_minesweeperBoard.CntAroundBomb(x, y)], pos);
                }

                // 未開封
                if (!_minesweeperBoard.IsOpened(x, y))
                {
                    gc.DrawImage(GcImage.Cell_closed, pos);
                    // gc.DrawImage(GcImage.Cell_closed, pos.x, pos.y, 6, 6); // チートモード用
                }

                // 旗
                if (_minesweeperBoard.IsFlag(x, y))
                {
                    gc.DrawImage(GcImage.Flag, pos);
                }
            }
        }
    }

    public override void LeaveScene()
    {
        gc.StopSound();
        gc.RemoveActorAll();
    }
}
