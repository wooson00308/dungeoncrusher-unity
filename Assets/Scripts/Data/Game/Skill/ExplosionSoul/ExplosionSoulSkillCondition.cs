using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionSoulSkillCondition",
    menuName = "Scriptable Objects/Skill/Condition/ExplosionSoulSkillCondition")]
public class ExplosionSoulSkillCondition : SkillConditionData
{
    public UnitEvents condition;
    [SerializeField] private int explosionPercent;

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

        int random = Random.Range(1, 100);
        if (random > explosionPercent) return false;

        return true;
    }
}