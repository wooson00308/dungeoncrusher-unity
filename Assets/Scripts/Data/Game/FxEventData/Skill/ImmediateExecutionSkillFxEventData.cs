using UnityEngine;

[CreateAssetMenu(fileName = "ImmediateExecutionSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/ImmediateExecutionSkillFxEventData")]
public class ImmediateExecutionSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        Debug.Log("처형");
        owner.Target?.OnDeath(owner);
    }
}