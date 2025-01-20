using UnityEngine;

[CreateAssetMenu(fileName = "SoulAbsorberSkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/SoulAbsorberSkillConditionData")]
public class SoulAbsorberSkillConditionData : SkillConditionData // 영혼흡수자 - 적 처치시 최대체력 1증가
{
    public UnitEvents condition;

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

        return true;
    }
}