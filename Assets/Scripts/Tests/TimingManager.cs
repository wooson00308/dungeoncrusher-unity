using System;
using System.Collections;
using UnityEngine;

public class TimingManager : SingletonMini<TimingManager>
{
    public void SetTimer(float duration, Action onComplete)
    {
        StartCoroutine(TimerCoroutine(duration, onComplete));
    }
    
    IEnumerator TimerCoroutine(float duration, Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }
}
