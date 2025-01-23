using UnityEngine;

/// <summary>
/// �뽬 ��ų
/// ��Ÿ�� ������ ��ų ����
/// ���� ������ ��ų ��Ÿ�� �ʱ�ȭ
/// </summary>
[CreateAssetMenu(fileName = "DashSkillConditionData", menuName = "Data/SkillData/ConditionData/DashSkillConditionData")]
public class DashConditionData : UnitEventConditionData
{
    public override void TryEvent(Skill skill, object gameEvent)
    {
        if (!IsSatisfied(skill, gameEvent)) return;

        skill.ResetCooltime();
    }
}