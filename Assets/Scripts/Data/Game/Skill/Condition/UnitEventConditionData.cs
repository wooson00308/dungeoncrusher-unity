using UnityEngine;

[CreateAssetMenu(fileName = "UnitEventConditionData", menuName = "Scriptable Objects/UnitEventConditionData")]
public class UnitEventConditionData : ConditionData
{
    public UnitEvents UnitEvent;

    public override int EventId => (int)UnitEvent;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;
        //if (gameEvent is not UnitEventArgs args) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        return true;
    }
}