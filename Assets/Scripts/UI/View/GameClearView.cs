using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearView : BaseView
{
    enum Buttons
    {
        LobbyButton
    }

    enum GameObjects
    {
        GameClearPanel,
    }

    private void OnEnable()
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
            UIManager.Instance.CloseLayoutUI<GameClearUI>();
            SceneManager.LoadScene(0);
        });

        Get<GameObject>((int)GameObjects.GameClearPanel).gameObject.SetActive(true);
    }
}