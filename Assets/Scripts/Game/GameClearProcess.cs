using UnityEngine;

public class GameClearProcess : Process
{
    public void OnEnable()
    {
        UIManager.Instance.CloseLayoutUI<MainUI>();
        UIManager.Instance.ShowLayoutUI<GameClearUI>();

        GameEventSystem.Instance.Publish(ProcessEvents.AllStageClear.ToString());
    }

    public void OnDisable()
    {
        UIManager.Instance.CloseLayoutUI<GameClearUI>();
    }
}