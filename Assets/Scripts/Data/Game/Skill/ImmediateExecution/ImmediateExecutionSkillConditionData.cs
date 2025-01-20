using UnityEngine;

[CreateAssetMenu(fileName = "ImmediateExecutionSkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/ImmediateExecutionSkillConditionData")]
public class ImmediateExecutionSkillConditionData : SkillConditionData //즉결처형 - 적 공격시 일정 확률로 처형
{
    public UnitEvents condition;
    [SerializeField] private int executionPercent;

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
        if (random > executionPercent) return false;
        return true;
    }
}