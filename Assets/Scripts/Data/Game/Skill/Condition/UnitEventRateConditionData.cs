using UnityEngine;

[CreateAssetMenu(fileName = "UnitEventRateConditionData", menuName = "Data/UnitEventRateConditionData")]
public class UnitEventRateConditionData : UnitEventConditionData
{
    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;
        if (!Operator.IsRate(skill.CurrentLevelData.Rate)) return false;

        return true;
    }
}