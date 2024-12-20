using UnityEngine;

public class GameClearProcess : Process
{
    public void OnEnable()
    {
        UIManager.Instance.CloseLayoutUI<MainUI>();
        UIManager.Instance.ShowLayoutUI<GameClearUI>();

        GameEventSystem.Instance.Publish(ProcessEvents.ProcessEvent_AllStageClear.ToString());
        SoundSystem.Instance.PlayBGM("GameClear");
    }

    public void OnDisable()
    {
        UIManager.Instance.CloseLayoutUI<GameClearUI>();
    }
}