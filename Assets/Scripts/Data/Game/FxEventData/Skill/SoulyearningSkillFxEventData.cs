using UnityEngine;

[CreateAssetMenu(fileName = "SoulyearningSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SoulyearningSkillFxEventData")]
public class SoulyearningSkillFxEventData : SkillFxEventData
{
    public int value;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateMp($"{skill.Data.Id}", value);
    }
}