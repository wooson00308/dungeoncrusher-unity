using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[CreateAssetMenu(fileName = "CursedStepDotDamageSkillEventData",
    menuName = "Data/SkillData/FxEventData/CursedStepDotDamageSkillEventData")]

public class CursedStepDotDamageSkillEventData : FxEventData
{
    public override void OnEventToTarget(Unit owner, Unit target)
    {
        DotDamage();
    }

    public async void DotDamage()
    {
        if (CursedStepSkillFxEventData.isProcessing) return;

        CursedStepSkillFxEventData.isProcessing = true;

        foreach (var list in CursedStepSkillFxEventData.poisonUnit.ToList())
        {
            if (!list.gameObject.activeSelf)
            {
                CursedStepSkillFxEventData.poisonUnit.Remove(list);
            }
            list?.OnHit(1);
        }

        await Awaitable.WaitForSecondsAsync(0.25f);
        CursedStepSkillFxEventData.isProcessing = false;
    }
}