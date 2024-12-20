using UnityEngine;

[RequireComponent(typeof(IdleState))]
[RequireComponent(typeof(AttackState))]
public class ChaseState : StateBase, IState
{
    public void OnEnter(Unit unit)
    {
        //if (IsStun(unit)) return;

        if (unit.Target == null)
        {
            _fsm.TransitionTo<IdleState>();
            return;
        }

        unit.CrossFade("Chase", 0f);
    }

    public void OnExit(Unit unit)
    {

    }

    public void OnUpdate(Unit unit)
    {
        //if (IsStun(unit)) return;

        if (unit.Target == null)
        {
            _fsm.TransitionTo<IdleState>();
            return;
        }

        unit.MoveFromTarget(unit.Target.transform);

        if (Vector2.Distance(unit.transform.position, unit.Target.transform.position) <= unit.AttackRange.Value)
        {
            _fsm.TransitionTo<AttackState>();
        }
    }
}
