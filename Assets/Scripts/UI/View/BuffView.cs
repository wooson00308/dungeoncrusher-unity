using System.Collections.Generic;
using UnityEngine;

public class BuffView : BaseView
{
    Dictionary<SkillData, BuffSloatView> skillDatas = new();

    private void Awake()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_UseSkillBuff, BuffSloat);
    }

    private void BuffSloat(object gameEvents)
    {
        if (gameEvents is BuffSkillEventArgs buffSkillEventArgs)
        {
            if (!skillDatas.ContainsKey(buffSkillEventArgs.data))
            {
                var buffSloatObject = ResourceManager.Instance.SpawnFromPath("UI/BuffSloat", transform);
                var buffSloat = buffSloatObject.GetComponent<BuffSloatView>();
                skillDatas.Add(buffSkillEventArgs.data, buffSloat);
            }
            skillDatas[buffSkillEventArgs.data].UpdateUI(buffSkillEventArgs.data.Icon, buffSkillEventArgs.currentCount);
        }
    }

    public override void BindUI()
    {
    }
}