using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionSoulSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/ExplosionSoulSkillFxEventData")]
public class ExplosionSoulSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        //프리팹 소환
    }
}