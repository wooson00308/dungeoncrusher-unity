using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogImageView : BaseView
{
    [SerializeField] private Sprite defaultSprite;

    public enum Images
    {
        Icon_Log
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
            Get<Image>((int)Images.Icon_Log).sprite = icon;
        }
        else
        {
            Get<Image>((int)Images.Icon_Log).sprite = defaultSprite;
        }

        if (description != null)
        {
            Get<TextMeshProUGUI>((int)Texts.DescriptionText).SetText(description);
        }
    }
}