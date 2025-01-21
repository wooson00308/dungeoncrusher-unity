using System;
using UnityEngine;

public abstract class Projectile_old : MonoBehaviour
{
    [SerializeField] protected ProjectileData_old _data;
    protected Unit _target;
    protected Vector2 _targetPos;
    protected int _damage;

    protected void FixedUpdate()
    {
        if (IsTargetInSight())
        {
            TargetHit();
        }
        else if (IsTargetPosInSight())
        {
            TargetPosHit();
        }

        OnMove();
    }

    public virtual void Initialize(Unit target, Vector2 targetPos, int damage)
    {
        _target = target;
        _targetPos = targetPos;
        _damage = damage;
    }

    protected abstract void OnMove();

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.projectileInfo.detectRange);
    }

    protected abstract void TargetHit();

    protected virtual void TargetPosHit()
    {
    }

    protected bool IsTargetInSight()
    {
        if (_target == null) return false;
        return Vector3.Distance(transform.position, _target.transform.position) <= _data.projectileInfo.detectRange;
    }

    protected bool IsTargetPosInSight()
    {
        return Vector3.Distance(transform.position, _targetPos) <= 0.1f;
    }
}