using UnityEngine;

[CreateAssetMenu(fileName = "ImmediateExecutionSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/ImmediateExecutionSkillFxEventData")]
public class ImmediateExecutionSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        if (owner.Target.IsBoss) return;
        owner.Target?.OnDeath(owner, isExecution: true);
    }
}