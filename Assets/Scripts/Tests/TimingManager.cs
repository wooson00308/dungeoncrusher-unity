using System;
using System.Collections;
using UnityEngine;

public class TimingManager : SingletonMini<TimingManager>
{
    public async Awaitable SetTimer(float duration, Action onComplete)
    {
        Debug.Log("진입");
        await Awaitable.WaitForSecondsAsync(duration);
        onComplete?.Invoke();
        Debug.Log("반환");
    }
    
}
