using UnityEngine;

public class LobbyView : BaseView
{
    public enum GameObjects
    {
        PressEnterText
    }

    private void OnEnable()
    {
        BindUI();
    }

    public override void BindUI()
    {
        Bind<GameObject>(typeof(GameObjects));
        UpdateUI();
    }

    public void UpdateUI()
    {
        Get<GameObject>((int)GameObjects.PressEnterText).SetActive(true);
    }
}