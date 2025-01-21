using UnityEngine;

[CreateAssetMenu(fileName = "PreviewStudySkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/PreviewStudySkillFxEventData")]
public class PreviewStudySkillFxEventData : SkillFxEventData
{
    [SerializeField] private int percentValue;
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        //유닛스탯에 경험치 관련 스탯 필요 
    }
}