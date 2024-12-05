using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceView : BaseView
{
    public enum Texts
    {
        Txt_Tier,
        Txt_Name,
        Txt_Description
    }

    public enum Images
    {
        Icon
    }

    private void OnEnable()
    {
        BindUI();
    }

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }

    public void SetupUI(ChoiceData data)
    {
        Get<TextMeshProUGUI>((int)Texts.Txt_Tier).SetText($"{data.tier}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Name).SetText($"{data.name}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Description).SetText($"{data.description}");
        Get<Image>((int)Images.Icon).sprite = data.Icon();
    }

    public void OnClick()
    {
        // TODO : ĳ���Ϳ� ������ ������ �� ��ų ���� ����

        GameEventSystem.Instance.Publish(ProcessEvents.Engage.ToString());
    }
}