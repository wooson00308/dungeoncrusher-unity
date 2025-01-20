using UnityEngine;

[CreateAssetMenu(fileName = "SupportShotSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SupportShotSkillFxEventData")]
public class SupportShotSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var target = owner.Target;
        var damage = owner.Attack.Value * skill.CurrentLevelData.ADRatio;

        if (target.Health.Max * 0.3f > target.Health.Value)
        {
            target?.OnDeath(owner, true);
            // target?.OnHit(target.Health.Value, user);
            return;
        }

        if (damage >= target.Health.Value)
        {
            target?.OnDeath(owner, true);
        }

        target?.OnHit(damage, owner);
    }
}