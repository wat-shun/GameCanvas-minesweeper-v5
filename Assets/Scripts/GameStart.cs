using GameCanvas;

public sealed class GameStartScene : GcScene
{
    public override void EnterScene(object state)
    {
        // タイトルの効果音再生
        gc.PlaySE(GcSound.Se_title);
    }

    public override void UpdateScene()
    {
        // 3.5秒のOPが終わったらゲーム画面に遷移する
        if (gc.TimeSinceStartup > 3.5f) gc.ChangeScene<GamePlayScene>();
    }

    public override void DrawScene()
    {
        // 画面消去
        gc.ClearScreen();

        // 背景
        gc.DrawImage(GcImage.Airport, gc.CanvasWidth - 425, gc.CanvasHeight - 450);

        // キャラ
        gc.DrawImage(GcImage.Boy_7, 10, gc.CanvasHeight - 250);

        // タイトルロゴ
        gc.DrawImage(GcImage.Logo_minesweeper, 0, 0);
        gc.DrawImage(
            GcImage.Logo_sublitle,
            gc.GetImageWidth(GcImage.Logo_minesweeper) / 5.0f,
            gc.GetImageHeight(GcImage.Logo_minesweeper)
        );
    }
}