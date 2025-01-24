using UnityEngine;

[CreateAssetMenu(fileName = "ImmediateExecutionSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/ImmediateExecutionSkillFxEventData")]
public class ImmediateExecutionSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        Debug.Log("처형");
        owner.Target?.OnDeath(owner, isExecution: true);
    }
}