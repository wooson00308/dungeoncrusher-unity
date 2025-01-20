using UnityEngine;

[CreateAssetMenu(fileName = "MeditationSkillConditionData",
    menuName = "Scriptable Objects/Skill/Condition/MeditationSkillConditionData")]
public class MeditationSkillConditionData : SkillConditionData //메디테이션 - 전투 시작 후 매 1초마다 마력 증가 버프 획득
{
    public UnitEvents condition;
    [SerializeField] private int everySeconds;

    public override int EventId
    {
        get { return (int)condition; }
    }

    public override bool IsSatisfied(Skill skill, object gameEvent)
    {
        if (!base.IsSatisfied(skill, gameEvent)) return false;

        if (gameEvent is not UnitEventWithAttackerArgs args) return false;
        if (!skill.Owner.EqualsUnit(args.attacker)) return false;

        var owner = skill.Owner;
        if (owner == null) return false;
        if (!owner.IsActive) return false;

        IsSeconds();

        return true;
    }

    private async void IsSeconds()
    {
        await Awaitable.WaitForSecondsAsync(everySeconds);
    }
}