using UnityEngine;

public class StateBase : MonoBehaviour
{
    protected FSM _fsm;

    protected bool IsStun(Unit unit)
    {
        return unit.IsUpdateState("Stun");
    }

    protected bool IsAerial(Unit unit)
    {
        return unit.IsUpdateState("Aerial");
    }

    protected virtual void Awake()
    {
        _fsm = GetComponent<FSM>();
    }
}