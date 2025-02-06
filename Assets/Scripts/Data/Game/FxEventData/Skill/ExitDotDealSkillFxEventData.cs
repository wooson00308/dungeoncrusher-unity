using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExitDotDealSkillFxEventData", menuName = "Data/FxEventData/ExitDotDealSkillFxEventData")]
public class ExitDotDealSkillFxEventData : FxEventData
{
    public override void OnEventToTarget(Unit owner, Unit target)
    {
        Awaitable.WaitForSecondsAsync(2f);
        foreach (var list in CursedStepSkillFxEventData.poisonUnit)
        {
            if(list == target)
            {
                CursedStepSkillFxEventData.poisonUnit.Remove(list);
            }
        }
    }
}

