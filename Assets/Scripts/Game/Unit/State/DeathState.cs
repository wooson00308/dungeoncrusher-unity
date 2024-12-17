using UnityEngine;

public class DeathState : StateBase, IState
{
    public void OnEnter(Unit unit)
    {
        unit.CrossFade("Death", 0f);
    }

    public void OnExit(Unit unit)
    {
    }

    public void OnUpdate(Unit unit)
    {

    }
}