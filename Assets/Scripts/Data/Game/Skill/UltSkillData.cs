
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skill_1112", menuName = "SkillData/Create UltTestData")]
public class UltSkillData : SkillData
{
    
    public override bool IsValidTarget(Unit unit)
    {
        return true;
    }

    public override void OnAction(Skill skill, Unit user, List<Unit> targets)
    {
        if (skill.SkillData.IsUltSkill && user.Mp.Value < user.Mp.Max)
            return;
        float skillValue = GetSkillLevelData(skill.Level).skillValue;
        int damage = (int)(user.Attack.Value * skillValue * 1f);
        TimeManager.Instance.SlowMotion();
        user.Mp.Update("Ult", -user.Mp.Value);
        foreach (var target in targets)
        {
            target?.OnHit(damage, user);
        }
    }
}