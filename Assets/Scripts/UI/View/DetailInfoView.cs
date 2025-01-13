using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailInfoView : BaseView
{
    public enum Images
    {
        UnitIcon
    }

    public enum RectTransforms
    {
        Grid_Stats
    }

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        UpdateStatsUI();
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvnet_LevelUpCount.ToString(), UpdateStatsUI);
        GameEventSystem.Instance.Subscribe(ProcessEvents.ProcessEvent_Engage.ToString(), UpdateStatsUI);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_Mana_Regen.ToString(), UpdateStatsUI);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnHit.ToString(), UpdateStatsUI);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvnet_LevelUpCount.ToString(), UpdateStatsUI);
        GameEventSystem.Instance.Unsubscribe(ProcessEvents.ProcessEvent_Engage.ToString(), UpdateStatsUI);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_Mana_Regen.ToString(), UpdateStatsUI);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnHit.ToString(), UpdateStatsUI);
    }

    public override void BindUI()
    {
        Bind<Image>(typeof(Images));
        Bind<RectTransform>(typeof(RectTransforms));
    }

    private void UpdateStatsUI(GameEvent gameEvent = null)
    {
        Unit player = UnitFactory.Instance.GetPlayer();

        if (player == null) return;

        Get<Image>((int)Images.UnitIcon).sprite = player.Icon;
        TextMeshProUGUI[] statsTexts = Get<RectTransform>((int)RectTransforms.Grid_Stats)
            .GetComponentsInChildren<TextMeshProUGUI>();

        statsTexts[0].SetText($"Health : {player.Health.Value}/{player.Health.Max}");
        statsTexts[1].SetText($"Defense : {player.Defense.Value}");
        statsTexts[2].SetText($"Attack : {player.Attack.Value}");
        statsTexts[3].SetText($"Mp : {player.Mp.Value}/{player.Mp.Max}");
        statsTexts[4].SetText($"AttackSpeed : {player.AttackSpeed.Value}");
        statsTexts[5].SetText($"CriticalRate : {player.CriticalRate.Value}");
        statsTexts[6].SetText($"CriticalPer : {player.CriticalPercent.Value}");
        statsTexts[7].SetText($"LifeStealRate : {player.LifestealRate.Value}");
        statsTexts[8].SetText($"LifeStealPer : {player.LifestealPercent.Value}");
        statsTexts[9].SetText($"Speed : {player.Speed.Value}");
    }
}