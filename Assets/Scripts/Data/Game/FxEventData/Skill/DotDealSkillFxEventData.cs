using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DotDealSkillFxEventData", menuName = "Data/FxEventData/DotDealSkillFxEventData")]
public class DotDealSkillFxEventData : FxEventData
{
    public override void OnEventToTarget(Unit owner, Unit target)
    {
        if (!CursedStepSkillFxEventData.poisonUnit.Contains(target))
        {
            CursedStepSkillFxEventData.poisonUnit.Add(target);
        }
    }
}