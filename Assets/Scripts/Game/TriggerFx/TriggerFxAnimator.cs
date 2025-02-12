using UnityEngine;

public class TriggerFxAnimator : MonoBehaviour
{
    public Animator Animator { get; private set; }

    public void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if (Animator == null) return; //캐싱되어 있으니 == 가 아닌 is null로 비교
        Animator.speed = GameTime.TimeScale;
    }

    public void OnDestroyed(AnimationEvent e)
    {
        if (e.stringParameter == "Pool")
        {
            ResourceManager.Instance.Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}