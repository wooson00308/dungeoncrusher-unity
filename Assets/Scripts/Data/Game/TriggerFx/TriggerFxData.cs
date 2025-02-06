using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerFxData", menuName = "Data/TriggerFxData")]
public class TriggerFxData : Data
{
    [field: SerializeField] public int MaxMultiTriggerCount { get; private set; }
    [field: SerializeField] public float Durtaion { get; private set; }
    [field: SerializeField] public float EventPerTime { get; private set; }
    [field: SerializeField] public bool OnDestroyEventWhenCollideUnit { get; private set; }
    [field: SerializeField] public bool OnEventFromSelf { get; private set; }
    [field: SerializeField] public bool OnEventFromOwnerProjectile { get; private set; }
    [field: SerializeField] public bool OnEventOnce { get; private set; }
    [field: SerializeField] public List<FxEventData> EnterFxDatas { get; private set; }
    [field: SerializeField] public List<FxEventData> StayFxDatas { get; private set; }
    [field: SerializeField] public List<FxEventData> ExitFxDatas { get; private set; }
}
