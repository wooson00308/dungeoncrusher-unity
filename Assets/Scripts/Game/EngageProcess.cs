using System;
using UnityEngine;

public class EngageProcess : Process
{
    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnDeath.ToString(), TryNextProcess);

        UIManager.Instance.ShowLayoutUI<EngageUI>();
        EngageStart();
    }

    private async void EngageStart()
    {
        await Awaitable.WaitForSecondsAsync(1f);
        GameEventSystem.Instance.Publish(ProcessEvents.SetActive.ToString(), new GameEvent { args = true });
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnDeath.ToString(), TryNextProcess);

        UIManager.Instance.CloseLayoutUI<EngageUI>();
    }

    private void TryNextProcess(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;
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

        int enemyCount = UnitFactory.Instance.GetTeamUnits(unit.Team).Count;

        if (enemyCount <= 0)
        {
            if (StageManager.Instance.isAllStageClear)
            {
                _processSystem.OnNextProcess<GameClearProcess>();
            }
            else
            {
                StageManager.Instance.ClearStage();
                _processSystem.OnNextProcess<ReadyProcess>();
            }
        }
    }
}