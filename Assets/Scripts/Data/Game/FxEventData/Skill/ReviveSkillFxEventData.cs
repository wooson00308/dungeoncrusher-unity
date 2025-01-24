using UnityEngine;

[CreateAssetMenu(fileName = "ReviveSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/ReviveSkillFxEventData")]
public class ReviveSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.IsRevive = true;                       
    }
}