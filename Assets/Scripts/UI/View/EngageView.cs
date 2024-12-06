using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EngageView : BaseView
{
    public enum GameObjects
    {
        EngageStart_Group
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_SetActive.ToString(), ShowHealthSlider, ShowMpSlider);
        // GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_SetActive.ToString(), ShowMpSlider);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnHit.ToString(), ShowDamageText);
        BindUI();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_SetActive.ToString(), ShowHealthSlider, ShowMpSlider);
        // GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_SetActive.ToString(), ShowMpSlider);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnHit.ToString(), ShowDamageText);
    }

    private void ShowDamageText(GameEvent gameEvent)
    {
        var onHitArgs = gameEvent.args as OnHitEventArgs;
        var damageText = ResourceManager.Instance.SpawnFromPath("UI/DamageTextUI").GetComponent<DamageTextUI>();
        damageText.Show(onHitArgs.damageValue, onHitArgs.publisher.transform.position);
    }

    private void ShowHealthSlider(GameEvent gameEvent)
    {
        var setActiveEventArgs = gameEvent.args as SetActiveEventArgs;
        if(!setActiveEventArgs.isActive) return;

        var hpSlider = ResourceManager.Instance.SpawnFromPath("UI/HpSlider").GetComponent<HpSliderUI>();
        hpSlider.Show(setActiveEventArgs.publisher);
    }

    private void ShowMpSlider(GameEvent gameEvent)
    {
        var setActiveEventArgs = gameEvent.args as SetActiveEventArgs;
        if(!setActiveEventArgs.isActive) return;
        
        var mpSlider = ResourceManager.Instance.SpawnFromPath("UI/MpSlider").GetComponent<MpSliderUI>();
        mpSlider.Show(setActiveEventArgs.publisher);
    }

    public override void BindUI()
    {
        Bind<GameObject>(typeof(GameObjects));
        UpdateUI();
    }

    private void UpdateUI()
    {
        //Get<Image>((int)Images.StageImage).gameObject.SetActive(true);
        //Get<GameObject>((int)GameObjects.EngageStart_Group).gameObject.SetActive(true);
        //Get<TextMeshProUGUI>((int)Texts.StageNumText).SetText($"{StageManager.Instance.CurrentStage}");
    }
}