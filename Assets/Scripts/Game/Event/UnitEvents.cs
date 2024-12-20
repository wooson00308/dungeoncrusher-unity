using UnityEngine;

public enum UnitEvents
{
    None,
    UnitEvent_SetActive,
    UnitEvent_UseSkill,
    UnitEvent_UseSkill_Ulti,
    UnitEvent_UseSkill_Publish_UI,
    UnitEvent_UseSkill_Publish_UI_Ulti,
    UnitEvent_RootSkill,
    UnitEvent_Health_Regen,
    UnitEvent_Mana_Regen,
    UnitEvent_OnAttack,
    UnitEvent_OnAttack_Critical,
    UnitEvent_OnHit,
    UnitEvent_OnHit_Critical,
    UnitEvent_OnStun,
    UnitEvent_OnDeath,
    UnitEvent_OnDeath_Special
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
}

public class SkillEventArgs : UnitEventArgs
{
    public SkillData data;
}