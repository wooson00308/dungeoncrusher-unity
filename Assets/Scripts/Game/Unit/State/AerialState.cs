using UnityEngine;

public class AerialState : StateBase, IState
{
    [SerializeField] private float aerialTime = 1;
    private float currentAerialTime = 0;

    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Aerial", 0f);
        unit.Stop();
    }

    public void OnUpdate(Unit unit)
    {
        currentAerialTime += Time.deltaTime;
        if (currentAerialTime >= aerialTime)
        {
            _fsm.TransitionTo<ChaseState>();
            currentAerialTime = 0;
        }
    }

    public void OnExit(Unit unit)
    {
    }
}