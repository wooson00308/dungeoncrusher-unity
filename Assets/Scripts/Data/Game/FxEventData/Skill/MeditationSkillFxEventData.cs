using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MeditationSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/MeditationSkillFxEventData")]
public class MeditationSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int apValue;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateAP("Ready", apValue);
    }
}