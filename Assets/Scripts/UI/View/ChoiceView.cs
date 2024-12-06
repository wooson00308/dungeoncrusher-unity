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

    private ChoiceData _data;

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
        _data = data;

        Get<TextMeshProUGUI>((int)Texts.Txt_Tier).SetText($"{data.tier}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Name).SetText($"{data.name}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Description).SetText($"{data.description}");
        Get<Image>((int)Images.Icon).sprite = data.Icon();
    }

    public void OnClick()
    {
        var players = UnitFactory.Instance.GetTeamUnits(Team.Friendly);
        foreach (var player in players)
        {
            if(player.Id == _data.UnitId)
            {
                if(_data.choiceType == ChoiceType.Item)
                {
                    var item = ResourceManager.Instance.SpawnFromPath($"Item/{_data.itemData.PrefId}").GetComponent<Item>();
                    player.EquipItem(item);
                }
                else
                {
                    player.AddSkill(_data.skillData);
                }
            }
        }

        GameEventSystem.Instance.Publish(ProcessEvents.Engage.ToString());
    }
}