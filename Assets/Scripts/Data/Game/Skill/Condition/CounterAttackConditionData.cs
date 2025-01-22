using UnityEngine;

[CreateAssetMenu(fileName = "CounterAttackSkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/CounterAttackSkillConditionData")]
public class CounterAttackConditionData : UnitEventSkillConditionData
{
    [SerializeField] private int counterAttackRate;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        if (!Operator.IsRate(counterAttackRate)) return false;

        return true;
    }
}