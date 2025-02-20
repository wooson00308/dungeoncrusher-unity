using UnityEngine;

[CreateAssetMenu(fileName = "DisdainWeekSkillConditionData",
    menuName = "Data/SkillData/ConditionData/DisdainWeekSkillConditionData")]
public class DisdainWeekConditionData : UnitEventConditionData // 약자멸시 - 공격하는 적의 현재 체력이 해당 몬스터 최대 체력의 n% 이하일 경우 피해량이 증가
{
    [SerializeField] private int healthPercent;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        if (owner.Target.Health.Value > owner.Target.Health.Max / healthPercent) return false;
        
        return true;
    }
}