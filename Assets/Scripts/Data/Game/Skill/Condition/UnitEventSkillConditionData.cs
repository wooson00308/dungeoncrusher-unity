using UnityEngine;

[CreateAssetMenu(fileName = "UnitEventSkillConditionData", menuName = "Data/UnitEventSkillConditionData")]
public class UnitEventSkillConditionData : UnitEventConditionData
{
    [Header("스킬이 같지않아도 상관없다")] public bool IsNotEqualsSkill = true;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;
        
        if (gameEvent is SkillEventArgs skillArgs)
        {
            if (!IsNotEqualsSkill && skillArgs.Data.Id != skill.Data.Id) return false;
        }

        return true;
    }
}