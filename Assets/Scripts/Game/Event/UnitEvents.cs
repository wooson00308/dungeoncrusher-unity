using UnityEngine;

public enum UnitEvents
{
    None,
    UnitEvent_SetActive,
    UnitEvent_UseSkill,
    UnitEvent_UseSkill_Ulti,
    UnitEvent_Attack,
    UnitEvent_AddMp,
    UnitEvent_Attack_Critical,
    UnitEvent_OnHit,
    UnitEvent_OnHit_Critical,
    UnitEvent_OnStun,
    UnitEvent_OnDeath,
    UnitEvent_OnSpecialDeath
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