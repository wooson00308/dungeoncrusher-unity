using UnityEngine;

[CreateAssetMenu(fileName = "PreviewStudySkillFxEventData",
    menuName = "Data/SkillData/FxEventData/PreviewStudySkillFxEventData")]
public class PreviewStudySkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.ExpPercent.Reset($"{skill.Data.Id}");
        owner.UpdateExpPercent($"{skill.Data.Id}", (int)skill.CurrentLevelData.SkillValue);
    }
}