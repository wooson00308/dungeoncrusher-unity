using UnityEngine;

public class CurveProjectile : Projectile_old
{
    #region Fields

    [SerializeField] private Vector2 _curve;
    [SerializeField] private GameObject _linoleum;
    [SerializeField] private GameObject model;

    [Header("회전")] [SerializeField] private bool isRotate;
    [Header("Z축기준")] [SerializeField] private float startAngle;
    [SerializeField] private float endAngle;

    private Vector2[] moveVec;

    private float currentTime = 0;
    private float endTime;

    #endregion

    public override void Initialize(Unit target, Vector2 targetPos, int damage)
    {
        base.Initialize(target, targetPos, damage);
        SetMoveVec();
    }

    private void SetMoveVec()
    {
        var halfVec = Vector2.Lerp(transform.position + new Vector3(0, 0.5f), _targetPos, 0.5f);
        Vector2 moveY = new Vector2(halfVec.x + _curve.x, _curve.y);
        moveVec = new Vector2[3]
        {
            transform.position, moveY,
            _targetPos
        };

        currentTime = 0;
        endTime = _data.projectileInfo.moveSpeed;
    }

    private void RotateModel(float t)
    {
        if (isRotate)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(startAngle, endAngle, t));
        }
    }

    protected override void OnMove()
    {
        currentTime += GameTime.DeltaTime;

        float t = Mathf.Clamp01(currentTime / endTime);

        transform.position = Bezior.BeziorMove(moveVec, t);
        RotateModel(t);

        if (currentTime >= 1f)
        {
            TargetPosHit();
        }
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

        if (_linoleum != null)
        {
            var spawnLinoleum = ResourceManager.Instance.Spawn(_linoleum).GetComponent<Linoleum>();
            spawnLinoleum.Initialize(_damage);
            spawnLinoleum.transform.position = transform.position;
        }

        ResourceManager.Instance.Destroy(gameObject);
    }

    protected override void TargetPosHit()
    {
        if (_linoleum != null)
        {
            var spawnLinoleum = ResourceManager.Instance.Spawn(_linoleum).GetComponent<Linoleum>();
            spawnLinoleum.Initialize(_damage);
            spawnLinoleum.transform.position = transform.position;
        }

        ResourceManager.Instance.Destroy(gameObject);
    }
}