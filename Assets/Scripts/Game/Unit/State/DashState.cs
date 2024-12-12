using System;
using UnityEngine;
using UnityEngine.AI;

public class DashState : StateBase, IState
{
    private bool _isDashing;
    private Vector2 _direction;
    private Vector2 _dashDirection;
    private float _dashSpeed;

    private Action _exitCallback;

    public void OnDash(Unit unit, float dashSpeed, Action exitCallback = null)
    {
        if (_isDashing) return;

        _dashSpeed = dashSpeed;

        _fsm.TransitionTo<DashState>();

        _exitCallback = exitCallback;
    }

    public void OnEnter(Unit unit)
    {
        if(IsStun(unit))
        {
            _fsm.TransitionTo<ChaseState>();
            return;
        }

        unit.CrossFade("Dash", 0f);
        unit.Stop();

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

        _direction = unit.Target.transform.position - transform.position;
        _dashDirection = new Vector2(_direction.x, _direction.y).normalized;

        float deltaTime = Time.deltaTime;
        float moveDistance = _dashSpeed * deltaTime;
        Vector3 newPosition = transform.position + (Vector3)_dashDirection * moveDistance;

        // NavMesh 위에서 위치 업데이트
        if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            unit.Warp(hit.position); // NavMesh 위에 강제로 위치 설정
        }

        if(Vector2.Distance(unit.transform.position, unit.Target.transform.position) < 0.1f)
        {
            _fsm.TransitionTo<ChaseState>();
        }
    }
}
