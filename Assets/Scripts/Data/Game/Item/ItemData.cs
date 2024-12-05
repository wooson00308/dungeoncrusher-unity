using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject, IStats
{
    [SerializeField] protected string _id;
    [SerializeField] protected string _unitId;
    [SerializeField] protected string _name;
    [SerializeField] protected string _iconId;
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected PartType _partType;
    [SerializeField] protected string _description;

    [Header("Stats")] [SerializeField] private IntStat _health;
    [SerializeField] private IntStat _attack;
    [SerializeField] private IntStat _defense;
    [SerializeField] private FloatStat _speed;
    [SerializeField] private FloatStat _attackSpeed;
    [SerializeField] private FloatStat _attackRange;

    [SerializeField] private FloatStat _criticalRate;
    [SerializeField] private FloatStat _stunRate;
    [SerializeField] private FloatStat _lifestealRate;
    [SerializeField] protected List<SkillData> _skillDatas;

    public string Id => _id;
    public string UnitId => _unitId;
    public PartType PartType => _partType;
    public string IconId => _iconId;
    public Sprite Icon => _icon;

    public IntStat Health => _health;
    public IntStat Attack => _attack;
    public IntStat Defense => _defense;
    public FloatStat Speed => _speed;
    public FloatStat AttackSpeed => _attackSpeed;
    public FloatStat AttackRange => _attackRange;

    public FloatStat CriticalRate => _criticalRate;
    public FloatStat AttackStunRate => _stunRate;
    public FloatStat LifestealRate => _lifestealRate;
}

public enum PartType
{
    None,
    Weapon,
    Helmet,
    Armor
}