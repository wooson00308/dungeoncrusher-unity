using System.Collections;
using UnityEngine;

public class ReadyText : MonoBehaviour
{
    public float disableTime = 2; //임시로 시간으로 정해둔거지 나중엔 서버까지 연동하면 서버 대기하는 걸로 수정

    private void OnEnable()
    {
        StartCoroutine(DisableCor());
    }

    private IEnumerator DisableCor()
    {
        yield return new WaitForSecondsRealtime(disableTime);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_Engage);
    } 
}