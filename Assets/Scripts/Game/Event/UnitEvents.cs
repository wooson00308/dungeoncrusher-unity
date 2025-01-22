using UnityEngine;

public enum UnitEvents
{
    None = 200,
    UnitEvent_SetActive,
    UnitEvent_UseSkill,
    UnitEvent_UseSkill_Ulti,
    UnitEvent_UseSkill_Publish_UI,
    UnitEvent_UseSkill_Publish_UI_Ulti,
    UnitEvent_RootSkill,
    UnitEvent_Health_Regen,
    UnitEvent_Mana_Regen,
    UnitEvent_OnAttack,
    UnitEvent_OnHit,
    UnitEvent_OnStun,
    UnitEvent_Exp,
    UnitEvent_Level,
    UnitEvnet_LevelUpCount,
    UnitEvent_OnDestroy,
    UnitEvent_OnDeath,
    UnitEvent_OnDeath_Special,
    UnitEvent_OnDeath_Execution,
    UnitEvent_OnKill,
    UnitEvent_ChangeStat
}

public class UnitEventArgs
{
    public Unit publisher;
}

public class SetActiveEventArgs : UnitEventArgs
{
    public bool isActive;
}

public class OnHitEventArgs : UnitEventArgs
{
    public int damageValue;
    public bool isCiritical;
}

public class SkillEventArgs : UnitEventArgs
{
    public SkillData_old data_old;
    public SkillData data;
}

public class UnitEventWithAttackerArgs : UnitEventArgs
{
    public Unit attacker;
}

public class UnitEventOnKillArgs : UnitEventArgs
{
    public Unit target;
}