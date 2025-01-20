using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ChoiceTable", menuName = "Data/Create ChoiceTable")]
public class ChoiceTable : ScriptableObject
{
    [SerializeField] private List<ChoiceData> _choiceDatas;

    public List<ChoiceData> ChoiceDatas => _choiceDatas;

    public List<ChoiceData> GetRandomChoices(int count = 3)
    {
        List<ChoiceData> result = new();

        if (_choiceDatas == null || _choiceDatas.Count == 0)
        {
            return result;
        }

        var weightedChoices = _choiceDatas
            .Where(data => data.weight > 0)
            .Select(data => (data, cumulativeWeight: data.weight))
            .ToList();

        float totalWeight = weightedChoices.Sum(w => w.cumulativeWeight);

        while (result.Count < count && weightedChoices.Count > 0)
        {
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float runningTotal = 0f;

            foreach (var (data, weight) in weightedChoices.ToList())
            {
                var player = UnitFactory.Instance.GetPlayer();

                if (data.choiceType == ChoiceType.Skill)
                {
                    if (player.SkillDic
                        .TryGetValue(data.skillData.Id, out Skill skill))
                    {
                        if (skill != null && skill.Level >= data.skillData.MaxLevel)
                        {
                            Debug.Log(skill.Data.Name);
                            Debug.Log(skill.Level);
                            weightedChoices.Remove((data, weight)); //만렙 스킬 제거
                            continue;
                        }
                    }
                }

                if (data.choiceType == ChoiceType.Item)
                {
                    if (player.Equipment.TryGetValue(data.itemData.PartType, out var item))
                    {
                        if (item != null && item.Data.Id == data.itemData.Id)
                        {
                            weightedChoices.Remove((data, weight)); //장착중인 아이템 제거
                            continue;
                        }
                    }
                }

                runningTotal += weight;

                if (randomValue <= runningTotal)
                {
                    result.Add(data);
                    totalWeight -= weight;
                    weightedChoices.Remove((data, weight));
                    break;
                }
            }
        }

        return result;
    }
}

public enum ChoiceType
{
    Item,
    Skill,
    Stat
}

[Serializable]
public class ChoiceData
{
    public int tier;

    public string GetPrefabPath
    {
        get
        {
            if (choiceType == ChoiceType.Item)
            {
                return "UI/Unit/Unit_Equip";
            }
            else if (choiceType == ChoiceType.Skill)
            {
                return "UI/Unit/Unit_Skill";
            }
            else
            {
                return "UI/Unit/Unit_Stat";
            }

            return string.Empty;
        }
    }

    [Space] public ChoiceType choiceType;

    public ItemData itemData;
    public SkillData skillData;
    public UnitStatsUpgradeData unitStatUpgradeData;

    public Sprite Icon()
    {
        if (choiceType == ChoiceType.Item)
            return itemData?.Icon;

        if (choiceType == ChoiceType.Skill)
            return skillData?.Icon;

        if (choiceType == ChoiceType.Stat)
            return unitStatUpgradeData?.UpgradeIcon;

        return null;
    }

    [Range(0, 100)] public float weight = 1f; // °¡ÁßÄ¡ ±âº»°ª 1
}