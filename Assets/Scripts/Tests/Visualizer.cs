using UnityEngine;

public class Visualizer : Singleton<Visualizer>
{
    public GameObject rangeIndicatorPrefab; // ������ ǥ���� ������ (���� Sprite ����)
    private GameObject _currentRangeIndicator;

    /// <summary>
    /// ��ų ������ �ð������� ǥ���մϴ�.
    /// </summary>
    /// <param name="center">���� �߽� ��ǥ</param>
    /// <param name="range">��ų ����</param>
    public void ShowRange(Vector3 center, float range)
    {
        if (_currentRangeIndicator == null)
        {
            _currentRangeIndicator = Instantiate(rangeIndicatorPrefab);
        }

        _currentRangeIndicator.transform.position = center;

        // SpriteRenderer�� ����Ͽ� ũ�� ����
        float diameter = range * 2; // ���� ���� ���
        _currentRangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1);
        _currentRangeIndicator.SetActive(true);
    }

    /// <summary>
    /// ���� �ð�ȭ�� ����ϴ�.
    /// </summary>
    public void HideRange()
    {
        if (_currentRangeIndicator != null)
        {
            _currentRangeIndicator.SetActive(false);
        }
    }
}
