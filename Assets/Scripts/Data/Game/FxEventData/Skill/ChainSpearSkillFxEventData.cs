using UnityEngine;

[CreateAssetMenu(fileName = "ChainSpearSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/ChainSpearSkillFxEventData")]
public class ChainSpearSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int percentValue;
    [SerializeField] private int maxOverlapCount = 100;
    [SerializeField] private int criticalPercent;
    private int _currentOverlapCount;
    private bool isSubscribe = false;
    private bool isCriticalPercentUpgrade = false;

    public void Initialize()
    {
        if (Application.isPlaying)
        {
            _currentOverlapCount = 0;
            isCriticalPercentUpgrade = false;
            isSubscribe = false;
            if (!isSubscribe)
            {
                GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Engage, ResetOverlapCount);
                isSubscribe = true;
            }
        }
    }

    private void ResetOverlapCount(object gameEvent)
    {
        _currentOverlapCount = 0;
        isCriticalPercentUpgrade = false;
    }

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        if (_currentOverlapCount >= maxOverlapCount)
        {
            if (isCriticalPercentUpgrade) return;
            isCriticalPercentUpgrade = true;
            owner.CriticalPercent.Update("Ready", criticalPercent);
            return;
        }

        _currentOverlapCount++;

        owner.UpdateCriticalRate("Ready", percentValue);
    }

    public void DisEvent()
    {
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Engage, ResetOverlapCount);
    }
}