using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MpPotionEngageSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/MpPotionEngageSkillFxEventData")]
public class MpPotionEngageSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int engageValue;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.UpdateSkillMp(engageValue);
    }
}