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

        // Cinemachine Camera�� Noise Channel ��������
        virtualCamera = GetComponent<CinemachineCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineCamera�� ã�� �� �����ϴ�.");
            return;
        }

        shakeChannel = virtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        // �⺻ FOV ����
        defaultFOV = virtualCamera.Lens.FieldOfView;
    }

    public async void Shake(float shakePower, float shakeTime)
    {
        if (shakeChannel == null)
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin�� ã�� �� �����ϴ�.");
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
            Debug.LogError("CinemachineCamera�� ã�� �� �����ϴ�.");
            return;
        }

        // ���� �Ǵ� �ܾƿ� �� �ﰢ������ FOV ����
        virtualCamera.Lens.FieldOfView = zoomIn ? defaultFOV / 1.5f : defaultFOV;
    }
}
