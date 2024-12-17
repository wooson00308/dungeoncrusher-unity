using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : Singleton<CameraShake>
{
    private CinemachineBasicMultiChannelPerlin shakeChannel;

    private void Awake()
    {
        shakeChannel = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public async void Shake(float shakePower, float shakeTime)
    {
        shakeChannel.AmplitudeGain = shakePower;
        await Awaitable.WaitForSecondsAsync(shakeTime);
        shakeChannel.AmplitudeGain = 0;
    }
}