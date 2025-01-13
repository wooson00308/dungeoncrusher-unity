using UnityEngine;

public class StraightProjectile : Projectile
{
    private Vector2 moveDir;

    public override void Initialize(Unit target, Vector2 targetPos, int damage)
    {
        base.Initialize(target, targetPos, damage);
        moveDir = _targetPos - (Vector2)transform.position;
    }

    protected override void OnMove()
    {
        transform.Translate(moveDir * _data.projectileInfo.moveSpeed * Time.deltaTime);
    }

    protected override void TargetHit()
    {
        _target.OnHit(_damage);
        
        if (_data.projectileInfo.isStun)
        {
            _target.OnStun();
        }

        if (_data.projectileInfo.isAerial)
        {
            _target.OnAerial();
        }

        ResourceManager.Instance.Destroy(gameObject);
    }
}