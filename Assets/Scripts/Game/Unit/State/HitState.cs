using UnityEngine;

public class HitState : StateBase, IState
{
    private float _currentAttackStateCheckDelay = 0;

    public void OnEnter(Unit unit)
    {
        if (IsAerial(unit)) return;

        unit.CrossFade("Hit", 0f);
        unit.Stop();

        _currentAttackStateCheckDelay = 0;
    }

    public void OnExit(Unit unit)
    {
    }

    public void OnUpdate(Unit unit)
    {
        if (_currentAttackStateCheckDelay <= 0.1f)
        {
            _currentAttackStateCheckDelay += Time.deltaTime;
            return;
        }

        if (!unit.IsUpdateState("Hit"))
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }
}