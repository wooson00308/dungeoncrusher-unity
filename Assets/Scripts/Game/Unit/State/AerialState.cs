using UnityEngine;

public class AerialState : StateBase, IState
{
    [SerializeField] private float aerialTime = 1;
    private float _currentAerialTime = 0;

    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Aerial", 0f);
        unit.Stop();
        unit.IsAerial = true;
    }

    public void OnUpdate(Unit unit)
    {
        _currentAerialTime += GameTime.DeltaTime;
        if (_currentAerialTime >= aerialTime)
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }

    public void OnExit(Unit unit)
    {
        _currentAerialTime = 0;
        unit.IsAerial = false;
    }
}