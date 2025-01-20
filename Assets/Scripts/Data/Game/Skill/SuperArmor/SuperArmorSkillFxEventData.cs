using UnityEngine;

[CreateAssetMenu(fileName = "SuperArmorSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SuperArmorSkillFxEventData")]
public class SuperArmorSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.SetSuperArmor(skill.CurrentLevelData.Duration);
    }
}