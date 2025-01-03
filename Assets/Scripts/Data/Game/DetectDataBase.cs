using System.Collections.Generic;
using UnityEngine;

public abstract class DetectDataBase : ScriptableObject
{
    public string _id;
    public bool _isLockOnTargetUntilDeath;

    public abstract Unit Detect(Unit user, HashSet<Unit> enemies, Unit currentTarget = null);
}
