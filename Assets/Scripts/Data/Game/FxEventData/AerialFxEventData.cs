using UnityEngine;

[CreateAssetMenu(fileName = "AerialFxEventData", menuName = "Scriptable Objects/AerialFxEventData")]
public class AerialFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.OnAerial();
    }
}
