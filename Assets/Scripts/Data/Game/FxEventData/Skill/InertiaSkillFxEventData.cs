using UnityEngine;

[CreateAssetMenu(fileName = "InertiaSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/InertiaSkillFxEventData")]
public class InertiaSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int damageBuffValue;
    [SerializeField] private int damageValue;
    [SerializeField] private int maxOverlapCount;
    private int _currentOverlapCount = 0;

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
            owner.UpdateAttack("Ready", damageValue);
            return;
        }

        _currentOverlapCount++;
        owner.UpdateAttack("Ready", damageBuffValue);
    }
}