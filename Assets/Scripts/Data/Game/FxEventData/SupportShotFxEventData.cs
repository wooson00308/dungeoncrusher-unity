using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportShotFxEventData", menuName = "Data/FxEventData/SupportShotFxEventData")]
public class SupportShotFxEventData : FxEventData
{
    public override void OnEvent(Unit owner, object args = null)
    {
        Skill skill = args as Skill;
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
