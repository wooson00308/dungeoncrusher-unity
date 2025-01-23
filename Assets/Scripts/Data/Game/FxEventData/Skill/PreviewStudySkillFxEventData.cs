using UnityEngine;

[CreateAssetMenu(fileName = "PreviewStudySkillFxEventData",
    menuName = "Data/SkillData/FxEventData/PreviewStudySkillFxEventData")]
public class PreviewStudySkillFxEventData : SkillFxEventData
{
    private int level = 1;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        Debug.Log("Apply");
        owner.ExpPercent.Reset($"{skill.Data.Id}");
        owner.UpdateExpPercent($"{skill.Data.Id}", (int)skill.CurrentLevelData.SkillValue);
    }
}