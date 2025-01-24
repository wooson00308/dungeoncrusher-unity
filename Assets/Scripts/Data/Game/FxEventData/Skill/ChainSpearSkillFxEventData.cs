using UnityEngine;

[CreateAssetMenu(fileName = "ChainSpearSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/ChainSpearSkillFxEventData")]
public class ChainSpearSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int percentValue;
    [SerializeField] private int maxOverlapCount = 100;
    [SerializeField] private int criticalPercent;
    private int _currentOverlapCount;

    private void OnEnable()
    {
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Engage, ResetOverlapCount);
        }
    }

    private void ResetOverlapCount(object gameEvent)
    {
        _currentOverlapCount = 0;
    }

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        if (_currentOverlapCount >= maxOverlapCount)
        {
            owner.CriticalPercent.Update("Ready", criticalPercent);
            return;
        }

        _currentOverlapCount++;

        owner.UpdateCriticalRate("Ready", percentValue);
    }
}