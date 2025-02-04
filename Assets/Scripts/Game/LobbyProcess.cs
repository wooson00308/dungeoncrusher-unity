using UnityEngine;

public class LobbyProcess : Process
{
    private void OnEnable()
    {
        UIManager.Instance.ShowLayoutUI<LobbyUI>();
        SoundSystem.Instance.PlayBGM("MainBGM");
        GameTime.TimeScale = 1;
        Time.timeScale = 1;
        _processSystem.IsSpawnPlayer = false;
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