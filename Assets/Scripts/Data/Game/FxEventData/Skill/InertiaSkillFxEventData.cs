using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InertiaSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/InertiaSkillFxEventData")]
public class InertiaSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int damageValue;
    [SerializeField] private int maxOverlapCount;
    private int _overlapCount = 0;

    private void Awake()
    {
        Debug.Log("Awake");
        _overlapCount = 0;
    }

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        if (_overlapCount >= maxOverlapCount) return;
        _overlapCount++;
        Debug.Log("관성");
        owner.UpdateAttack("Engage", damageValue);
    }
}