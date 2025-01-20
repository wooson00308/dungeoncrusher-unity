using UnityEngine;

[CreateAssetMenu(fileName = "PowerStrikeConditionData",
    menuName = "Scriptable Objects/Skill/Condition/PowerStrikeConditionData")]
public class PowerStrikeConditionData : SkillConditionData
{
    public UnitEvents UnitEvent;

    [SerializeField] private int powerStrikeRate;

    public override int EventId => (int)UnitEvent;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        int random = Random.Range(1, 100);
        if (random > powerStrikeRate) return false;
        return true;
    }
}