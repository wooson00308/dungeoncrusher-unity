using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatsUpgradeData", menuName = "Data/UnitStatsUpgradeData")]
public class UnitStatsUpgradeData : Data, IStats
{
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
    [field: SerializeField] public IntStat Health { get; set; }
    [field: SerializeField] public IntStat Attack { get; set; }
    [field: SerializeField] public FloatStat AP { get; set; }
    [field: SerializeField] public FloatStat AD { get; set; }
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