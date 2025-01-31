using UnityEngine;

[CreateAssetMenu(fileName = "AerialFxEventData", menuName = "Data/FxEventData/AerialFxEventData")]
public class AerialFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.Target?.OnAerial();
    }
}