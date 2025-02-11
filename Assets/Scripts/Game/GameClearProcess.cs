public class GameClearProcess : Process
{
    public void OnEnable()
    {
        UIManager.Instance.CloseLayoutUI<MainUI>();
        UIManager.Instance.ShowLayoutUI<GameClearUI>();

        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_AllStageClear);
        SoundSystem.Instance.PlayBGM("GameClear");
    }

    public void OnDisable()
    {
        UIManager.Instance.CloseLayoutUI<GameClearUI>();
    }
}