using System.Linq;
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

    public enum GameObjects
    {
        Group_Stats,
        Group_Stats_After
    }

    private ChoiceData _data;

    private ReadyView _readyView;

    private void Awake()
    {
        _readyView = transform.parent.GetComponentInParent<ReadyView>();
        BindUI();
    }

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
    }

    public void SetupUI(ChoiceData data)
    {
        _data = data;

        Get<TextMeshProUGUI>((int)Texts.Txt_Tier).SetText($"{data.tier}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Name).SetText($"{GetNameData(data)}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Description).SetText($"{GetDescriptionData(data)}");
        Get<Image>((int)Images.Icon).sprite = data.Icon();

        ItemUI(data);
    }

    private void ItemUI(ChoiceData data)
    {
        TextMeshProUGUI[] texts = Get<GameObject>((int)GameObjects.Group_Stats)?
            .GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] afterTexts = Get<GameObject>((int)GameObjects.Group_Stats_After)?
            .GetComponentsInChildren<TextMeshProUGUI>();

        if (texts != null)
        {
            texts[0].text = $"Health +{data.itemData.Health.Value}";
            texts[1].text = $"Defense +{data.itemData.Defense.Value}";
            texts[2].text = $"Attack +{data.itemData.Attack.Value}";
        }

        if (afterTexts != null)
        {
            var teamUnits = UnitFactory.Instance.GetTeamUnits(Team.Friendly);

            if (teamUnits == null)
            {
                afterTexts[0].text = $"Health +{data.itemData.Health.Value}";
                afterTexts[1].text = $"Defense +{data.itemData.Defense.Value}";
                afterTexts[2].text = $"Attack +{data.itemData.Attack.Value}";
            }
            else
            {
                var player = teamUnits.FirstOrDefault();

                if (player.Equipment.TryGetValue(data.itemData.PartType, out var playerCurrentItem))
                {
                    playerCurrentItem = player.Equipment[data.itemData.PartType];

                    afterTexts[0].text =
                        $"Health {ResultText(data.itemData.Health.Value - playerCurrentItem.Data.Health.Value)}";
                    afterTexts[1].text =
                        $"Defense {ResultText(data.itemData.Defense.Value - playerCurrentItem.Data.Defense.Value)}";
                    afterTexts[2].text =
                        $"Attack {ResultText(data.itemData.Attack.Value - playerCurrentItem.Data.Attack.Value)}";
                }
                else
                {
                    afterTexts[0].text = $"Health +{data.itemData.Health.Value}";
                    afterTexts[1].text = $"Defense +{data.itemData.Defense.Value}";
                    afterTexts[2].text = $"Attack +{data.itemData.Attack.Value}";
                }
            }
        }
    }

    private string GetNameData(ChoiceData data)
    {
        if (data.skillData != null)
        {
            return data.skillData.Name;
        }
        else if (data.itemData != null)
        {
            return data.itemData.Name;
        }
        else if (data.unitStatUpgradeData != null)
        {
            return data.unitStatUpgradeData.UpgradeName;
        }
        else
        {
            Debug.Log("모든 선택 데이터가 없습니다.");
            return string.Empty;
        }
    }

    private string GetDescriptionData(ChoiceData data)
    {
        if (data.skillData != null)
        {
            return data.skillData.Description;
        }
        else if (data.itemData != null)
        {
            return data.itemData.Description;
        }
        else if (data.unitStatUpgradeData != null)
        {
            return data.unitStatUpgradeData.Description;
        }
        else
        {
            Debug.Log("아이템 데이터와 스킬데이터가 둘다 없습니다.");
            return string.Empty;
        }
    }

    private string ResultText(float value)
    {
        string result;
        if (value > 0)
        {
            result = $"+{value}";
        }
        else if (value == 0)
        {
            result = $"{value}"; //현재와 동일하다는 텍스트를 넣어도됨
        }
        else
        {
            result = $"{value}";
        }

        return result;
    }

    public void OnClick()
    {
        var players = UnitFactory.Instance.GetTeamUnits(Team.Friendly);
        Debug.Log("OnClick");
        foreach (var player in players)
        {
            if (_data.choiceType == ChoiceType.Item)
            {
                var item = ResourceManager.Instance.Spawn(_data.itemData.Prefab).GetComponent<Item>();
                player.EquipItem(item);
            }
            else if (_data.choiceType == ChoiceType.Skill)
            {
                player.AddSkill(_data.skillData);
            }
            else
            {
                player.UpdateStats("Enagage", _data.unitStatUpgradeData);
                _readyView.DisCountStatChoiceCount();
            }
        }

        if (_readyView.StatChoiceCount >= 0) return;
        GameEventSystem.Instance.Publish(ProcessEvents.ProcessEvent_Engage.ToString());
    }
}