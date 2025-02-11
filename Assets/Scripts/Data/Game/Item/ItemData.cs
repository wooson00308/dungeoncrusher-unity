using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : Data, IStats
{
    // [SerializeField] protected string _id;
    [SerializeField] protected GameObject _prefab;
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected PartType _partType;

    [Header("Stats")] [SerializeField] private IntStat _health;
    
    [SerializeField] private IntStat _attack;
    [SerializeField] private FloatStat _ap;
    [SerializeField] private FloatStat _ad;

    [SerializeField] private IntStat _defense;
    [SerializeField] private IntStat _mp;
    [SerializeField] private FloatStat _mpPercent;
    [SerializeField] private IntStat _maxMp;
    [SerializeField] private IntStat _exp;
    [SerializeField] private FloatStat _expPercent;
    [SerializeField] private IntStat _level;
    [SerializeField] private IntStat _stageLevel;
    [SerializeField] private FloatStat _speed;
    [SerializeField] private FloatStat _attackSpeed;
    [SerializeField] private FloatStat _attackRange;

    [SerializeField] private FloatStat _criticalRate;
    [SerializeField] private FloatStat _criticalDamage;
    [SerializeField] private FloatStat _stunRate;
    [SerializeField] private FloatStat _lifestealRate;
    [SerializeField] private FloatStat _lifestealPercent;
    [SerializeField] protected List<SkillData_old> _skillDatas;

    // public string Id => _id;
    public GameObject Prefab => _prefab;
    public PartType PartType => _partType;
    public Sprite Icon => _icon;

    public IntStat Health => _health;
    public IntStat Attack => _attack;
    public FloatStat AP => _ap;
    public FloatStat AD => _ad;
    public IntStat Defense => _defense;
    public IntStat Mp => _mp;
    public FloatStat MpPercent => _mpPercent;
    public IntStat Exp => _exp;
    public FloatStat ExpPercent => _expPercent;
    public IntStat Level => _level;
    public IntStat StageLevel => _stageLevel;
    public IntStat MaxMp => _maxMp;
    public FloatStat Speed => _speed;
    public FloatStat AttackSpeed => _attackSpeed;
    public FloatStat AttackRange => _attackRange;

    public FloatStat CriticalRate => _criticalRate;
    public FloatStat CriticalPercent => _criticalDamage;
    public FloatStat AttackStunRate => _stunRate;
    public FloatStat LifestealRate => _lifestealRate;
    public FloatStat LifestealPercent => _lifestealPercent;
}

public enum PartType
{
    None,
    Weapon,
    Helmet,
    Armor
}