using UnityEngine;

[CreateAssetMenu(fileName = "UnitEventSkillConditionData", menuName = "Scriptable Objects/UnitEventSkillConditionData")]
public class UnitEventSkillConditionData : SkillConditionData
{
    public UnitEvents UnitEvent;

    public override int EventId => (int)UnitEvent;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if(!base.IsSatisfied(skill, gameEvent)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        return true;
    }
}
