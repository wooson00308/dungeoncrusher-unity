using UnityEngine;

public class Visualizer : Singleton<Visualizer>
{
    public GameObject rangeIndicatorPrefab; // ������ ǥ���� ������ (���� Sprite ����)
    private GameObject currentRangeIndicator;

    /// <summary>
    /// ��ų ������ �ð������� ǥ���մϴ�.
    /// </summary>
    /// <param name="center">���� �߽� ��ǥ</param>
    /// <param name="range">��ų ����</param>
    public void ShowRange(Vector3 center, float range)
    {
        if (currentRangeIndicator == null)
        {
            currentRangeIndicator = Instantiate(rangeIndicatorPrefab);
        }

        currentRangeIndicator.transform.position = center;

        // SpriteRenderer�� ����Ͽ� ũ�� ����
        float diameter = range * 2; // ���� ���� ���
        currentRangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1);
        currentRangeIndicator.SetActive(true);
    }

    /// <summary>
    /// ���� �ð�ȭ�� ����ϴ�.
    /// </summary>
    public void HideRange()
    {
        if (currentRangeIndicator != null)
        {
            currentRangeIndicator.SetActive(false);
        }
    }
}
