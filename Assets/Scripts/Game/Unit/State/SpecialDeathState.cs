public class SpecialDeathState : StateBase, IState
{
    public void OnEnter(Unit unit)
    {
        unit.CrossFade("SpecialDeath", 0f);
        _fsm.LockState();
    }

    public void OnUpdate(Unit unit)
    {
    }

    public void OnExit(Unit unit)
    {
    }
}
