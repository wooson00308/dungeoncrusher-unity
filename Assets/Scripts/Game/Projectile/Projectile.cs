using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected ProjectileData _data;
    protected Unit _target;
    protected Vector2 _targetPos;
    protected int _damage;

    protected void FixedUpdate()
    {
        OnMove();

        if (IsTargetInSight())
        {
            TargetHit();
        }
        else if (IsTargetPosInSight())
        {
            TargetPosHit();
        }
    }

    public virtual void Initialize(Unit target, Vector2 targetPos, int damage)
    {
        this._target = target;
        this._targetPos = targetPos;
        this._damage = damage;
    }

    protected abstract void OnMove();

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.detectRange);
    }

    protected abstract void TargetHit();

    protected virtual void TargetPosHit()
    {
    }

    protected bool IsTargetInSight()
    {
        return Vector3.Distance(transform.position, _target.transform.position) <= _data.detectRange;
    }

    protected bool IsTargetPosInSight()
    {
        return Vector3.Distance(transform.position, _targetPos) <= _data.detectRange;
    }
    //도착하는 시간과 위치를 반환하는 함수 필요할듯
}