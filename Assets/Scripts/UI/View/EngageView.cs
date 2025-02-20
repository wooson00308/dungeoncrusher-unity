using System.Collections.Generic;
using UnityEngine;

public class EngageView : BaseView
{
    private readonly Queue<OnHitEventArgs> _damageEventQueue = new();
    private readonly Queue<UnitEventOnAttackArgs> _executionEventQueue = new();
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
        transform.SetSiblingIndex(0); //Main UI 보다 밑에 있게(일부 적 체력바가 Main UI 위로 올라옴)
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
            var executionTxtObject = ResourceManager.Instance.SpawnFromPath("UI/ExecutionTextUI", transform);
            var executionText = executionTxtObject
                .GetComponent<ExecutionTextUI>();

            executionText.Show("처형!", unitEventOnKillArgs.Publisher.transform.position);

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
            var damageTxtObject = ResourceManager.Instance.SpawnFromPath("UI/DamageTextUI", transform);
            var damageText = damageTxtObject
                .GetComponent<DamageTextUI>();
            if (onHitArgs.IsCiritical)
            {
                damageText.Show(onHitArgs.DamageValue, onHitArgs.Publisher.transform.position, true);
            }
            else
            {
                damageText.Show(onHitArgs.DamageValue, onHitArgs.Publisher.transform.position);
            }

            await Awaitable.EndOfFrameAsync();
        }

        _isProcessingDamageQueue = false;
    }

    private void ShowHealthSlider(object gameEvent)
    {
        var setActiveEventArgs = gameEvent as SetActiveEventArgs;
        if (setActiveEventArgs == null) return;
        if (!setActiveEventArgs.IsActive) return;

        HpSliderUI hpSlider;

        if (setActiveEventArgs.Publisher.IsBoss)
        {
            var bossHpSlider = ResourceManager.Instance.SpawnFromPath("UI/BossHpSlider", transform);
            hpSlider = bossHpSlider.GetComponent<HpSliderUI>();
        }
        else
        {
            var normalHpSlider = ResourceManager.Instance.SpawnFromPath("UI/HpSlider", transform);
            hpSlider = normalHpSlider.GetComponent<HpSliderUI>();
        }

        hpSlider.OnInitialize(transform);
        hpSlider.Show(setActiveEventArgs.Publisher);
    }

    private void ShowMpSlider(object gameEvent)
    {
        var setActiveEventArgs = gameEvent as SetActiveEventArgs;
        if (setActiveEventArgs == null) return;
        if (!setActiveEventArgs.IsActive) return;

        MpSliderUI mpSlider;

        if (setActiveEventArgs.Publisher.IsBoss)
        {
            var bossMpSlider = ResourceManager.Instance.SpawnFromPath("UI/BossMpSlider", transform);
            mpSlider = bossMpSlider.GetComponent<MpSliderUI>();
        }
        else
        {
            var normalMpSlider = ResourceManager.Instance.SpawnFromPath("UI/MpSlider", transform);
            mpSlider = normalMpSlider.GetComponent<MpSliderUI>();
        }

        mpSlider.OnInitialize(transform);
        mpSlider.Show(setActiveEventArgs.Publisher);
    }

    public override void BindUI()
    {
    }
}