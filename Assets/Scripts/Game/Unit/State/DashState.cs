using System;
using UnityEngine;
using UnityEngine.AI;

public class DashState : StateBase, IState
{
    private bool _isEndDash;
    private bool _isDashing;
    private Vector2 _dashDirection;
    private Vector2 _dashPos;
    private float _dashSpeed;
    private float _remainingDistance;

    private Action _exitCallback;

    public void OnDash(Unit unit, float dashSpeed, float additionalDistance, Action exitCallback = null)
    {
        if (_isDashing) return;

        _dashSpeed = dashSpeed;
        _remainingDistance = additionalDistance;
        _isEndDash = false;

        _fsm.TransitionTo<DashState>();
        _exitCallback = exitCallback;
    }

    public void OnEnter(Unit unit)
    {
        if (IsStun(unit))
        {
            _fsm.TransitionTo<ChaseState>();
            return;
        }

        unit.CrossFade("Dash", 0f);
        unit.Stop();

        var target = unit.Target;
        
        if (target == null)
        {
            _fsm.TransitionTo<ChaseState>();
            return;
        }

        Vector2 direction = target.transform.position - unit.transform.position;
        _dashDirection = direction.normalized;
        _dashPos = target.transform.position;

        _isDashing = true;
    }

    public void OnExit(Unit unit)
    {
        _isDashing = false;
        _exitCallback?.Invoke();
    }

    public void OnUpdate(Unit unit)
    {
        if (IsStun(unit))
        {
            _fsm.TransitionTo<ChaseState>();
            return;
        }

        if (!_isEndDash)
        {
            PerformDash(unit);
        }
        else
        {
            PerformAdditionalDash(unit);
        }
    }

    private void PerformDash(Unit unit)
    {
        unit.Rotation(_dashDirection);

        float moveDistance = _dashSpeed * GameTime.DeltaTime;
        Vector3 newPosition = unit.transform.position + (Vector3)_dashDirection * moveDistance;

        if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            unit.Warp(hit.position);
        }

        if (Vector2.Distance(unit.transform.position, _dashPos) < 0.5f)//벽에 끼는 문제 이것 때문일 수 도 있을 듯 
        {
            _isEndDash = true;
        }
    }

    private void PerformAdditionalDash(Unit unit)
    {
        float moveDistance = _dashSpeed * GameTime.DeltaTime;
        float moveDelta = Mathf.Min(_remainingDistance, moveDistance);
        _remainingDistance -= moveDelta;

        Vector3 additionalPosition = unit.transform.position + (Vector3)_dashDirection * moveDelta;

        if (NavMesh.SamplePosition(additionalPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            unit.Warp(hit.position);
        }

        if (_remainingDistance <= 0f)
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }
}
