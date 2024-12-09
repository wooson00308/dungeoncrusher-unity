using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Data/Unit/Create UnitData")]
public class UnitData : ScriptableObject, IStats
{
    [Header("Config")] [SerializeField] private string _id;
    [SerializeField] private string _prfId;

    [Header("Stats")] [SerializeField] private IntStat _health;
    [SerializeField] private IntStat _attack;
    [SerializeField] private IntStat _defense;
    [SerializeField] private IntStat _mp;
    [SerializeField] private FloatStat _speed;
    [SerializeField] private FloatStat _attackSpeed;
    [SerializeField] private FloatStat _attackRange;

    [SerializeField] private FloatStat _criticalRate;
    [SerializeField] private FloatStat _stunRate;
    [SerializeField] private FloatStat _lifestealRate;

    public string Id => _id;
    public string PrfId => _prfId;

    public IntStat Health => _health;
    public IntStat Attack => _attack;
    public IntStat Defense => _defense;
    public IntStat Mp => _mp;
    public FloatStat Speed => _speed;
    public FloatStat AttackSpeed => _attackSpeed;
    public FloatStat AttackRange => _attackRange;

    public FloatStat CriticalRate => _criticalRate;
    public FloatStat AttackStunRate => _stunRate;
    public FloatStat LifestealRate => _lifestealRate;
}