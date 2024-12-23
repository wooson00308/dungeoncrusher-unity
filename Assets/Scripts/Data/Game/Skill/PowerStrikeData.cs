using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skill_997", menuName = "SkillData/Create PowerStrike")]
public class PowerStrikeData : SkillData
{
    public override bool IsValidTarget(Unit unit)
    {
        return true;
    }

    /// <summary>
    /// 스킬 비즈니스 코드
    /// </summary>
    /// <param name="level"></param>
    /// <param name="user"></param>
    /// <param name="targets"></param>
    public override void OnAction(Skill skill, Unit user, List<Unit> targets)
    {
        float skillValue = GetSkillLevelData(skill.Level).skillValue;
        int damage = (int)(user.Attack.Value * skillValue * 0.01f);

        foreach (var target in targets)
        {
            target?.OnHit(damage, user);
        }
    }
}
