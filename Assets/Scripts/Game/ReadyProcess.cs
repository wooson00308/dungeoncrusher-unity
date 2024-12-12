using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ReadyProcess : Process
{
    [SerializeField] private UnitData _playerData;
    [SerializeField] private CinemachineCamera _camera;

    private bool _isSpawnPlayers = true; // 플레이어 스폰여부

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(ProcessEvents.Engage.ToString(), AllReady);
        UIManager.Instance.ShowLayoutUI<ReadyUI>();

        Ready2Units();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(ProcessEvents.Engage.ToString(), AllReady);
        UIManager.Instance.CloseLayoutUI<ReadyUI>();
    }

    private void Ready2Units()
    {
        var stageInfoData = StageManager.Instance.GetStageData().stageUnitDatas;

        foreach (var stageUnitData in stageInfoData)
        {
            UnitFactory.Instance.Spawn(stageUnitData.stageUnits, Team.Enemy, stageUnitData.spawnCount);
        }

        if (_isSpawnPlayers)
        {
            _isSpawnPlayers = false;

            var units = UnitFactory.Instance.Spawn(_playerData, Team.Friendly, 1);
            _camera.Follow = units[0].transform;
        }

        GameEventSystem.Instance.Publish(ProcessEvents.SetActive.ToString(), new GameEvent { args = false });
        GameEventSystem.Instance.Publish(ProcessEvents.Ready.ToString());
        SoundSystem.Instance.PlayBGM("EngageBGM");
    }

    private void AllReady(GameEvent gameEvent)
    {
        _processSystem.OnNextProcess<EngageProcess>();
    }
}