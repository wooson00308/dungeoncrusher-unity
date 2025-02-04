using UnityEngine;

[CreateAssetMenu(fileName = "InertiaSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/InertiaSkillFxEventData")]
public class InertiaSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int damageBuffValue;
    [SerializeField] private int damageValue;
    [SerializeField] private int maxOverlapCount;
    private int _currentOverlapCount = 0;
    private bool isSubscribe = false;
    private bool isDamageUpgrade = false;

    public void Initialize()
    {
        if (Application.isPlaying)
        {
            _currentOverlapCount = 0;
            isDamageUpgrade = false;
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
        isDamageUpgrade = false;
    }

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        if (_currentOverlapCount >= maxOverlapCount)
        {
            if (isDamageUpgrade) return;
            isDamageUpgrade = true;
            owner.UpdateAttack("Ready", damageValue);
            return;
        }

        _currentOverlapCount++;

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_UseSkillBuff, new BuffSkillEventArgs()
        {
            data = skill.Data,
            currentCount = _currentOverlapCount
        });

        owner.UpdateAttack("Ready", damageBuffValue);
    }

    public void DisEvent()
    {
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Engage, ResetOverlapCount);
    }
}