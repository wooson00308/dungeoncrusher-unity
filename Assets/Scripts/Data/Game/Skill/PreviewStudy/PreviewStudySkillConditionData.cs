using UnityEngine;

[CreateAssetMenu(fileName = "PreviewStudySkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/PreviewStudySkillConditionData")]
public class PreviewStudySkillConditionData : SkillConditionData
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