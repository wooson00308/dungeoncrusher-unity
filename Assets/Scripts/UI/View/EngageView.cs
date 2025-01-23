using System.Collections.Generic;
using UnityEngine;

public class EngageView : BaseView
{
    private Queue<OnHitEventArgs> _damageEventQueue = new Queue<OnHitEventArgs>();
    private Queue<UnitEventOnAttackArgs> _executionEventQueue = new Queue<UnitEventOnAttackArgs>();
    private bool _isProcessingDamageQueue = false;
    private bool _isProcessingExecutionQueue = false;

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_SetActive, ShowHealthSlider, ShowMpSlider);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnHit, EnqueueDamageText);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath_Execution, ExecutionText);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_SetActive, ShowHealthSlider, ShowMpSlider);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnHit, EnqueueDamageText);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath_Execution, ExecutionText);
    }

    private void EnqueueDamageText(object gameEvent)
    {
        if (gameEvent is OnHitEventArgs onHitArgs)
        {
            _damageEventQueue.Enqueue(onHitArgs);
            if (!_isProcessingDamageQueue)
            {
                ProcessDamageQueue();
            }
        }
    }

    private void ExecutionText(object gameEvent)
    {
        if (gameEvent is UnitEventOnAttackArgs onHitArgs)
        {
            _executionEventQueue.Enqueue(onHitArgs);
            if (!_isProcessingExecutionQueue)
            {
                ProcessExecutionQueue();
            }
        }
    }

    private async void ProcessExecutionQueue()
    {
        _isProcessingExecutionQueue = true;

        while (_executionEventQueue.Count > 0)
        {
            var unitEventOnKillArgs = _executionEventQueue.Dequeue();
            var executionText = ResourceManager.Instance.SpawnFromPath("UI/ExecutionTextUI")
                .GetComponent<ExecutionTextUI>();

            executionText.Show("처형!", unitEventOnKillArgs.publisher.transform.position);

            await Awaitable.EndOfFrameAsync();
        }

        _isProcessingExecutionQueue = false;
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

    private void ShowHealthSlider(object gameEvent)
    {
        var setActiveEventArgs = gameEvent as SetActiveEventArgs;
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

    private void ShowMpSlider(object gameEvent)
    {
        var setActiveEventArgs = gameEvent as SetActiveEventArgs;
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
    }
}