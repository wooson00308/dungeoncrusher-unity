using UnityEngine;

public class CurveProjectile : Projectile
{
    [SerializeField] private Vector2 _curve;
    [SerializeField] private Linoleum _linoleum;

    private Vector2[] moveVec;

    public override void Initialize(Unit target, Vector2 targetPos, int damage)
    {
        base.Initialize(target, targetPos, damage);

        SetMoveVec();
    }

    private void SetMoveVec()
    {
        var halfVec = Vector2.Lerp(transform.position, _targetPos, 0.5f);
        Vector2 moveY = new Vector2(halfVec.x + _curve.x, _curve.y);
        moveVec = new Vector2[3]
        {
            transform.position, moveY,
            _targetPos
        };
    }

    protected override void OnMove()
    {
        transform.position = Bezior.BeziorMove(moveVec, _data.moveSpeed);
    }

    protected override void TargetHit()
    {
        _target.OnHit(_damage);
        var spawnLinoleum = ResourceManager.Instance.Spawn(_linoleum.gameObject).GetComponent<Linoleum>();
        spawnLinoleum.Initialize(_damage);
        spawnLinoleum.transform.position = transform.position;
        ResourceManager.Instance.Destroy(gameObject);
    }

    protected override void TargetPosHit()
    {
        var spawnLinoleum = ResourceManager.Instance.Spawn(_linoleum.gameObject).GetComponent<Linoleum>();
        spawnLinoleum.Initialize(_damage);
        spawnLinoleum.transform.position = transform.position;
        ResourceManager.Instance.Destroy(gameObject);
    }
}