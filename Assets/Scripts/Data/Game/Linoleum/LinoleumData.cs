using UnityEngine;

[CreateAssetMenu(fileName = "LinoleumData", menuName = "Data/LinoleumData")]
public class LinoleumData : ScriptableObject
{
    [Tooltip("틱당 데미지 퍼센트")] public float tickDamagePercent;
    [Tooltip("틱 간격")] public float tickInterval;
    [Tooltip("콜라이더 범위")] public float detectRange;
}