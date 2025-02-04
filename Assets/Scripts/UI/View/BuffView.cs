using System.Collections.Generic;

public class BuffView : BaseView
{
    private Dictionary<SkillData, BuffSloatView> _skillDatas = new();

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_UseSkillBuff, BuffSloat);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_UseSkillBuff, BuffSloat);
        DestroyBuffSloat();
    }

    private void BuffSloat(object gameEvents)
    {
        if (gameEvents is BuffSkillEventArgs buffSkillEventArgs)
        {
            if (!_skillDatas.ContainsKey(buffSkillEventArgs.data))
            {
                var buffSloatObject = ResourceManager.Instance.SpawnFromPath("UI/BuffSloat", transform);
                var buffSloat = buffSloatObject.GetComponent<BuffSloatView>();
                _skillDatas.Add(buffSkillEventArgs.data, buffSloat);
            }

            _skillDatas[buffSkillEventArgs.data]
                .UpdateUI(buffSkillEventArgs.data.Icon, buffSkillEventArgs.currentCount);
        }
    }

    private void DestroyBuffSloat()
    {
        foreach (var buffSloatView in _skillDatas.Values)
        {
            Destroy(buffSloatView.gameObject);
        }

        _skillDatas.Clear();
    }


    public override void BindUI()
    {
    }
}