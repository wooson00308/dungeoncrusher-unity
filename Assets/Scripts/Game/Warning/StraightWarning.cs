using UnityEngine;

public class StraightWarning : Warning
{
    public override void Initialize(Unit owner, Vector2 targetPos)
    {
        _owner = owner;
        float x = targetPos.x - owner.transform.position.x;
        transform.position =
            Vector3.Lerp(owner.transform.position, targetPos, 0.5f);
        transform.localScale = new Vector3(x, transform.lossyScale.y, transform.lossyScale.z);
        transform.rotation = Quaternion.Euler(Util.LookAt2D(owner.transform.position, targetPos));
    }
}