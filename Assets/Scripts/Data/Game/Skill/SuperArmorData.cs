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
        float skillValue = GetSkillLevelData(skill.Level).skillValue;
        user.SetSuperArmor(skillValue); //1�� �׸�

        //user.IsSuperArmor = true;
        //Debug.Log(user.IsSuperArmor);
        //TimingManager.Instance.SetTimer(skillValue, () => { user.IsSuperArmor = false; Debug.Log(user.IsSuperArmor); }); //2�� �׸�
    }
}
