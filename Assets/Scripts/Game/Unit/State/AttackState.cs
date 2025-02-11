using UnityEngine;

[RequireComponent(typeof(ChaseState))]
public class AttackState : StateBase, IState
{
    [SerializeField] private int _animationSize = 1;
    private string _currentState;
    private int _currentStateIndex;

    private float _currentAttackStateCheckDelay = 0;

    public void OnEnter(Unit unit)
    {
        if (IsStun(unit)) return;

        _currentStateIndex = Random.Range(0, _animationSize);
        _currentState = $"Attack {_currentStateIndex + 1}";
        unit.CrossFade(_currentState, 0f);
        unit.Stop();

        _currentAttackStateCheckDelay = 0;
    }

    public void OnExit(Unit unit)
    {
    }

    public void OnUpdate(Unit unit)
    {
        if (IsStun(unit)) return;

        if (_currentAttackStateCheckDelay <= 0.1f)
        {
            _currentAttackStateCheckDelay += GameTime.DeltaTime;
            return;
        }

        if (unit.Target == null) return;
        
        unit.Rotation(unit.Target.transform.position - unit.transform.position);

        if (unit.GetAttackState() == 0)
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }
}