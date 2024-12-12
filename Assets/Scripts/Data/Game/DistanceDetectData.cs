using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistanceDetectData", menuName = "Data/Unit/Detect/Create DistanceDetectData")]
public class DistanceDetectData : DetectDataBase
{
    [SerializeField]
    private bool _isClosestDistance = true; // true: ���� ����� �Ÿ�, false: ���� �� �Ÿ�

    public override Unit Detect(Unit user, HashSet<Unit> enemies, Unit currentTarget = null)
    {
        // Ÿ�� ���� �ɼ��� Ȱ��ȭ�Ǿ� �ְ�, ���� Ÿ���� �����ϸ�, ����ִ� ���
        if (_isLockOnTargetUntilDeath && currentTarget != null && !currentTarget.IsDeath)
        {
            return currentTarget;
        }

        Unit bestTarget = null;
        float bestDistance = _isClosestDistance ? float.MaxValue : float.MinValue;

        foreach (Unit unit in enemies)
        {
            // ������ �׾��ų� �ڽ��� ��� ��ŵ
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
