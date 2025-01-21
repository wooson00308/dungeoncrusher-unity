using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class FxEventAnimator : MonoBehaviour
{
    public Animator Animator { get; private set; }

    private FxEventData _data;
    private Unit _owner;
    private Skill _skill;

    public bool isGameTime = true;

    public void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Update()
    {
        if(isGameTime)
        {
            Animator.speed = GameTime.TimeScale;
        }
    }

    public void Initialized(FxEventData data, Unit owner, Skill skill)
    {
        _data = data;
        _owner = owner;
        _skill = skill;

        isGameTime = data.IsGameTime;
    }

    public void OnAction(AnimationEvent e)
    {
        _data.OnEvent(_owner, _skill);
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