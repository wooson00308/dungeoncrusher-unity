using UnityEngine;

[CreateAssetMenu(fileName = "CainSpearSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/CainSpearSkillFxEventData")]
public class CainSpearSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int percentValue;
    [SerializeField] private int overlapCount = 100;
    private int _currentOverlapCount;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        if (_currentOverlapCount >= overlapCount) return;
        _currentOverlapCount++;
        
        owner.UpdateCriticalRate("Engage", percentValue);
    }
}