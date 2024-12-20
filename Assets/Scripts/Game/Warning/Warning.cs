using System;
using UnityEngine;

public abstract class Warning : MonoBehaviour
{
    protected Unit _owner;

    protected void FixedUpdate()
    {
        if (_owner != null)
        {
            if (_owner.IsHit)
            {
                ResourceManager.Instance.Destroy(gameObject);
            }
        }
    }

    public abstract void Initialize(Unit owner, Vector2 targetPos);
}