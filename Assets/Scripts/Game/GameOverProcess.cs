using System;
using UnityEngine;

public class GameOverProcess : Process
{
    public void OnEnable()
    {
        UIManager.Instance.CloseLayoutUI<MainUI>();
        UIManager.Instance.ShowLayoutUI<GameOverUI>();

        GameEventSystem.Instance.Publish(ProcessEvents.ProcessEvent_GameOver.ToString());
        SoundSystem.Instance.PlayBGM("GameOver");
    }

    public void OnDisable()
    {
        UIManager.Instance.CloseLayoutUI<GameOverUI>();
    }
}