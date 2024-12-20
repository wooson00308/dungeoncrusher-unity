using UnityEngine;

public class StunState : StateBase, IState
{
    private float _currentCheckDelay = 0;

    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Stun", 0f);
        unit.Stop();

        _currentCheckDelay = 0;
    }

    public void OnExit(Unit unit)
    {
        
    }

    public void OnUpdate(Unit unit)
    {
        if (!unit.IsStun)
        {
            _fsm.TransitionTo<ChaseState>();
        }

        if (_currentCheckDelay <= 0.1f)
        {
            _currentCheckDelay += Time.deltaTime;
            return;
        }

        if (!unit.IsUpdateState("Stun"))
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }
}
