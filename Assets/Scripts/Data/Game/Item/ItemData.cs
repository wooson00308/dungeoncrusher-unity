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

    [Header("Stats")] [SerializeField] private int _health;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
    [SerializeField] private float _speed;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;

    [SerializeField] private float _criticalRate;
    [SerializeField] private float _stunRate;
    [SerializeField] private float _lifestealRate;
    [SerializeField] protected List<SkillData> _skillDatas;

    public string Id => _id;
    public string UnitId => _unitId;
    public PartType PartType => _partType;
    public string IconId => _iconId;
    public Sprite Icon => _icon;

    public int Health => _health;
    public int Attack => _attack;
    public int Defense => _defense;
    public float Speed => _speed;
    public float AttackSpeed => _attackSpeed;
    public float AttackRange => _attackRange;

    public float CriticalRate => _criticalRate;
    public float AttackStunRate => _stunRate;
    public float LifestealRate => _lifestealRate;
}

public enum PartType
{
    None,
    Weapon,
    Helmet,
    Armor
}