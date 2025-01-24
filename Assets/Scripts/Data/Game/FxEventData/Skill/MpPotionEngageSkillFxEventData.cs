using UnityEngine;

[CreateAssetMenu(fileName = "MpPotionEngageSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/MpPotionEngageSkillFxEventData")]
public class MpPotionEngageSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int engageValue;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateSkillMp(engageValue);
    }
}