using UnityEngine;

public class CurveWarning : Warning
{
    public override void Initialize(Unit owner, Vector2 targetPos)
    {
        _owner = owner;
        transform.position = targetPos;
    }
}