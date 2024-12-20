using UnityEngine;

public enum UnitEvents
{
    None,
    SetActive,
    UseSkill,
    UseSkill_Ulti,
    UseSkill_Publish_UI,
    UseSkill_Publish_UI_Ulti,
    RootSkill,
    Health_Regen,
    Mana_Regen,
    OnAttack,
    OnAttack_Critical,
    OnHit,
    OnHit_Critical,
    OnStun,
    OnDeath,
    OnDeath_Special
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