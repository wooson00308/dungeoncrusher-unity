using UnityEngine;

[CreateAssetMenu(fileName = "MpPotionUseUltiSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/MpPotionUseUltiSkillFxEventData")]
public class MpPotionUseUltiSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int useUltiMp;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateSkillMp(useUltiMp);
    }
}