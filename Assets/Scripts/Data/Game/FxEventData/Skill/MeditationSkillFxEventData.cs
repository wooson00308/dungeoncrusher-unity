using UnityEngine;

[CreateAssetMenu(fileName = "MeditationSkillFxEventData", menuName = "Scriptable Objects/Skill/FxEvent/MeditationSkillFxEventData")]
public class MeditationSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int manaValue;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        //마력
        // owner.UpdateMana("Engage", manaValue);
    }
}