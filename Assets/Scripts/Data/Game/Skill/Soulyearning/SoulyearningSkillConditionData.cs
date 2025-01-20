using UnityEngine;

[CreateAssetMenu(fileName = "SoulyearningSkillConditionData", menuName = "Scriptable Objects/Skill/Condition/SoulyearningSkillConditionData")]
public class SoulyearningSkillConditionData : SkillConditionData//영혼갈망 - 공격시 마력 획득량 증가
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