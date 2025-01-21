using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ReadyProcess : Process
{
    [SerializeField] private UnitData _playerData;
    [SerializeField] private ChoiceTable _choiceTable;
    [SerializeField] private CinemachineCamera _camera;

    private bool _isSpawnPlayers = true; // 플레이어 스폰여부

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Engage, AllReady);
        UIManager.Instance.ShowLayoutUI<ReadyUI>();

        Ready2Units();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Engage, AllReady);
        UIManager.Instance.CloseLayoutUI<ReadyUI>();
    }

    private async void Ready2Units()
    {
        await Awaitable.EndOfFrameAsync();

        var stageInfoData = StageManager.Instance.GetStageData().stageUnitDatas;

        if (_isSpawnPlayers)
        {
            _isSpawnPlayers = false;

            UnitFactory.Instance.Spawn(_playerData, Team.Friendly, 1);
            var player = UnitFactory.Instance.GetPlayer();

            _camera.Follow = player.transform;

            foreach(var data in _choiceTable.ChoiceDatas)
            {
                if (data.choiceType == ChoiceType.Item)
                {
                    var item = ResourceManager.Instance.Spawn(data.itemData.Prefab).GetComponent<Item>();
                    player.EquipItem(item);
                }
                else if (data.choiceType == ChoiceType.Skill)
                {
                    player.AddSkill(data.skillData);
                }
                else
                {
                    player.UpdateStats("Enagage", data.unitStatUpgradeData);
                }
            }
        }

        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_SetActive, false);
        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_Ready);
        SoundSystem.Instance.PlayBGM("EngageBGM");

        TimeManager.Instance.StopTime();
    }

    private void AllReady(object gameEvent)
    {
        _processSystem.OnNextProcess<EngageProcess>();
    }
}