using UnityEngine;

public class Visualizer : Singleton<Visualizer>
{
    public GameObject rangeIndicatorPrefab; // 범위를 표시할 프리팹 (원의 Sprite 포함)
    private GameObject currentRangeIndicator;

    /// <summary>
    /// 스킬 범위를 시각적으로 표시합니다.
    /// </summary>
    /// <param name="center">원의 중심 좌표</param>
    /// <param name="range">스킬 범위</param>
    public void ShowRange(Vector3 center, float range)
    {
        if (currentRangeIndicator == null)
        {
            currentRangeIndicator = Instantiate(rangeIndicatorPrefab);
        }

        currentRangeIndicator.transform.position = center;

        // SpriteRenderer를 사용하여 크기 조정
        float diameter = range * 2; // 원의 지름 계산
        currentRangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1);
        currentRangeIndicator.SetActive(true);
    }

    /// <summary>
    /// 범위 시각화를 숨깁니다.
    /// </summary>
    public void HideRange()
    {
        if (currentRangeIndicator != null)
        {
            currentRangeIndicator.SetActive(false);
        }
    }
}
