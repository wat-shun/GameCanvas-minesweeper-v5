using GameCanvas;

public sealed class Game : GameBase
{
    public override void InitGame()
    {
        // 初期設定
        gc.ChangeCanvasSize(600, 450);
        gc.SetSoundVolume(0.1f); // 効いてない気がする

        // シーンの登録
        gc.RegisterScene<GameStartScene>();
        gc.RegisterScene<GamePlayScene>();
        gc.RegisterScene<GameClearScene>();
        gc.RegisterScene<GameOverScene>();

        // ゲーム開始
        gc.ChangeScene<GameStartScene>();
        // gc.ChangeScene<GamePlayScene>();
        // gc.ChangeScene<GameClearScene>(new GameClearSceneState(1234567890));
        // gc.ChangeScene<GameOverScene>();
    }
}