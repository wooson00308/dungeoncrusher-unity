using UnityEngine;

[CreateAssetMenu(fileName = "SoulyearningSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/SoulyearningSkillFxEventData")]
public class SoulyearningSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        Debug.Log("OnSkill");
        owner.MpPercent.Reset($"{skill.Data.Id}");
        owner.UpdateMpPercent($"{skill.Data.Id}", skill.CurrentLevelData.SkillValue);
    }
}