using Unity.Cinemachine;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineBasicMultiChannelPerlin shakeChannel;
    private CinemachineCamera virtualCamera;
    private float defaultFOV;

    protected override void Awake()
    {
        base.Awake();

        // Cinemachine Camera와 Noise Channel 가져오기
        virtualCamera = GetComponent<CinemachineCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineCamera를 찾을 수 없습니다.");
            return;
        }

        shakeChannel = virtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        // 기본 FOV 저장
        defaultFOV = virtualCamera.Lens.FieldOfView;
    }

    public async void Shake(float shakePower, float shakeTime)
    {
        if (shakeChannel == null)
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin을 찾을 수 없습니다.");
            return;
        }

        shakeChannel.AmplitudeGain = shakePower;
        await Awaitable.WaitForSecondsAsync(shakeTime);
        shakeChannel.AmplitudeGain = 0;
    }

    public void ZoomInOut(bool zoomIn)
    {
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineCamera를 찾을 수 없습니다.");
            return;
        }

        // 줌인 또는 줌아웃 시 즉각적으로 FOV 변경
        virtualCamera.Lens.FieldOfView = zoomIn ? defaultFOV / 1.5f : defaultFOV;
    }
}
