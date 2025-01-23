using UnityEngine;

[CreateAssetMenu(fileName = "SoulAbsorberSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/SoulAbsorberSkillFxEventData")]
public class SoulAbsorberSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateMaxHealth(owner.Health.Max + (int)skill.CurrentLevelData.SkillValue);
    }
}