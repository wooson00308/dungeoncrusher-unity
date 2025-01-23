using UnityEngine;

[CreateAssetMenu(fileName = "ProcessEventConditionData",
    menuName = "Data/ConditionData/ProcessEventConditionData")]
public class ProcessEventConditionData : ConditionData
{
    public ProcessEvents ProcessEvent;
    public override int EventId => (int)ProcessEvent;
}