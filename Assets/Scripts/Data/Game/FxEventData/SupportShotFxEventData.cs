using UnityEngine;

[CreateAssetMenu(fileName = "SupportShotFxEventData", menuName = "Data/FxEventData/SupportShotFxEventData")]
public class SupportShotFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var target = owner.Target;
        var damage = owner.Attack.Value * skill.CurrentLevelData.ADRatio;

        if (target?.Health.Max * 0.3f > target?.Health.Value)
        {
            target?.OnDeath(owner, true);
            return;
        }

        if (damage >= target.Health.Value)
        {
            target?.OnDeath(owner, true);
            return;
        }

        target?.OnHit(damage, owner);
    }
}