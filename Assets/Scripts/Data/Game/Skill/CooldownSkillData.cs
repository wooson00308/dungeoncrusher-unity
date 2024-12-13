
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skill_1111", menuName = "SkillData/Create CoolDownSkill")]
public class CooldownSkillData : SkillData
{
    public override bool IsValidTarget(Unit unit)
    {
        return true;
    }

    public override void OnAction(int level, Unit user, List<Unit> targets)
    {
        float skillValue = GetSkillLevelData(level).skillValue;
        int damage = (int)(user.Attack.Value * skillValue * 0.01f);

        foreach (var target in targets)
        {
            target?.OnHit(damage, user);
        }
    }
}