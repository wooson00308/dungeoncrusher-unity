using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsUpgradeData", menuName = "Data/UnitStatsUpgradeData")]
public class UnitStatsUpgradeData : ScriptableObject, IStats
{
    [SerializeField] private string _upgradeName;
    [SerializeField] private Sprite _upgradeIcon;
    [SerializeField] private string _description;

    public string UpgradeName => _upgradeName;
    public Sprite UpgradeIcon => _upgradeIcon;
    public string Description => _description;
    [field: SerializeField] public IntStat Health { get; set; }
    [field: SerializeField] public IntStat Attack { get; set; }
    [field: SerializeField] public IntStat Defense { get; set; }
    [field: SerializeField] public IntStat Mp { get; set; }
    [field: SerializeField] public FloatStat MpPercent { get; set; }
    [field: SerializeField] public IntStat Exp { get; set; }
    [field: SerializeField] public FloatStat ExpPercent { get; set; }
    [field: SerializeField] public IntStat Level { get; set; }
    [field: SerializeField] public IntStat StageLevel { get; set; }
    [field: SerializeField] public FloatStat Speed { get; set; }
    [field: SerializeField] public FloatStat AttackSpeed { get; set; }
    [field: SerializeField] public FloatStat AttackRange { get; set; }
    [field: SerializeField] public FloatStat CriticalRate { get; set; }
    [field: SerializeField] public FloatStat CriticalPercent { get; set; }
    [field: SerializeField] public FloatStat AttackStunRate { get; set; }
    [field: SerializeField] public FloatStat LifestealRate { get; set; }
    [field: SerializeField] public FloatStat LifestealPercent { get; set; }
}