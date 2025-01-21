using UnityEngine;

[CreateAssetMenu(fileName = "SoulyearningSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SoulyearningSkillFxEventData")]
public class SoulyearningSkillFxEventData : SkillFxEventData
{
    public float value;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        Debug.Log("OnSkill");
        owner.MpPercent.Reset($"{skill.Data.Id}");
        owner.UpdateMpPercent($"{skill.Data.Id}", value);
    }
}