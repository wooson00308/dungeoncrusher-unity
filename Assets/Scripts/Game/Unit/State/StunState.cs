using UnityEngine;

public class StunState : StateBase, IState
{
    private float _currentCheckDelay = 0;
    private float _stunDuration;

    public void OnStun(float stunDuration = 0.1f)
    {
        _stunDuration = stunDuration;
        _fsm.TransitionTo<StunState>();
    }

    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Stun", 0f);
        unit.Stop();
        unit.IsStun = true;

        _currentCheckDelay = 0;
    }

    public void OnExit(Unit unit)
    {
        unit.IsStun = false;
    }

    public void OnUpdate(Unit unit)
    {
        if (_currentCheckDelay < _stunDuration)
        {
            _currentCheckDelay += Time.deltaTime;
            return;
        }

        _fsm.TransitionTo<ChaseState>();
    }
}