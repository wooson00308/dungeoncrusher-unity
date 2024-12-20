using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Data/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Tooltip("이동속도")]public float moveSpeed;
    [Tooltip("콜라이더 범위")]public float detectRange;
}