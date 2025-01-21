using UnityEngine;

[CreateAssetMenu(fileName = "CallihansSpearConditionData",
    menuName = "Scriptable Objects/Skill/Condition/CallihansSpearConditionData")]
public class CallihansSpearConditionData : UnitEventSkillConditionData
{
    [SerializeField] private int callihansSpearRate;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        if (!Operator.IsRate(callihansSpearRate)) return false;
        return true;
    }
}