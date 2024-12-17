using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogImageView : BaseView
{
    public enum Images
    {
        LogIcon
    }

    public enum Texts
    {
        DescriptionText
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

    public void SetLog(Sprite icon, string description)
    {
        if (icon != null)
        {
            Get<Image>((int)Images.LogIcon).sprite = icon;
        }

        if (description != null)
        {
            Get<TextMeshProUGUI>((int)Texts.DescriptionText).SetText(description);
        }
    }
}