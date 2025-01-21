using UnityEngine;

public class SkillCutAnimator : MonoBehaviour
{
    private void Awake()
    {
        //GetComponent<Animator>().speed = GameTime.DeltaTime;
    }

    private void Update()
    {
        GetComponent<Animator>().speed = GameTime.TimeScale;
    }

    public void StartCut(AnimationEvent e)
    {
        TimeManager.Instance.SlowMotion(true);
        CameraController.Instance.ZoomInOut(false);
    }

    public void EndCut(AnimationEvent e)
    {
        if(GameTime.TimeScale != 1)
        {
            TimeManager.Instance.SlowMotion(false);
            CameraController.Instance.ZoomInOut(true);
        }
        
        transform.parent.gameObject.SetActive(false);
    }
}
