using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : Data
{
    [field: SerializeField] public Sprite Icon { get; private set; }

    public int MaxLevel
    {
        get
        {
            if (LevelDatas == null) return 0;
            return LevelDatas.Count;
        }
    }

    public SkillLevelData GetSkillLevelData(int level)
    {
        if (LevelDatas == null || LevelDatas.Count == 0)
        {
            Debug.LogError($"{Id}�� ���� �� ��ų �����Ͱ� ����ֽ��ϴ�.");
            return null;
        }

        int index = level - 1;
        SkillLevelData defaultData = level >= MaxLevel ? LevelDatas[^1] : LevelDatas[0];

        if (index >= MaxLevel || index < 0)
        {
            return defaultData;
        }

        return LevelDatas[index];
    }

    [field: SerializeField] private string _defaultPrefabPath = "Skill/Skill";

    [Header("Ŀ���� �� ��ų �������� ������ �־��ּ���")] [field: SerializeField]
    private Skill _prefab;

    public Skill Prefab
    {
        get
        {
            if (_prefab != null) return _prefab;

            var prefab = Resources.Load<Skill>(_defaultPrefabPath);

            return prefab;
        }
    }

    [field: SerializeField] public List<SkillLevelData> LevelDatas { get; private set; }
}

[Serializable]
public class SkillLevelData
{
    [field: SerializeField] public int ADRatio { get; private set; }
    [field: SerializeField] public int APRatio { get; private set; }
    [field: SerializeField] public int NeedMP { get; private set; }

    [field: TextArea]
    [field: SerializeField]
    public string Description { get; private set; }

    [field: SerializeField] public float Duration { get; private set; }

    [field: SerializeField] public float Cooltime { get; private set; }
    [field: SerializeField] public List<SkillConditionData> Conditions { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> ApplyFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> UseSkillFxDatas { get; private set; }
    [field: SerializeField] public List<SkillFxEventData> RemoveFxDatas { get; private set; }
}