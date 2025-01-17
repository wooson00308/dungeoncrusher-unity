using UnityEngine;

public class UnitEventSkillConditionData : SkillConditionData
{
    public UnitEvents UnitEvent;

    public override int EventId => (int)UnitEvent;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if(!base.IsSatisfied(skill, gameEvent)) return false;

        // °ø°ÝÀ» 
        if (gameEvent is UnitEventWithAttackerArgs args && 
            !skill.Owner.EqualsUnit(args.attacker)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        return true;
    }
}
