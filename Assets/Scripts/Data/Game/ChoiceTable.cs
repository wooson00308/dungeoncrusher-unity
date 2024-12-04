using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    Skill
}

[Serializable]
public class ChoiceData
{
    public string name;
    public int tier;
    public string description;
    public Sprite iconSprite;
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

            return string.Empty;
        }
    }

    [Space]
    public ChoiceType choiceType;

    public ItemData itemData;
    public SkillData skillData;
}
