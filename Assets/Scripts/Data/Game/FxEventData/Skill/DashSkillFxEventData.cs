using UnityEngine;

[CreateAssetMenu(fileName = "DashSkillFxEventData", menuName = "Scriptable Objects/DashSkillFxEventData")]
public class DashSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        // 대쉬 스킬 구현
    }
}
