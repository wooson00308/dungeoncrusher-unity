using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UnitInfoUI : BaseView
{
    [SerializeField] private string _unitId;
    [SerializeField] private Sprite _defaultSkillSprite;
    private Unit _unit;

    public enum Images
    {
        Unit_Bar_Hp,
        Unit_Bar_Mp,
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
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Engage, Initialized);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_SetActive, Initialized);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnHit, UpdateHpUI);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_Mana_Regen, UpdateMpUI);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Engage, Initialized);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_SetActive, Initialized);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnHit, UpdateHpUI);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_Mana_Regen, UpdateMpUI);
    }

    public override void BindUI()
    {
        Bind<Image>(typeof(Images));
        Bind<RectTransform>(typeof(RectTransforms));
    }

    private async void Initialized(object gameEvent)
    {
        if (_unit == null)
        {
            _unit = UnitFactory.Instance.GetPlayer();

            while (_unit == null)
            {
                await Awaitable.EndOfFrameAsync();
            }
        }

        ShowHpUI();
        ShowMpUI();
        ShowItemUI();
        ShowSkillUI();
    }

    #region HpUI

    private int _maxHealth = 0;

    private void ShowHpUI()
    {
        if (_unitId == null || String.IsNullOrEmpty(_unitId))
        {
            Debug.LogWarning("unitId가 안들어가 있어요");
        }

        if (_unitId != _unit.Id) return;

        if (_maxHealth <= _unit.Health.Value)
        {
            _maxHealth = _unit.Health.Value;
        }
    }

    private void UpdateHpUI(object gameEvent)
    {
        if (_unit.Team == Team.Enemy) return;

        var fillAmount = (float)_unit.Health.Value / _maxHealth;
        Get<Image>((int)Images.Unit_Bar_Hp).rectTransform.localScale = new Vector2(fillAmount, 1);
    }

    #endregion

    #region MpUI

    private float _maxMp = 0;

    private void ShowMpUI()
    {
        if (_unitId == null || String.IsNullOrEmpty(_unitId))
        {
            Debug.LogWarning("unitId가 안들어가 있어요");
        }

        if (_unitId != _unit.Id) return;

        if (_maxMp <= _unit.Mp.Max)
        {
            _maxMp = _unit.Mp.Max;
        }
    }

    private void UpdateMpUI(object gameEvent)
    {
        if (_unitId != _unit.Id) return;
        if (_unit.Team == Team.Enemy) return;

        var fillAmount = _unit.Mp.Value / _maxMp;
        Get<Image>((int)Images.Unit_Bar_Mp).rectTransform.localScale = new Vector2(fillAmount, 1);
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

        var groupListSkill = Get<RectTransform>((int)RectTransforms.Group_List_Skill);
        List<Image> images = new List<Image>();

        for (int i = 0; i < groupListSkill.childCount; i++)
        {
            images.Add(groupListSkill.GetChild(i).GetComponent<Image>());
        }

        foreach (Image image in images)
        {
            image.sprite = _defaultSkillSprite;
            var skillLevelBackground = image.transform.GetChild(0).GetComponent<Image>();
            skillLevelBackground.gameObject.SetActive(false);
        }

        //foreach (var skills in _unit.SkillDic.Keys)
        //{
        //    for (int i = 0; i < _unit.SkillDic.Count; i++)
        //    {
        //        var skillDataIcon = _unit.SkillDic[skills].SkillData.Icon;
        //        if (skillDataIcon != null)
        //        {
        //            images[i].sprite = skillDataIcon;
        //        }
        //    }
        //}

        int index = 0;

        foreach (var key in _unit.SkillDic.Keys)
        {
            var data = _unit.SkillDic[key].SkillData;
            var image = images[index++];

            image.sprite = data.Icon;

            var skillLevelBackground = image.transform.GetChild(0).GetComponent<Image>();
            skillLevelBackground.gameObject.SetActive(true);

            var skillLevelTxt = skillLevelBackground.GetComponentInChildren<TextMeshProUGUI>();
            skillLevelTxt.SetText($"{data.Level}");
        }
    }

    #endregion
}