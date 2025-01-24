using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoView : BaseView
{
    private Image[] levelUpImages;

    public enum Sliders
    {
        Group_Exp
    }

    public enum Texts
    {
        Txt_Level,
        Txt_Exp,
        Txt_Count_Kill
    }

    public enum RectTransforms
    {
        Group_List_Leveluppoint
    }

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_Exp, UpdateExpUI);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_Level, UpdateLevelUI);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath, UpdateKillCountUI);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath_Special, UpdateKillCountUI);
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Engage, UpdateLevelUppoint);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvnet_LevelUpCount, UpdateLevelUppoint);
        StartInfo();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_Exp, UpdateExpUI);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_Level, UpdateLevelUI);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath, UpdateKillCountUI);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath_Special, UpdateKillCountUI);
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Engage, UpdateLevelUppoint);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvnet_LevelUpCount, UpdateLevelUppoint);
    }

    public override void BindUI()
    {
        Bind<Slider>(typeof(Sliders));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<RectTransform>(typeof(RectTransforms));

        levelUpImages = Get<RectTransform>((int)RectTransforms.Group_List_Leveluppoint)
            .GetComponentsInChildren<Image>();
        StartInfo();
    }

    private void StartInfo()
    {
        Get<TextMeshProUGUI>((int)Texts.Txt_Level).SetText($"LV.{1}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Exp).SetText($"{0}%");
        killCount = 0;
        Get<TextMeshProUGUI>((int)Texts.Txt_Count_Kill).SetText($"{0}");
        Get<Slider>((int)Sliders.Group_Exp).value = 0;

        UpdateLevelUppoint();
    }

    #region Level

    public void UpdateExpUI(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;
        Unit unit = unitEventArgs.publisher;

        if (unit.Team == Team.Enemy) return;

        Get<Slider>((int)Sliders.Group_Exp).value = unit.Exp.Value / (float)unit.Exp.Max;
        Get<TextMeshProUGUI>((int)Texts.Txt_Exp)
            .SetText($"{(unit.Exp.Value / (float)unit.Exp.Max * 100):N2}%");
    }

    public void UpdateLevelUI(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;
        Unit unit = unitEventArgs.publisher;

        if (unit.Team == Team.Enemy) return;

        Get<TextMeshProUGUI>((int)Texts.Txt_Level).SetText($"LV.{unit.Level.Value}");
        UpdateLevelUppoint(unit.StageLevel.Value);
    }

    public void UpdateLevelUppoint(object gameEvent = null)
    {
        int levelUpPoint;

        if (gameEvent != null)
        {
            levelUpPoint = gameEvent is int ? (int)gameEvent : 0;
        }
        else
        {
            levelUpPoint = 0;
        }

        if (levelUpPoint != 0)
        {
            for (int i = 0; i < levelUpPoint; i++)
            {
                if (levelUpPoint >= levelUpImages.Length) break;
                levelUpImages[i].transform.GetChild(0).gameObject.SetActive(true);
            }

            for (int i = levelUpPoint; i < levelUpImages.Length; i++)
            {
                if (levelUpPoint >= levelUpImages.Length) break;
                levelUpImages[i].transform.GetChild(0)?.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < levelUpImages.Length; i++)
            {
                if (levelUpImages[i].transform.childCount > 0)
                {
                    levelUpImages[i].transform.GetChild(0)?.gameObject.SetActive(false);
                }
            }
        }
    }

    #endregion


    private int killCount = 0;

    public void UpdateKillCountUI(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;
        Unit unit = unitEventArgs.publisher;
        if (unit.Team == Team.Friendly) return;
        Get<TextMeshProUGUI>((int)Texts.Txt_Count_Kill).SetText($"{++killCount}");
    }
}