using System;
using UnityEngine;

public class EngageProcess : Process
{
    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath, TryNextProcess);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath_Special, TryNextProcess);

        UIManager.Instance.ShowLayoutUI<EngageUI>();
        EngageStart();
    }

    private async void EngageStart()
    {
        StageManager.Instance.StartStage();
        await Awaitable.WaitForSecondsAsync(1f);
        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_SetActive, true);

        TimeManager.Instance.PlayTime();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath, TryNextProcess);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath_Special, TryNextProcess);

        UIManager.Instance.CloseLayoutUI<EngageUI>();
    }

    private void TryNextProcess(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;
        Unit unit = unitEventArgs.publisher;

        if (unit.Team == Team.Friendly)
        {
            int friendlyCount = UnitFactory.Instance.GetTeamUnits(unit.Team).Count;

            if (friendlyCount <= 0)
            {
                _processSystem.OnNextProcess<GameOverProcess>();
            }

            return;
        }
    }
}