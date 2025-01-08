using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Data/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("이동속도")] [Tooltip("커브라면 도착하는 시간")] public float moveSpeed;
    [Tooltip("콜라이더 범위")] public float detectRange;
}