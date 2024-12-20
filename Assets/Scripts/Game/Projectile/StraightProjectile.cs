using UnityEngine;

public class StraightProjectile : Projectile
{
    protected override void OnMove()
    {
        Vector2 movePos = _targetPos - (Vector2)transform.position;
        transform.Translate(movePos * _data.moveSpeed * Time.deltaTime);
    }

    protected override void TargetHit()
    {
        _target.OnHit(_damage);
        ResourceManager.Instance.Destroy(gameObject);
    }
}