using UnityEngine;

public class SkillCutAnimator : MonoBehaviour
{
    public void StartCut(AnimationEvent e)
    {
        TimeManager.Instance.SlowMotion(true);
        CameraController.Instance.ZoomInOut(false);
    }

    public void EndCut(AnimationEvent e)
    {
        TimeManager.Instance.SlowMotion(false);
        CameraController.Instance.ZoomInOut(true);
        transform.parent.gameObject.SetActive(false);
    }
}
