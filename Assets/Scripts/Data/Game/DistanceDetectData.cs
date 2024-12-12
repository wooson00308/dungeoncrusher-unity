using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistanceDetectData", menuName = "Data/Unit/Detect/Create DistanceDetectData")]
public class DistanceDetectData : DetectDataBase
{
    [SerializeField]
    private bool _isClosestDistance = true; // true: 가장 가까운 거리, false: 가장 먼 거리

    public override Unit Detect(Unit user, HashSet<Unit> enemies, Unit currentTarget = null)
    {
        // 타겟 고정 옵션이 활성화되어 있고, 현재 타겟이 존재하며, 살아있는 경우
        if (_isLockOnTargetUntilDeath && currentTarget != null && !currentTarget.IsDeath)
        {
            return currentTarget;
        }

        Unit bestTarget = null;
        float bestDistance = _isClosestDistance ? float.MaxValue : float.MinValue;

        foreach (Unit unit in enemies)
        {
            // 유닛이 죽었거나 자신인 경우 스킵
            if (unit.IsDeath || unit == user)
                continue;

            float unitDistance = Vector3.Distance(user.transform.position, unit.transform.position);

            bool isBetterTarget = _isClosestDistance ? unitDistance < bestDistance : unitDistance > bestDistance;

            if (isBetterTarget)
            {
                bestDistance = unitDistance;
                bestTarget = unit;
            }
        }

        return bestTarget;
    }
}
