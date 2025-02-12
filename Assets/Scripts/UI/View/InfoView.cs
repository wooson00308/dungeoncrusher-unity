using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoView : BaseView
{
    private Image[] _levelUpImages;

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

        _levelUpImages = Get<RectTransform>((int)RectTransforms.Group_List_Leveluppoint)
            .GetComponentsInChildren<Image>();
        StartInfo();
    }

    private void StartInfo()
    {
        Get<TextMeshProUGUI>((int)Texts.Txt_Level).SetText($"LV.{1}");
        Get<TextMeshProUGUI>((int)Texts.Txt_Exp).SetText($"{0}%");
        _killCount = 0;
        Get<TextMeshProUGUI>((int)Texts.Txt_Count_Kill).SetText($"{0}");
        Get<Slider>((int)Sliders.Group_Exp).value = 0;

        UpdateLevelUppoint();
    }

    #region Level

    public void UpdateExpUI(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;
        Unit unit = unitEventArgs.Publisher;

        if (unit.Team == Team.Enemy) return;

        Get<Slider>((int)Sliders.Group_Exp).value = unit.Exp.Value / (float)unit.Exp.Max;

        var expValue = unit.Exp.Value / (float)unit.Exp.Max * 100;
        Get<TextMeshProUGUI>((int)Texts.Txt_Exp)
            .SetText($"{expValue:N2}%");
    }

    public void UpdateLevelUI(object gameEvent)
    {
        UnitEventArgs unitEventArgs = gameEvent as UnitEventArgs;
        if (unitEventArgs == null) return;
        Unit unit = unitEventArgs.Publisher;

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
                if (levelUpPoint >= _levelUpImages.Length) break;
                _levelUpImages[i].transform.GetChild(0).gameObject.SetActive(true);
            }

            for (int i = levelUpPoint; i < _levelUpImages.Length; i++)
            {
                if (levelUpPoint >= _levelUpImages.Length) break;
                _levelUpImages[i].transform.GetChild(0)?.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _levelUpImages.Length; i++)
            {
                if (_levelUpImages[i].transform.childCount > 0)
                {
                    _levelUpImages[i].transform.GetChild(0)?.gameObject.SetActive(false);
                }
            }
        }
    }

    #endregion

    private int _killCount = 0;

    public void UpdateKillCountUI(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;
        Unit unit = unitEventArgs.Publisher;
        if (unit.Team == Team.Friendly) return;
        Get<TextMeshProUGUI>((int)Texts.Txt_Count_Kill).SetText($"{++_killCount}");
    }
}