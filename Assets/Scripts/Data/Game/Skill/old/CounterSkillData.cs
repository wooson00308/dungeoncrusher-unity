using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Data_Skill_1115", menuName = "SkillData/Create CounterSkill")]
public class CounterSkillData : SkillData_old
{
    public override bool IsValidTarget(Unit unit)
    {
        return true;
    }

    public override void OnAction(Skill_old skill, Unit user, List<Unit> targets)
    {
        float skillValue = GetSkillLevelData(skill.Level).skillValue;
        int damage = (int)(user.Attack.Value * skillValue * 0.01f);
        Debug.Log("Counter");
        foreach (var target in targets)
        {
            target?.OnHit(damage, user);
            if (skill.Level > 1)
            {
                if (!target.IsStun)
                {
                    if (Random.Range(1, 100)<=20)
                        target.OnStun();
                }
            }
            if (skill.Level > 2)
            {
                if (target.IsStun)
                {
                    target?.OnDeath(user, true);
                }
            }
        }
    }
}