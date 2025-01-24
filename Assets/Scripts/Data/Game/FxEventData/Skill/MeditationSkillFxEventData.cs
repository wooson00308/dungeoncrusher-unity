using UnityEngine;

[CreateAssetMenu(fileName = "MeditationSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/MeditationSkillFxEventData")]
public class MeditationSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int apValue;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateAP("Ready", apValue);
    }
}