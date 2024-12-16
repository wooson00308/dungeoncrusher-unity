using System;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class FSM : MonoBehaviour
{
    private Unit _unit;
    private IState _currentState;

    public Unit Unit => _unit;

    private string _currentStateName;
    public string CurrentStateName => _currentStateName;
    private bool _isLocked = false;
    public void LockState() => _isLocked = true;
    public void UnlockState() => _isLocked = false;
    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    public void TransitionTo<T>() where T : MonoBehaviour, IState
    {
        if (_isLocked)
            return;
        _currentState?.OnExit(_unit);

        if (!TryGetComponent<T>(out var nextState))
        {
            Debug.LogError($"State {typeof(T).Name} not found on {gameObject.name}");
            return;
        }

        _currentState = nextState;
        _currentStateName = typeof(T).Name;
        _currentState.OnEnter(_unit);
    }

    private void Update()
    {
        _currentState?.OnUpdate(_unit);
    }

    public void StartState<T>() where T : MonoBehaviour, IState
    {
        TransitionTo<T>();
    }
}