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
        transform.Translate(moveDir * _data.moveSpeed * Time.deltaTime);
    }

    protected override void TargetHit()
    {
        _target.OnHit(_damage);
        ResourceManager.Instance.Destroy(gameObject);
    }
}