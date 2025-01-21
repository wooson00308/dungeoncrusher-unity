using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Data/ProjectileData")]
public class ProjectileData_old : ScriptableObject
{
    public ProjectileInfo projectileInfo = new();
}

[Serializable]
public class ProjectileInfo
{
    [Header("이동속도")] [Tooltip("커브라면 도착하는 시간")]
    public float moveSpeed;

    [Tooltip("콜라이더 범위")] public float detectRange;

    public bool isStun;
    public bool isAerial;
}