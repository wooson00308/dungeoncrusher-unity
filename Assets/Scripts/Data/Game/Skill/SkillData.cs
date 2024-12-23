using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillFXSpawnPosType 
{ 
    Self,
    Target
}

public abstract class SkillData : ScriptableObject
{
    [SerializeField] protected string _id;
    [SerializeField] protected GameObject _prefab;
    [SerializeField] protected int _rarity;
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;

    [Space] [SerializeField] protected bool _isAreaAttack;
    [SerializeField] protected bool _isCooltimeSkill;
    [SerializeField] protected bool _isPassiveSkill; //자가 구동을 위한 bool 값 
    [SerializeField] protected bool _isUltSkill; //필살기 체크 값 
    [SerializeField] protected UnitEvents _skillEventType;
    [SerializeField] protected SkillFXSpawnPosType _skillFxSpawnPosType;

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

    public bool IsCooltimeSkill => _isCooltimeSkill;
    public bool IsAreaAttack => _isAreaAttack;
    public bool IsPassiveSkill => _isPassiveSkill; //자가 구동을 위한 bool 값 입력부
    public bool IsUltSkill => _isUltSkill; //자가 구동을 위한 bool 값 입력부
    public string Description { get => _description; set =>_description = value;  }// => _description;
    
    public UnitEvents SkillEventType => _skillEventType;
    public SkillFXSpawnPosType SkillFXSpawnPosType => _skillFxSpawnPosType;

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

        if (_skillLevelDatas.Count - 1 <= skillIndex)
        {
            _description = _skillLevelDatas[skillIndex].description;
        }
        else
        {
            _description = _skillLevelDatas[skillIndex+1].description;
        }

        return _skillLevelDatas[skillIndex];
    }

    public abstract bool IsValidTarget(Unit unit);
    public abstract void OnAction(Skill skill, Unit user, List<Unit> targets);
}

[Serializable]
public class SkillLevelData
{
    [Range(0f, 100f)] public float activationChance;
    public float skillValue; // n%
    public float coolTime; //쿨타임 입력
    public int targetNum; //타켓수 지정방식
    public float range; //공격 범위 크기?
    public string description;
    public GameObject skillFxPrefab;
}

public class SkillDataNotFoundException : Exception
{
    public SkillDataNotFoundException(string message) : base(message)
    {
    }
}