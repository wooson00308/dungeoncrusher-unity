using UnityEngine;

[CreateAssetMenu(fileName = "ExplodeSoulCondition", menuName = "Scriptable Objects/Skill/Condition/ExplodeSoulCondition")]
public class ExplodeSoulCondition : UnitEventConditionData
{
    [SerializeField] private int explodingRate;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;
        if (!Operator.IsRate(explodingRate)) return false;
        
        return true;
    }
}