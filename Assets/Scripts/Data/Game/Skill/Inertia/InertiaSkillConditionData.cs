using UnityEngine;

[CreateAssetMenu(fileName = "InertiaSkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/InertiaSkillConditionData")]
public class InertiaSkillConditionData : SkillConditionData //관성 - 적을 처치했을 때, 공격력이 x만큼 증가하는 버프 효과를 얻음 최대 5중첩
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