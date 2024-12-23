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

        if (_choiceDatas == null || _choiceDatas.Count == 0)
        {
            return result;
        }

        // 확률 기반 선택을 위해 누적 가중치 계산
        var weightedChoices = _choiceDatas
            .Where(data => data.weight > 0) // 가중치가 0보다 큰 데이터만 사용
            .Select(data => (data, cumulativeWeight: data.weight))
            .ToList();

        float totalWeight = weightedChoices.Sum(w => w.cumulativeWeight);

        while (result.Count < count && weightedChoices.Count > 0)
        {
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float runningTotal = 0f;

            foreach (var (data, weight) in weightedChoices)
            {
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
    Skill
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

            return string.Empty;
        }
    }

    [Space]
    public ChoiceType choiceType;

    public Sprite Icon()
    {
        if (choiceType == ChoiceType.Item)
            return itemData?.Icon;
        else if (choiceType == ChoiceType.Skill)
            return skillData?.Icon;

        return null;
    }

    public ItemData itemData;
    public SkillData skillData;

    [Range(0, 100)]
    public float weight = 1f; // 가중치 기본값 1
}
