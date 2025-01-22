using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MpPotionUseUltiSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/MpPotionUseUltiSkillFxEventData")]
public class MpPotionUseUltiSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int useUltiMp;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        Debug.Log("UltiSkill");
        owner.UpdateSkillMp(useUltiMp);
    }
}