using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoView : BaseView
{
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
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_Exp.ToString(), UpdateExpUI);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_Level.ToString(), UpdateLevelUI);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnDeath.ToString(), UpdateKillCountUI);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnDeath_Special.ToString(), UpdateKillCountUI);
        GameEventSystem.Instance.Subscribe(ProcessEvents.ProcessEvent_Engage.ToString(), UpdateLevelUppoint);
        StartInfo();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_Exp.ToString(), UpdateExpUI);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_Level.ToString(), UpdateLevelUI);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnDeath.ToString(), UpdateKillCountUI);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnDeath_Special.ToString(), UpdateKillCountUI);
        GameEventSystem.Instance.Unsubscribe(ProcessEvents.ProcessEvent_Engage.ToString(), UpdateLevelUppoint);
    }

    public override void BindUI()
    {
        Bind<Slider>(typeof(Sliders));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<RectTransform>(typeof(RectTransforms));

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

    public void UpdateExpUI(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;
        Unit unit = unitEventArgs.publisher;

        if (unit.Team == Team.Enemy) return;

        Get<Slider>((int)Sliders.Group_Exp).value = unit.Exp.Value / (float)unit.Exp.Max;
        Get<TextMeshProUGUI>((int)Texts.Txt_Exp)
            .SetText($"{unit.Exp.Value / (float)unit.Exp.Max * 100}%");
    }

    public void UpdateLevelUI(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;
        Unit unit = unitEventArgs.publisher;

        if (unit.Team == Team.Enemy) return;

        Get<TextMeshProUGUI>((int)Texts.Txt_Level).SetText($"LV.{unit.Level.Value}");
        UpdateLevelUppoint(gameEvent);
    }
    
    public void UpdateLevelUppoint(GameEvent gameEvent = null)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent?.args;
        Image[] levelUpObject = Get<RectTransform>((int)RectTransforms.Group_List_Leveluppoint)
            .GetComponentsInChildren<Image>();

        if (unitEventArgs != null)
        {
            Unit unit = unitEventArgs.publisher;

            for (int i = 0; i < unit.StageLevel.Value; i++)
            {
                levelUpObject[i].transform.GetChild(0).gameObject.SetActive(true);
            }

            for (int i = unit.StageLevel.Value; i < levelUpObject.Length; i++)
            {
                levelUpObject[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < levelUpObject.Length; i++)
            {
                if (levelUpObject[i].transform.childCount > 0)
                {
                    levelUpObject[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
    
    #endregion


    private int killCount = 0;

    public void UpdateKillCountUI(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;
        Unit unit = unitEventArgs.publisher;
        if (unit.Team == Team.Friendly) return;
        Get<TextMeshProUGUI>((int)Texts.Txt_Count_Kill).SetText($"{++killCount}");
    }
}