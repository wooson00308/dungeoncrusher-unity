using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDB", menuName = "Data/DB/ProjectileDB")]
public class ProjectileDB_old : ScriptableObject
{
    public ProjectileInfo projectileInfo;

    public List<ProjectileData_old> ProjectileDatas = new();
    public Stack<ProjectileData_old> AddedProjectiles = new();
}