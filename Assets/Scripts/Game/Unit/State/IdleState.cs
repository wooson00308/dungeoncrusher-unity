using UnityEngine;

[RequireComponent(typeof(ChaseState))]
public class IdleState : StateBase, IState
{
    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Idle", 0f);
    }

    public void OnExit(Unit unit)
    {
        
    }

    public void OnUpdate(Unit unit)
    {
        if(unit.Target != null)
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }
}
