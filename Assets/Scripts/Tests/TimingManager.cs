using System;
using UnityEngine;

public class TimingManager : SingletonMini<TimingManager>
{
    public async Awaitable SetTimer(float duration, Action onComplete)
    {
        Debug.Log("����");
        await Awaitable.WaitForSecondsAsync(duration);
        onComplete?.Invoke();
        Debug.Log("��ȯ");
    }
}