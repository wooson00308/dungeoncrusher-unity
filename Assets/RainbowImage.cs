using UnityEngine;
using UnityEngine.UI;

public class RainbowImage : MonoBehaviour
{
    private Image targetImage; // 색상을 변경할 Image 컴포넌트
    public float colorChangeSpeed = 1.0f; // 색상이 변화하는 속도
    private bool _enabled;

    private float time; // 시간 누적 변수

    public void OnClick()
    {
        _enabled = !_enabled;
        targetImage.enabled = _enabled;
    }

    void Start()
    {
        if (targetImage == null)
        {
            // Image 컴포넌트가 연결되지 않은 경우, 현재 오브젝트에서 자동으로 찾아봄
            targetImage = GetComponent<Image>();
        }

        if (targetImage == null)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!_enabled) return;

        // 시간 누적
        time += colorChangeSpeed * Time.deltaTime;

        // RGB 값을 각각 부드럽게 변경
        float r = Mathf.Sin(time) * 0.5f + 0.5f; // 0~1 범위
        float g = Mathf.Sin(time + Mathf.PI * 2 / 3) * 0.5f + 0.5f; // 120도 위상 차
        float b = Mathf.Sin(time + Mathf.PI * 4 / 3) * 0.5f + 0.5f; // 240도 위상 차

        // 새로운 색상을 설정
        targetImage.color = new Color(r, g, b);
    }
}
