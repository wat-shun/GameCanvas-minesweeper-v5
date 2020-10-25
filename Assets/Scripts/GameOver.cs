using GameCanvas;
using Unity.Mathematics;

public sealed class GameOverScene : GcScene
{
    private readonly ButtonActor _retryButton;

    public GameOverScene()
    {
        _retryButton = new ButtonActor(GcImage.Button_retry, GcAnchor.LowerCenter, new float2(0, -32));
    }

    public override void EnterScene(object state)
    {
        // 爆発音再生
        gc.PlaySE(GcSound.Se_explosion);

        // ゲームオーバー音再生
        gc.PlaySound(GcSound.Se_gameover);

        // ボタンの追加
        AddActor(_retryButton);
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
        gc.ClearScreen();

        // 背景
        gc.DrawImageWithAnchor(GcImage.Over_bomb, GcAnchor.MiddleCenter);
        // ロゴ
        gc.DrawImageWithAnchor(GcImage.Logo_gameover, GcAnchor.UpperCenter, new float2(0, 32));
        // キャラ
        gc.DrawImageWithAnchor(GcImage.Over_boy, GcAnchor.MiddleCenter);
    }

    public override void LeaveScene()
    {
        gc.StopSound();
        gc.RemoveActorAll();
    }
}
