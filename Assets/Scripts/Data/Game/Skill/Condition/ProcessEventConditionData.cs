using UnityEngine;

[CreateAssetMenu(fileName = "ProcessEventConditionData",
    menuName = "Scriptable Objects/ProcessEventConditionData")]
public class ProcessEventConditionData : ConditionData
{
    public ProcessEvents ProcessEvent;
    public override int EventId => (int)ProcessEvent;
}