using UnityEngine;

[CreateAssetMenu(fileName = "ProcessEventConditionData",
    menuName = "Scriptable Objects/Skill/Condition/ProcessEventConditionData")]
public class ProcessEventConditionData : ConditionData
{
    public ProcessEvents ProcessEvent;
    public override int EventId => (int)ProcessEvent;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        return base.IsSatisfied(skill, gameEvent);
    }
}