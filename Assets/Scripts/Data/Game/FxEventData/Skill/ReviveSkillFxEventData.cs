using UnityEngine;

[CreateAssetMenu(fileName = "ReviveSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/ReviveSkillFxEventData")]
public class ReviveSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.IsRevive = true;
    }
}