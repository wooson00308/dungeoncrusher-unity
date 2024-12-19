using UnityEngine;
using UnityEngine.UI;

public class RainbowImage : MonoBehaviour
{
    private Image targetImage; // ������ ������ Image ������Ʈ
    public float colorChangeSpeed = 1.0f; // ������ ��ȭ�ϴ� �ӵ�
    private bool _enabled;

    private float time; // �ð� ���� ����

    public void OnClick()
    {
        _enabled = !_enabled;
        targetImage.enabled = _enabled;
    }

    void Start()
    {
        if (targetImage == null)
        {
            // Image ������Ʈ�� ������� ���� ���, ���� ������Ʈ���� �ڵ����� ã�ƺ�
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

        // �ð� ����
        time += colorChangeSpeed * Time.deltaTime;

        // RGB ���� ���� �ε巴�� ����
        float r = Mathf.Sin(time) * 0.5f + 0.5f; // 0~1 ����
        float g = Mathf.Sin(time + Mathf.PI * 2 / 3) * 0.5f + 0.5f; // 120�� ���� ��
        float b = Mathf.Sin(time + Mathf.PI * 4 / 3) * 0.5f + 0.5f; // 240�� ���� ��

        // ���ο� ������ ����
        targetImage.color = new Color(r, g, b);
    }
}
