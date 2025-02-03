using UnityEngine;

[CreateAssetMenu(fileName = "DisdainWeekSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/DisdainWeekSkillFxEventData")]
public class DisdainWeekSkillFxEventData : SkillFxEventData
{
    // private Unit _target;
    //
    // private void SetTarget(object gameEvent)
    // {
    //     if (gameEvent is UnitEventOnAttackArgs attackArgs)
    //     {
    //         _target = attackArgs.target;
    //     }
    // }

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        // GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnAttack, SetTarget);
        // if (_target != null)
        // {
        // _target.OnHit((int)skill.CurrentLevelData.SkillValue);
        // }
        owner.Target?.OnHit((int)skill.CurrentLevelData.SkillValue);
    }
}