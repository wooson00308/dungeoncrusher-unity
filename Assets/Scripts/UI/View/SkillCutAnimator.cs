using UnityEngine;

public class SkillCutAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = GameTime.DeltaTime;
    }

    private void Update()
    {
        _animator.speed = GameTime.TimeScale;
    }

    public void StartCut(AnimationEvent e)
    {
        TimeManager.Instance.SlowMotion(true);
        CameraController.Instance.ZoomInOut(false);
    }

    public void EndCut(AnimationEvent e)
    {
        if (GameTime.TimeScale != 1)
        {
            TimeManager.Instance.SlowMotion(false);
            CameraController.Instance.ZoomInOut(true);
        }

        transform.parent.gameObject.SetActive(false);
    }
}