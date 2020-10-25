using GameCanvas;
using Unity.Mathematics;

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
    private readonly byte[] _numbers;

    public BombCountActor(int count)
    {
        _numbers = count.CalcDigits(); ;
    }

    public override void Draw()
    {
        // 爆弾の描画
        gc.DrawImage(GcImage.Bomb);

        // 数字の描画
        for (var i = 0; i < _numbers.Length; i++)
        {
            var idx = _numbers[_numbers.Length - i - 1];
            var img = NumberImage.Image[idx];
            var pos = new float2(NumberImage.Size.x * (i + 1), 0);
            gc.DrawImage(img, pos);
        }
    }
}

public sealed class GamePlayScene : GcScene
{
    // ゲーム設定
    private static readonly int2 BoardSize = new int2(10, 8);
    private const int BombCount = 15; // 爆弾の数
    private float _startTime;
    private MinesweeperBoard _minesweeperBoard;

    // 描画設定
    private readonly int2 _cellImageSize = new int2(36, 36);
    private readonly float2 _boardPadding = new float2(18, 18 + 36);
    private TimerActor _timerActor;

    // 入力関係
    private ButtonActor[][] _cellArray;
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
        _minesweeperBoard = new MinesweeperBoard(gc, BoardSize, BombCount);

        // セルボタンの初期化
        _cellArray = new ButtonActor[BoardSize.x][];
        for (var x = 0; x < BoardSize.x; x++)
        {
            _cellArray[x] = new ButtonActor[BoardSize.y];

            for (var y = 0; y < BoardSize.y; y++)
            {
                var pos = _boardPadding + _cellImageSize + new float2(_cellImageSize.x * x, _cellImageSize.y * y);
                var cell = new ButtonActor(GcImage.Cell_dummy, GcAnchor.UpperLeft, pos);

                _cellArray[x][y] = cell;
                gc.AddActor(cell);
            }
        }
    }

    public override void UpdateScene()
    {
        _timerActor.Second = (int)(gc.TimeSinceStartup - _startTime);

        // 以降は、リリースされた瞬間だけ処理する
        if (gc.IsTouched())
        {
            // 既に処理済みであればスキップする
            if (_isProcessedPointerEvent)
            {
                if (gc.IsTouchEnded())
                {
                    _isProcessedPointerEvent = false;
                }
                return;
            }

            for (var x = 0; x < BoardSize.x; x++)
            {
                for (var y = 0; y < BoardSize.y; y++)
                {
                    // 既に開いているマスは処理しない
                    if (_minesweeperBoard.IsOpened(x, y)) continue;

                    // 旗の設置処理
                    if (_cellArray[x][y].IsHolding())
                    {
                        gc.PlaySE(GcSound.Se_flagged);
                        _minesweeperBoard.TurnFlag(x, y);

                        // 設置と開封で多重に処理するのを防ぐため
                        // 次のタッチ終了イベントまで処理をスキップする
                        _isProcessedPointerEvent = true;
                        return;
                    }

                    // セルの開封処理
                    if (!_minesweeperBoard.IsFlag(x, y) && _cellArray[x][y].IsTapped())
                    {
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
                        return;
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
        for (var x = 0; x < BoardSize.x; x++)
        {
            for (var y = 0; y < BoardSize.y; y++)
            {
                var pos = _boardPadding + _cellImageSize + new float2(_cellImageSize.x * x, _cellImageSize.y * y);

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
