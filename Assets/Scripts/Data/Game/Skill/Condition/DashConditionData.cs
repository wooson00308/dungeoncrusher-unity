using UnityEngine;

/// <summary>
/// �뽬 ��ų
/// ��Ÿ�� ������ ��ų ����
/// ���� ������ ��ų ��Ÿ�� �ʱ�ȭ
/// </summary>
[CreateAssetMenu(fileName = "DashSkillConditionData", menuName = "Scriptable Objects/DashSkillConditionData")]
public class DashConditionData : UnitEventConditionData
{
    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        return true;
    }

    public override void TryEvent(Skill skill, object gameEvent)
    {
        if(!IsSatisfied(skill, gameEvent)) return;

        skill.ResetCooltime();
    }
}
