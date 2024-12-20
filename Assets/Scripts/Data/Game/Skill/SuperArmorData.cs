using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skill_1114", menuName = "SkillData/Create SuperArmor")]
public class SuperArmorData : SkillData
{
    public override bool IsValidTarget(Unit unit)
    {
        return true;
    }

    public override void OnAction(Skill skill, Unit user, List<Unit> targets)
    {
        user.IsSuperArmor = true;
    }
}
