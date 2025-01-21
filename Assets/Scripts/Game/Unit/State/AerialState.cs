using UnityEngine;

public class AerialState : StateBase, IState
{
    [SerializeField] private float aerialTime = 1;
    private float currentAerialTime = 0;

    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Aerial", 0f);
        unit.Stop();
        unit.IsAerial = true;
    }

    public void OnUpdate(Unit unit)
    {
        currentAerialTime += GameTime.DeltaTime;
        if (currentAerialTime >= aerialTime)
        {
            _fsm.TransitionTo<ChaseState>();
            currentAerialTime = 0;
        }
    }

    public void OnExit(Unit unit)
    {
        unit.IsAerial = false;
    }
}