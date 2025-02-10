using System.Collections.Generic;
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

    private Unit _owner;

    private void Awake()
    {
        _readyView = transform.parent.GetComponentInParent<ReadyView>();
        _owner = UnitFactory.Instance.GetPlayer(); 
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

        if (_data.itemData != null)
        {
            ItemUI(data);
        }
    }


    private List<TextMeshProUGUI> texts = new();
    private List<TextMeshProUGUI> afterTexts = new();

    private void ItemUI(ChoiceData data)
    {
        if (texts.Count == 0)
        {
            texts = Get<GameObject>((int)GameObjects.Group_Stats)?
                .GetComponentsInChildren<TextMeshProUGUI>().ToList();
        }

        if (afterTexts.Count == 0)
        {
            afterTexts = Get<GameObject>((int)GameObjects.Group_Stats_After)?
                .GetComponentsInChildren<TextMeshProUGUI>().ToList();
        }

        Dictionary<string, float> statValues = new();
        Dictionary<string, float> afterStatValues = new();

        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
        }

        foreach (var afterText in afterTexts)
        {
            afterText.gameObject.SetActive(false);
        }

        #region currentStatValue

        if (!IsNotChagedStat(data.itemData.Health.Value))
        {
            statValues.Add("Health", data.itemData.Health.Value);
        }

        if (!IsNotChagedStat(data.itemData.Defense.Value))
        {
            statValues.Add("Defense", data.itemData.Defense.Value);
        }

        if (!IsNotChagedStat(data.itemData.Attack.Value))
        {
            statValues.Add("Attack", data.itemData.Attack.Value);
        }

        if (!IsNotChagedStat(data.itemData.AttackSpeed.Value))
        {
            statValues.Add("AttackSpeed", data.itemData.AttackSpeed.Value);
        }

        if (!IsNotChagedStat(data.itemData.Speed.Value))
        {
            statValues.Add("Speed", data.itemData.Speed.Value);
        }

        #endregion

        if (texts != null) //현재 아이템 스탯 증가값 표기
        {
            int textCount = 0;
            foreach (var value in statValues)
            {
                texts[textCount].gameObject.SetActive(true);
                SetStatTextUI(texts[textCount], value.Key, value.Value);
                textCount++;
            }
        }

        if (afterTexts != null) //현재 아이템을 장착했을때 바뀌는 스탯 값 표기
        {
            var player = UnitFactory.Instance.GetPlayer();

            if (player == null)
            {
                int textCount = 0;
                foreach (var value in statValues)
                {
                    afterTexts[textCount].gameObject.SetActive(true);
                    SetStatTextUI(afterTexts[textCount], value.Key, value.Value);
                    textCount++;
                }
            }
            else // 현재 아이템을 장착했을 때 기존 아이템과 바뀌는 스탯 값 표기
            {
                if (player.Equipment.TryGetValue(data.itemData.PartType, out var playerCurrentItem))
                {
                    if (!IsNotChagedStat(data.itemData.Health.Value - playerCurrentItem.Data.Health.Value))
                    {
                        afterStatValues.Add("Health", data.itemData.Health.Value - playerCurrentItem.Data.Health.Value);
                    }

                    if (!IsNotChagedStat(data.itemData.Defense.Value - playerCurrentItem.Data.Defense.Value))
                    {
                        afterStatValues.Add("Defense",
                            data.itemData.Defense.Value - playerCurrentItem.Data.Defense.Value);
                    }

                    if (!IsNotChagedStat(data.itemData.Attack.Value - playerCurrentItem.Data.Attack.Value))
                    {
                        afterStatValues.Add("Attack", data.itemData.Attack.Value - playerCurrentItem.Data.Attack.Value);
                    }

                    if (!IsNotChagedStat(data.itemData.AttackSpeed.Value - playerCurrentItem.Data.AttackSpeed.Value))
                    {
                        afterStatValues.Add("AttackSpeed",
                            data.itemData.AttackSpeed.Value - playerCurrentItem.Data.AttackSpeed.Value);
                    }

                    if (!IsNotChagedStat(data.itemData.Speed.Value - playerCurrentItem.Data.Speed.Value))
                    {
                        afterStatValues.Add("Speed", data.itemData.Speed.Value - playerCurrentItem.Data.Speed.Value);
                    }

                    int textCount = 0;
                    foreach (var value in afterStatValues)
                    {
                        afterTexts[textCount].gameObject.SetActive(true);
                        SetStatTextUI(afterTexts[textCount], value.Key, value.Value);
                        textCount++;
                    }
                }
                else
                {
                    int textCount = 0;
                    foreach (var value in statValues)
                    {
                        afterTexts[textCount].gameObject.SetActive(true);
                        SetStatTextUI(afterTexts[textCount], value.Key, value.Value);
                        textCount++;
                    }
                }
            }
        }
    }

    private bool IsNotChagedStat(float stat)
    {
        return stat == 0;
    }

    private void SetStatTextUI(TextMeshProUGUI text, string name, float value)
    {
        text?.SetText($"{name} {ResultValueText(value)}");
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
            if (_owner.SkillDic.TryGetValue(data.skillData.Id, out Skill skill))
            {
                return data.skillData.GetSkillLevelData(skill.Level + 1).Description;
            }
            else
            {
                return data.skillData.GetSkillLevelData(1).Description;
            }
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

    private string ResultValueText(float value) //값이+라면 "+"값   값이-라면 "-"값 
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
        if (_data.choiceType == ChoiceType.Item)
        {
            var item = ResourceManager.Instance.Spawn(_data.itemData.Prefab).GetComponent<Item>();
            _owner.EquipItem(item);
        }
        else if (_data.choiceType == ChoiceType.Skill)
        {
            _owner.AddSkill(_data.skillData);
        }
        else
        {
            _owner.UpdateStats("Enagage", _data.unitStatUpgradeData, true);
            _readyView.DisCountStatChoiceCount();
        }

        if (_readyView.StatChoiceCount >= 0) return;
        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_Engage);
    }
}