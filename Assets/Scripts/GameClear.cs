using GameCanvas;
using Unity.Mathematics;
using UnityEngine;

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

public sealed class BackgroundActor : GcActor
{
    public BackgroundActor()
    {
        Priority = 0;
    }

    public override void Draw()
    {
        gc.DrawImageWithAnchor(GcImage.Clear_sky, GcAnchor.MiddleCenter);
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
        Priority = 1;

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

public sealed class ClearBoyActor : GcActor
{
    public ClearBoyActor()
    {
        Priority = 2;
    }

    public override void Draw()
    {
        var deg = 360 * (gc.TimeSinceStartup % 1);
        var cy = (int)(32 * gc.Sin(deg));
        gc.DrawImageWithAnchor(GcImage.Clear_boy, GcAnchor.MiddleCenter, new float2(0, cy));
    }
}

public sealed class ClearLogoActor : GcActor
{
    public ClearLogoActor()
    {
        Priority = 3;
    }

    public override void Draw()
    {
        gc.DrawImageWithAnchor(GcImage.Logo_clear, GcAnchor.UpperCenter, new float2(0, 32));
    }
}

public sealed class ClearTimeActor : TimerActor
{
    public ClearTimeActor(in int clearTime) : base(clearTime, CalcRightUpperPos(clearTime))
    {
        Priority = 5;
    }

    private static int2 CalcRightUpperPos(in int clearTime)
    {
        var k = (clearTime == 1) ? 1 : ((int)Mathf.Log10(clearTime) + 2); // 秒を含む桁数
        var size = gc.GetImageSize(GcImage.Number_0);
        var x = gc.CanvasWidth / 2 + size.x * (k - 1) / 2; // 右端の座標
        var y = gc.CanvasHeight - size.y; // 下端の座標
        return new int2(x, y);
    }
}

public sealed class GameClearScene : GcScene
{
    private GameClearSceneState _state;
    private readonly ButtonActor _retryButton;
    private readonly GcActor[] _actors;

    public GameClearScene()
    {
        _retryButton = new ButtonActor(GcImage.Button_retry, GcAnchor.LowerCenter, new float2(0, -32));
        _retryButton.Priority = 4;

        _actors = new GcActor[]
        {
            new BackgroundActor(),
            new ClearBoyActor(),
            new PlaneActor(GcImage.Plane_1),
            new PlaneActor(GcImage.Plane_2),
            new PlaneActor(GcImage.Plane_3),
            new PlaneActor(GcImage.Plane_4),
            new PlaneActor(GcImage.Plane_5),
            new PlaneActor(GcImage.Plane_6),
            new PlaneActor(GcImage.Plane_7),
            new PlaneActor(GcImage.Plane_8),
            _retryButton
        };
    }

    public override void EnterScene(object state)
    {
        _state = (GameClearSceneState)state;

        // クリア音再生
        gc.PlaySE(GcSound.Se_clear);

        // タイマーの追加
        gc.AddActor(new ClearTimeActor(_state.ClearTime));

        // その他のアクターの追加
        foreach (var actor in _actors)
        {
            gc.AddActor(actor);
        }
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
    }

    public override void LeaveScene()
    {
        gc.StopSound();
        gc.RemoveActorAll();
    }
}
