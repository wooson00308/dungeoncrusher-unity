public class GameOverProcess : Process
{
    public void OnEnable()
    {
        UIManager.Instance.CloseLayoutUI<MainUI>();
        UIManager.Instance.ShowLayoutUI<GameOverUI>();

        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_GameOver);
        SoundSystem.Instance.PlayBGM("GameOver");
        _processSystem.IsSpawnPlayer = true;
    }

    public void OnDisable()
    {
        UIManager.Instance.CloseLayoutUI<GameOverUI>();
    }
}