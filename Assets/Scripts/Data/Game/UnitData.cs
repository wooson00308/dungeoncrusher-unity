using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Data/Unit/Create UnitData")]
public class UnitData : ScriptableObject, IStats
{
    [Header("Config")] [SerializeField] private string _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private GameObject _hitPrefab;

    [Header("원거리")] [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Warning _warningPrefab;

    [Header("Stats")] [SerializeField] private IntStat _health;
    [SerializeField] private IntStat _attack;
    [SerializeField] private IntStat _defense;
    [SerializeField] private IntStat _mp;
    [SerializeField] private FloatStat _speed;
    [SerializeField] private FloatStat _attackSpeed;
    [SerializeField] private FloatStat _attackRange;

    [Tooltip("발생확률")] [SerializeField] private FloatStat _criticalRate;
    [Tooltip("데미지 퍼센트")] [SerializeField] private FloatStat _criticalPercent;
    [SerializeField] private FloatStat _stunRate;
    [Tooltip("발생확률")] [SerializeField] private FloatStat _lifestealRate;
    [Tooltip("흡혈 퍼센트")] [SerializeField] private FloatStat _lifestealPercent;

    public string Id => _id;
    public GameObject Prefab => _prefab;
    public GameObject HitPrefab => _hitPrefab;

    public Projectile ProjectilePrefab => _projectilePrefab;
    public Warning WarningPrefab => _warningPrefab;
    public IntStat Health => _health;
    public IntStat Attack => _attack;
    public IntStat Defense => _defense;
    public IntStat Mp => _mp;
    public FloatStat Speed => _speed;
    public FloatStat AttackSpeed => _attackSpeed;
    public FloatStat AttackRange => _attackRange;

    public FloatStat CriticalRate => _criticalRate;
    public FloatStat CriticalPercent => _criticalPercent;
    public FloatStat AttackStunRate => _stunRate;
    public FloatStat LifestealRate => _lifestealRate;
    public FloatStat LifestealPercent => _lifestealPercent;
}