using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ImmediateExecutionSkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/ImmediateExecutionSkillConditionData")]
public class ImmediateExecutionConditionData : UnitEventSkillConditionData //즉결처형 - 적 공격시 일정 확률로 처형
{
    // [SerializeField] private int executionRate;

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        if (!Operator.IsRate(skill.CurrentLevelData.SkillValue /*executionRate*/)) return false;
        return true;
    }
}