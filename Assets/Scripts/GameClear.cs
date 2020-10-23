using GameCanvas;
using Unity.Mathematics;
using UnityEngine;

// TODO 描画順の管理がうまくいってないのを直したい
// 理想:背景→飛行機→男の子→ロゴ・ボタン・スコア

/// <summary>
/// ゲームクリア画面を呼び出すときに渡す変数を管理する構造体です。
/// </summary>
public readonly struct GameClearSceneState
{
    public readonly int ClearTime;

    public GameClearSceneState(int clearTime)
    {
        ClearTime = clearTime;
    }
}

public sealed class PlaneActor : GcActor
{
    private readonly GcImage _image;

    private float2 _position;
    private float _speed;
    private float _degree;

    public PlaneActor(GcImage image)
    {
        _image = image;
        Priority = 0;

        InitStatus();
    }

    private void InitStatus()
    {
        _position = new float2(gc.Random(0, gc.CanvasWidth * 2), gc.CanvasHeight);
        _speed = gc.Random(13, 18);
        _degree = gc.Random(25, 35);
    }

    public override void Update()
    {
        // 画像の右下が画面外まで出てたら初期化
        if (_position.x + gc.GetImageWidth(_image) < 0 && _position.y + gc.GetImageHeight(_image) < 0)
        {
            InitStatus();
        }
        else
        {
            // 移動
            _position -= new float2(_speed * gc.Cos(_degree), _speed * gc.Sin(_degree));
        }
    }

    public override void Draw()
    {
        gc.DrawImage(_image, _position, _degree);
    }
}

public sealed class GameClearScene : GcScene
{
    private GameClearSceneState _state;

    private ButtonActor _retryButton;
    private TimerActor _timerActor;

    public override void EnterScene(object state)
    {
        _state = (GameClearSceneState) state;

        // クリア音再生
        gc.PlaySE(GcSound.Se_clear);

        // リトライボタンの追加
        _retryButton = new ButtonActor(GcImage.Button_retry, GcAnchor.LowerCenter, new float2(0, -32));
        gc.AddActor(_retryButton);

        // タイマーの追加
        // TODO 座標計算ちょっと汚いのとTimerActor内の実装と重複してて気持ち悪い
        var k = (_state.ClearTime == 1) ? 1 : ((int) Mathf.Log10(_state.ClearTime) + 2); // 秒を含む桁数
        var size = gc.GetImageSize(GcImage.Number_0);
        var x = gc.CanvasWidth / 2 + size.x * (k - 1) / 2; // 右端の座標
        var y = gc.CanvasHeight - size.y; // 下端の座標
        _timerActor = new TimerActor(_state.ClearTime, new int2(x, y));
        gc.AddActor(_timerActor);

        // 飛行機の追加
        gc.AddActor(new PlaneActor(GcImage.Plane_1));
        gc.AddActor(new PlaneActor(GcImage.Plane_2));
        gc.AddActor(new PlaneActor(GcImage.Plane_3));
        gc.AddActor(new PlaneActor(GcImage.Plane_4));
        gc.AddActor(new PlaneActor(GcImage.Plane_5));
        gc.AddActor(new PlaneActor(GcImage.Plane_6));
        gc.AddActor(new PlaneActor(GcImage.Plane_7));
        gc.AddActor(new PlaneActor(GcImage.Plane_8));
    }

    public override void UpdateScene()
    {
        // リトライボタンが離上されたらゲームシーンに飛ばす
        if (_retryButton.IsReleased())
        {
            gc.ChangeScene<GamePlayScene>();
        }
    }

    public override void DrawScene()
    {
        // 画面消去
        gc.ClearScreen();

        // 背景
        gc.DrawImageWithAnchor(GcImage.Clear_sky, GcAnchor.MiddleCenter);
        // ロゴ
        gc.DrawImageWithAnchor(GcImage.Logo_clear, GcAnchor.UpperCenter, new float2(0, 32));
        // キャラ Actor化してもいいかも
        var deg = 360 * (gc.TimeSinceStartup % 1);
        var cy = (int) (32 * gc.Sin(deg));
        // Debug.Log(gc.Sin(deg));
        gc.DrawImageWithAnchor(GcImage.Clear_boy, GcAnchor.MiddleCenter, new float2(0, cy));
    }

    public override void LeaveScene()
    {
        gc.StopSound();
        gc.RemoveActorAll();
    }
}