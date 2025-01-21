using UnityEngine;

public abstract class FxEventData : Data
{
    public bool IsGameTime = true;
    public FxEventAnimator Prefab;

    public virtual void OnEventToTarget(Unit owner, Unit target) { }
    public virtual void OnEvent(Unit owner, object args = null) { }
    public virtual void OnEndEvent(Unit owner, object args = null) { }
}
