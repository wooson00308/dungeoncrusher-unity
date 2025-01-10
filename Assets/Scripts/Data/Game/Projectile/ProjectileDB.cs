using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDB", menuName = "Data/DB/ProjectileDB")]
public class ProjectileDB : ScriptableObject
{
    public ProjectileInfo projectileInfo;

    public List<ProjectileData> ProjectileDatas = new();
    public Stack<ProjectileData> AddedProjectiles = new();
}