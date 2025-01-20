using UnityEngine;

[CreateAssetMenu(fileName = "DisdainWeekSkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/DisdainWeekSkillConditionData")]
public class DisdainWeekSkillConditionData : SkillConditionData // 약자멸시 - 공격하는 적의 현재 체력이 해당 몬스터 최대 체력의 n% 이하일 경우 피해량이 증가
{
    public UnitEvents condition;
    [SerializeField] private int healthPercent;

    public override int EventId
    {
        get { return (int)condition; }
    }

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        if (gameEvent is not UnitEventWithAttackerArgs args) return false;
        if (!skill.Owner.EqualsUnit(args.attacker)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        if (owner.Target.Health.Value > owner.Target.Health.Max * (healthPercent * 100)) return false;
        return true;
    }
}