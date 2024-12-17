using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverView : BaseView
{
    enum Buttons
    {
        LobbyButton
    }
    enum GameObjects
    {
        GameOverPanel,
    }

    private void Awake()
    {
        BindUI();
    }

    public override void BindUI()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        UpdateUI();
    }

    private void UpdateUI()
    {
        Get<Button>((int)Buttons.LobbyButton).onClick.AddListener(() =>
        {
            UIManager.Instance.CloseLayoutUI<GameOverUI>();
            SceneManager.LoadScene(0);
        });

        Get<GameObject>((int)GameObjects.GameOverPanel).gameObject.SetActive(true);
    }
}