using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [SerializeField] protected string _id;
    [SerializeField] protected GameObject _prefab;
    [SerializeField] protected int _rarity;
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;

    [Space] [SerializeField] protected bool _isAreaAttack;
    [SerializeField] protected bool _isPassiveSkill;
    [SerializeField] protected bool _isSelfSkill; //자가 구동을 위한 bool 값 
    [SerializeField] protected UnitEvents _skillEventType;

    [Space] [SerializeField] protected List<SkillLevelData> _skillLevelDatas;

    public string Id => _id;

    public GameObject Prefab
    {
        get { return _prefab; }
        set { _prefab = value; }
    }

    public int Rarity => _rarity;
    public Sprite Icon => _icon;
    
    public string Name => _name;

    public string Description(int level)
    {
        var skillLevelDetail = GetSkillLevelData(level);

        // 원본 문자열
        string description = "공격시 {n}(%)확률로 용사 공격력의 {a}%로 추가 공격을 가한다.";

        // 변수값으로 문자열 대체
        description = _description
            .Replace("{n}", skillLevelDetail.activationChance.ToString("F0")) // {n}에 확률 값 대입 (소수점 한 자리)
            .Replace("{a}", skillLevelDetail.skillValue.ToString("F0")); // {a}에 공격 배율 값 대입 (정수로 표시)

        return description;
    }

    public bool IsAreaAttack => _isAreaAttack;
    public bool IsPassiveSkill => _isPassiveSkill;
    public bool IsSelfSkill => _isSelfSkill; //자가 구동을 위한 bool 값 입력부
    public UnitEvents SkillEventType => _skillEventType;

    public List<SkillLevelData> SkillLevelDatas
    {
        get { return _skillLevelDatas; }
        set { _skillLevelDatas = value; }
    }

    public SkillLevelData GetSkillLevelData(int skillLevel)
    {
        int skillIndex = skillLevel - 1;
        SkillLevelData defaultSkillLevelData = _skillLevelDatas[^1];

        if (defaultSkillLevelData == null)
        {
            string errorMessage = $"{_id}의 레벨 별 스킬 데이터가 존재하지 않습니다.";
            Debug.LogError(errorMessage);
            throw new SkillDataNotFoundException(errorMessage);
        }

        if (_skillLevelDatas.Count - 1 < skillIndex)
        {
            return defaultSkillLevelData;
        }

        return _skillLevelDatas[skillIndex];
    }

    public abstract bool IsValidTarget(Unit unit);
    public abstract void OnAction(int level, Unit user, List<Unit> targets);
}

[Serializable]
public class SkillLevelData
{
    [Range(0f, 100f)] public float activationChance;
    public float skillValue; // n%
    public float coolTIme; //쿨타임 입력
    public int targetNum; //타켓수 지정방식
    public GameObject skillFxPrefab;
}

public class SkillDataNotFoundException : Exception
{
    public SkillDataNotFoundException(string message) : base(message)
    {
    }
}