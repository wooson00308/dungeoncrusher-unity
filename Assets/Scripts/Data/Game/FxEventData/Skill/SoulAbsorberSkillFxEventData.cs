using UnityEngine;

[CreateAssetMenu(fileName = "SoulAbsorberSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SoulAbsorberSkillFxEventData")]
public class SoulAbsorberSkillFxEventData : SkillFxEventData
{
    public int healthValue;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateMaxHealth(healthValue);
    }
}