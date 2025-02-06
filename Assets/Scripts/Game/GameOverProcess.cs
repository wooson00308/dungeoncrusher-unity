using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverProcess : Process
{
    public void OnEnable()
    {
        UIManager.Instance.CloseLayoutUI<MainUI>();
        UIManager.Instance.ShowLayoutUI<GameOverUI>();

        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_GameOver);
        SoundSystem.Instance.PlayBGM("GameOver");
    }

    public void OnDisable()
    {
        UIManager.Instance.CloseLayoutUI<GameOverUI>();
    }
    
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            UIManager.Instance.CloseLayoutUI<GameOverUI>();
            SceneManager.LoadScene(0);
        }
    }
}