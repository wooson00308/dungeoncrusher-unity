using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSloatView : BaseView
{
    public enum Images
    {
        Icon
    }

    public enum Texts
    {
        BuffCount
    }

    private void Awake()
    {
        BindUI();
    }

    public override void BindUI()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void UpdateUI(Sprite sprite, int count)
    {
        Get<Image>((int)Images.Icon).sprite = sprite;
        Get<TextMeshProUGUI>((int)Texts.BuffCount).text = $"{count}";
    }
}