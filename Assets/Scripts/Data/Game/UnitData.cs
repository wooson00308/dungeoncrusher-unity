using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Data/Unit/Create UnitData")]
public class UnitData : ScriptableObject, IStats
{
    [Header("Config")]
    [SerializeField] private string _id;
    [SerializeField] private string _prfId;

    [Header("Stats")]
    [SerializeField] private int _health;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
    [SerializeField] private float _speed;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;

    [SerializeField] private float _criticalRate;
    [SerializeField] private float _stunRate;
    [SerializeField] private float _lifestealRate;

    public string Id => _id;
    public string PrfId => _prfId;

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