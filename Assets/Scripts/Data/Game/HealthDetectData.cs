using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthDetectData", menuName = "Data/Unit/Detect/Create HealthDetectData")]
public class HealthDetectData : DetectDataBase
{
    [SerializeField]
    private bool _isLowestHealth = true; // true: 최소 체력, false: 최대 체력

    public override Unit Detect(Unit user, HashSet<Unit> enemies, Unit currentTarget = null)
    {
        // 타겟 고정 옵션이 활성화되어 있고, 현재 타겟이 존재하며, 살아있는 경우
        if (_isLockOnTargetUntilDeath && currentTarget != null && !currentTarget.IsDeath)
        {
            return currentTarget;
        }

        Unit bestTarget = null;
        float bestHealth = _isLowestHealth ? float.MaxValue : float.MinValue;

        foreach (Unit unit in enemies)
        {
            // 유닛이 죽었거나 자신인 경우 스킵
            if (unit.IsDeath || unit == user)
                continue;

            float unitHealth = unit.Health;

            bool isBetterTarget = _isLowestHealth ? unitHealth < bestHealth : unitHealth > bestHealth;

            if (isBetterTarget)
            {
                bestHealth = unitHealth;
                bestTarget = unit;
            }
        }

        return bestTarget;
    }
}
