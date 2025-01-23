using UnityEngine;

[CreateAssetMenu(fileName = "LifeStealSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/LifeStealSkillFxEventData")]
public class LifeStealSkillFxEventData : SkillFxEventData
{
    private int totalDamage;

    private void TotalDamage(object gameEvent)
    {
        if (gameEvent is OnHitEventArgs)
        {
            OnHitEventArgs onHitEventArgs = (OnHitEventArgs)gameEvent;
            totalDamage = onHitEventArgs.damageValue;
        }
    }

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnHit, TotalDamage);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnHit, TotalDamage);
        owner.TryLifeSteal(totalDamage * (int)(skill.CurrentLevelData.SkillValue * 0.01f));
    }
}