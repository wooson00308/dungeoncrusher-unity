using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UnitInfoUI : BaseView
{
    [SerializeField] private string _unitId;
    private Unit _unit;

    public enum Scrollbars
    {
        Unit_Bar_Hp,
        Unit_Bar_Mp
    }

    public enum Images
    {
        Img_UnitProfile,
        Unit_Equip_Weapon,
        Unit_Equip_Head,
        Unit_Equip_Arm,
    }

    public enum RectTransforms
    {
        Group_List_Skill
    }

    private void Awake()
    {
        BindUI();
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_SetActive.ToString(), Initialized);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnHit.ToString(), UpdateHpUI);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_SetActive.ToString(), Initialized);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnHit.ToString(), UpdateHpUI);
    }

    public override void BindUI()
    {
        Bind<Scrollbar>(typeof(Scrollbars));
        Bind<Image>(typeof(Images));
        Bind<RectTransform>(typeof(RectTransforms));
    }

    #region HpUI

    private int _maxHealth = 0;

    private void Initialized(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;

        if (unitEventArgs.publisher.Team == Team.Enemy) return;

        _unit = unitEventArgs.publisher;

        ShowHpUI();
        ShowItemUI();
        ShowSkillUI();
    }

    private void ShowHpUI()
    {
        if (_unitId == null || String.IsNullOrEmpty(_unitId))
        {
            Debug.LogWarning("unitId가 안들어가 있어요");
        }

        if (_unitId != _unit.Id) return;

        if (_maxHealth <= _unit.Health)
        {
            _maxHealth = _unit.Health;
        }
    }

    private void UpdateHpUI(GameEvent gameEvent)
    {
        if (_unitId != _unit.Id) return;
        if (_unit.Team == Team.Enemy) return;

        var unitHealth = (float)_unit.Health / _maxHealth;
        Get<Scrollbar>((int)Scrollbars.Unit_Bar_Hp).size = unitHealth;
    }

    #endregion

    #region ItemUI

    private void ShowItemUI()
    {
        if (_unitId != _unit.Id) return;

        _unit.Equipment.TryGetValue(PartType.Weapon, out Item weaponItem);
        if (weaponItem != null)
        {
            Get<Image>((int)Images.Unit_Equip_Weapon).sprite = weaponItem.Data.Icon;
        }

        _unit.Equipment.TryGetValue(PartType.Helmet, out Item helmetItem);
        if (helmetItem != null)
        {
            Get<Image>((int)Images.Unit_Equip_Head).sprite = helmetItem.Data.Icon;
        }

        _unit.Equipment.TryGetValue(PartType.Armor, out Item armorItem);
        if (armorItem != null)
        {
            Get<Image>((int)Images.Unit_Equip_Arm).sprite = armorItem.Data.Icon;
        }
    }

    #endregion

    #region SkillUI

    private void ShowSkillUI()
    {
        if (_unitId != _unit.Id) return;

        Image[] images = Get<RectTransform>((int)RectTransforms.Group_List_Skill).GetComponentsInChildren<Image>();

        foreach (var skills in _unit.SkillDic.Keys)
        {
            for (int i = 0; i < _unit.SkillDic.Count; i++)
            {
                var skillDataIcon = _unit.SkillDic[skills].SkillData.Icon;
                if (skillDataIcon != null)
                {
                    images[i].sprite = skillDataIcon;
                }
            }
        }
    }

    #endregion
}