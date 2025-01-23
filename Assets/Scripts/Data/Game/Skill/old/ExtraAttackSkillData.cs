
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [CreateAssetMenu(fileName = "Data_Skill_1113", menuName = "SkillData/Create ExtraAttackSkill")]
public class ExtraAttackSkillData : SkillData_old
{
    public override bool IsValidTarget(Unit unit)
    {
        return true;
    }

    public override void OnAction(Skill_old skill, Unit user, List<Unit> targets)
    {
        float skillValue = GetSkillLevelData(skill.Level).skillValue;
        int damage = (int)(user.Attack.Value * skillValue * 0.01f);
        foreach (var target in targets)
        {
            if (target.Health.Max * 0.3f > target.Health.Value)
            {
                target?.OnDeath(user, true);
                // target?.OnHit(target.Health.Value, user);
                skill.ResetCooltime();
                return;
            }
            if (damage >= target.Health.Value)
            {
                target?.OnDeath(user, true);
                skill.ResetCooltime();
            }
            target?.OnHit(damage, user);
        }
    }
}