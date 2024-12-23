using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ChoiceTable", menuName = "Data/Create ChoiceTable")]
public class ChoiceTable : ScriptableObject
{
    [SerializeField] private List<ChoiceData> _choiceDatas;

    public List<ChoiceData> GetRandomChoices(int count = 3)
    {
        List<ChoiceData> result = new List<ChoiceData>();

        if (_choiceDatas.Count == 0)
        {
            return result;
        }

        while (result.Count < count)
        {
            int choiceIndex = UnityEngine.Random.Range(0, _choiceDatas.Count);
            result.Add(_choiceDatas[choiceIndex]);
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

    public Sprite Icon()
    {
        if (choiceType == ChoiceType.Item)
            return itemData.Icon;
        else if (choiceType == ChoiceType.Skill)
            return skillData.Icon;

        return null;
    }

    public ItemData itemData;
    public SkillData skillData;
    public UnitStatUpgradeData unitStatUpgradeData;
}

public enum UpgradeStatType
{
    Health,
    Attack,
    Defense,
    Speed,
    AttackSpeed,
    CriticalRate,
    CriticalPercent,
    AttackStunRate,
    LifeStealRate,
    LifeStealPercent,
}

[Serializable]
public class UnitStatUpgradeData : IStats
{
    public UpgradeStatType upgradeType;
    [field: SerializeField] public IntStat Health { get; set; }
    [field: SerializeField] public IntStat Attack { get; set; }
    [field: SerializeField] public IntStat Defense { get; set; }
    [field: SerializeField] public IntStat Mp { get; set; }
    [field: SerializeField] public IntStat Exp { get; set; }
    [field: SerializeField] public IntStat Level { get; set; }
    [field: SerializeField] public IntStat StageLevel { get; set; }
    [field: SerializeField] public FloatStat Speed { get; set; }
    [field: SerializeField] public FloatStat AttackSpeed { get; set; }
    [field: SerializeField] public FloatStat AttackRange { get; set; }
    [field: SerializeField] public FloatStat CriticalRate { get; set; }
    [field: SerializeField] public FloatStat CriticalPercent { get; set; }
    [field: SerializeField] public FloatStat AttackStunRate { get; set; }
    [field: SerializeField] public FloatStat LifestealRate { get; set; }
    [field: SerializeField] public FloatStat LifestealPercent { get; set; }
}