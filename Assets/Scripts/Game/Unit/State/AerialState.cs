using UnityEngine;

public class AerialState : StateBase, IState
{
    [SerializeField] private float aerialTime;
    private float currentAerialTime = 0;

    public void OnEnter(Unit unit)
    {
        Debug.Log("Aerial State");
        unit.CrossFade("Aerial", 0f);
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