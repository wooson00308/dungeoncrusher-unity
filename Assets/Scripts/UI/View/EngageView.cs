using System.Collections.Generic;
using UnityEngine;

public class EngageView : BaseView
{
    public enum GameObjects
    {
        EngageStart_Group
    }

    private Queue<OnHitEventArgs> _damageEventQueue = new Queue<OnHitEventArgs>();
    private bool _isProcessingDamageQueue = false;

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_SetActive.ToString(), ShowHealthSlider, ShowMpSlider);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnHit.ToString(), EnqueueDamageText);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_SetActive.ToString(), ShowHealthSlider, ShowMpSlider);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnHit.ToString(), EnqueueDamageText);
    }

    private void EnqueueDamageText(GameEvent gameEvent)
    {
        if (gameEvent.args is OnHitEventArgs onHitArgs)
        {
            _damageEventQueue.Enqueue(onHitArgs);
            if (!_isProcessingDamageQueue)
            {
                ProcessDamageQueue();
            }
        }
    }

    private async void ProcessDamageQueue()
    {
        _isProcessingDamageQueue = true;

        while (_damageEventQueue.Count > 0)
        {
            var onHitArgs = _damageEventQueue.Dequeue();
            var damageText = ResourceManager.Instance.SpawnFromPath("UI/DamageTextUI").GetComponent<DamageTextUI>();
            if (onHitArgs.isCiritical)
            {
                damageText.Show(onHitArgs.damageValue, onHitArgs.publisher.transform.position, true);
            }
            else
            {
                damageText.Show(onHitArgs.damageValue, onHitArgs.publisher.transform.position);
            }
            await Awaitable.EndOfFrameAsync();
        }

        _isProcessingDamageQueue = false;
    }

    private void ShowHealthSlider(GameEvent gameEvent)
    {
        var setActiveEventArgs = gameEvent.args as SetActiveEventArgs;
        if (!setActiveEventArgs.isActive) return;

        HpSliderUI hpSlider;

        if (setActiveEventArgs.publisher.IsBoss)
        {
            hpSlider = ResourceManager.Instance.SpawnFromPath("UI/BossHpSlider").GetComponent<HpSliderUI>();
        }
        else
        {
            hpSlider = ResourceManager.Instance.SpawnFromPath("UI/HpSlider").GetComponent<HpSliderUI>();
        }

        hpSlider.Show(setActiveEventArgs.publisher);
    }

    private void ShowMpSlider(GameEvent gameEvent)
    {
        var setActiveEventArgs = gameEvent.args as SetActiveEventArgs;
        if (!setActiveEventArgs.isActive) return;

        MpSliderUI mpSlider;

        if (setActiveEventArgs.publisher.IsBoss)
        {
            mpSlider = ResourceManager.Instance.SpawnFromPath("UI/BossMpSlider").GetComponent<MpSliderUI>();
        }
        else
        {
            mpSlider = ResourceManager.Instance.SpawnFromPath("UI/MpSlider").GetComponent<MpSliderUI>();
        }

        mpSlider.Show(setActiveEventArgs.publisher);
    }

    public override void BindUI()
    {
        Bind<GameObject>(typeof(GameObjects));
    }
}