using UnityEngine;

/// <summary>
/// 대쉬 스킬
/// 쿨타임 지나면 스킬 실행
/// 적이 죽으면 스킬 쿨타임 초기화
/// </summary>
[CreateAssetMenu(fileName = "DashSkillConditionData", menuName = "Scriptable Objects/DashSkillConditionData")]
public class DashSkillConditionData : UnitEventSkillConditionData
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
