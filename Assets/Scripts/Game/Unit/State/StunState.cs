using UnityEngine;

public class StunState : StateBase, IState
{
    private float _currentTime;

    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Stun", 0f);
    }

    public void OnExit(Unit unit)
    {
        
    }

    public void OnUpdate(Unit unit)
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= unit.StunDuration)
        {
            _currentTime = 0f;
            _fsm.TransitionTo<ChaseState>();
        }
    }
}
