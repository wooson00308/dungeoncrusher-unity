using UnityEngine;

[CreateAssetMenu(fileName = "LightingSpearSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/LightingSpearSkillFxEventData")]
public class LightingSpearSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.Target.OnHit(owner.Attack.Value /*마력으로*/, owner);
    }
}