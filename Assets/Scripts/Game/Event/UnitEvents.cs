using UnityEngine;

public enum UnitEvents
{
    UnitEvent_SetActive,
    UnitEvent_UseSkill,
    UnitEvent_UseSkill_Ulti,
    UnitEvent_Attack,
    UnitEvent_AddMp,
    UnitEvent_Attack_Critical,
    UnitEvent_OnHit,
    UnitEvent_OnHit_Critical,
    UnitEvent_OnDeath
}

public class UnitEventArgs
{
    public Unit publisher;
}

public class OnHitEventArgs : UnitEventArgs
{
    public int damageValue;
}