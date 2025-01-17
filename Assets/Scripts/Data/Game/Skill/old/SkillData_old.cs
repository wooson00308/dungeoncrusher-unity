using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillFXSpawnPosType
{
    Self,
    Target
}

public abstract class SkillData_old : ScriptableObject
{
    private int _level = 0; //스킬데이터 자체에서 레벨을 알 수 있도록 함.
    [SerializeField] protected string _id;
    [SerializeField] protected GameObject _prefab;
    [SerializeField] protected int _rarity;
    [SerializeField] protected string _name;
    [Space] [SerializeField] protected bool _isAreaAttack;
    [SerializeField] protected bool _isCooltimeSkill;
    [SerializeField] protected bool _isRandomDetected;
    [SerializeField] protected bool _isPassiveSkill; //자가 구동을 위한 bool 값 
    [SerializeField] protected bool _isUltSkill; //필살기 체크 값 
    [SerializeField] protected UnitEvents _skillEventType;
    [SerializeField] protected SkillFXSpawnPosType _skillFxSpawnPosType;

    [Space] [SerializeField] protected List<SkillLevelData_old> _skillLevelDatas;

    public string Id => _id;

    public GameObject Prefab
    {
        get { return _prefab; }
        set { _prefab = value; }
    }

    public int Rarity => _rarity;

    public Sprite Icon
    {
        get
        {
            if (_level < _skillLevelDatas.Count)
                return _skillLevelDatas[_level].icon;
            else
                return _skillLevelDatas[_level - 1].icon;
        }
    } //=> _icon;

    public string Name => _name;

    public int Level
    {
        get => _level;
        set
        {
            if (MaxLv > _level)
            {
                _level = value;
            }
            else
            {
                _level = MaxLv;
            }
        }
    }

    public int MaxLv
    {
        get => _skillLevelDatas.Count;
    }

    public bool IsCooltimeSkill => _isCooltimeSkill;
    public bool IsAreaAttack => _isAreaAttack;
    public bool IsPassiveSkill => _isPassiveSkill; //자가 구동을 위한 bool 값 입력부
    public bool IsRamdomDetected => _isRandomDetected;
    public bool IsUltSkill => _isUltSkill; //자가 구동을 위한 bool 값 입력부

    public string Description
    {
        get
        {
            if (_level < _skillLevelDatas.Count)
                return _skillLevelDatas[_level].description;
            else
                return _skillLevelDatas[_level - 1].description;
        }
    }

    public UnitEvents SkillEventType => _skillEventType;
    public SkillFXSpawnPosType SkillFXSpawnPosType => _skillFxSpawnPosType;

    public List<SkillLevelData_old> SkillLevelDatas
    {
        get { return _skillLevelDatas; }
        set { _skillLevelDatas = value; }
    }

    public SkillLevelData_old GetSkillLevelData(int skillLevel)
    {
        int skillIndex = skillLevel - 1;
        SkillLevelData_old defaultSkillLevelData = _skillLevelDatas[^1];

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

        _level = skillLevel;
        return _skillLevelDatas[skillIndex];
    }

    public abstract bool IsValidTarget(Unit unit);
    public abstract void OnAction(Skill_old skill, Unit user, List<Unit> targets);
}

[Serializable]
public class SkillLevelData_old
{
    [Range(0f, 100f)] public float activationChance;
    public float skillValue; // n%
    public float coolTime; //쿨타임 입력
    public int targetNum; //타켓수 지정방식
    public float range; //공격 범위 크기?
    public Sprite icon;
    [TextArea] public string description;
    public GameObject skillFxPrefab;
}

public class SkillDataNotFoundException : Exception
{
    public SkillDataNotFoundException(string message) : base(message)
    {
    }
}