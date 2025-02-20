using UnityEngine;

public class LobbyProcess : Process
{
    private void OnEnable()
    {
        UIManager.Instance.ShowLayoutUI<LobbyUI>();
        SoundSystem.Instance.PlayBGM("MainBGM");
        ResetTimeScale();
        _processSystem.IsSpawnPlayer = false;
    }

    private static void ResetTimeScale()
    {
        GameTime.TimeScale = 1;
        Time.timeScale = 1;
    }

    private void OnDisable()
    {
        UIManager.Instance.CloseLayoutUI<LobbyUI>();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            UIManager.Instance.ShowLayoutUI<MainUI>();
            _processSystem.OnNextProcess<ReadyProcess>();
        }
    }
}