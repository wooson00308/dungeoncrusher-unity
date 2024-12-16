using System.Linq;
using UnityEngine;

public class SpecialDeathState : StateBase, IState
{
    public void OnEnter(Unit unit)
    {
        unit.CrossFade("SpecialDeath", 0f);
    }

    public void OnUpdate(Unit unit)
    {
    }

    public void OnExit(Unit unit)
    {
    }
}
