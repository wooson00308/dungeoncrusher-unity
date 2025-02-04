using Unity.Cinemachine;
using UnityEngine;

public class ReadyProcess : Process
{
    [SerializeField] private UnitData _playerData;
    [SerializeField] private ChoiceTable _choiceTable;
    [SerializeField] private CinemachineCamera _camera;
    Unit _player = null;
    private void OnEnable()
    {
        Ready2Units();
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Engage, AllReady);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Engage, AllReady);
        UIManager.Instance.CloseLayoutUI<ReadyUI>();
    }

    private async void Ready2Units()
    {
        // while (UnitFactory.Instance.GetTeamUnits(Team.Friendly) == null ||
        //        UnitFactory.Instance.GetTeamUnits(Team.Friendly).Count == 0)
        // {
        //     UnitFactory.Instance.Spawn(_playerData, Team.Friendly, 1);
        //     await Awaitable.EndOfFrameAsync();
        // }
        //
        // while (UnitFactory.Instance._parent.Find("Prf_Unit_3") == null)
        // {
        //     UnitFactory.Instance.Spawn(_playerData, Team.Friendly, 1);
        //     await Awaitable.EndOfFrameAsync();
        // }
        //
        // if (UnitFactory.Instance.GetTeamUnits(Team.Friendly) == null ||
        //     UnitFactory.Instance.GetTeamUnits(Team.Friendly).Count == 0)
        // {
        //     Debug.Log("null or Count 0");
        // }
        //
        // if (UnitFactory.Instance._parent.Find("Prf_Unit_3") == null)
        // {
        //     Debug.Log("null");
        // }
        // }
        


        if (!_processSystem.IsSpawnPlayer)
        {
            UnitFactory.Instance.Spawn(_playerData, Team.Friendly, 1);
            _processSystem.IsSpawnPlayer = true;
        }

        while (_player == null)
        {
            _player = UnitFactory.Instance.GetPlayer();
            await Awaitable.EndOfFrameAsync();
        }

        if (_player != null)
        {
            _camera.Follow = _player.transform;

            if (_processSystem.IsSpawnPlayer)
            {
                UIManager.Instance.ShowLayoutUI<ReadyUI>();
            }
        }

        SetPlayerSkillsItems(_player);

        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_SetActive, false);
        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_Ready);
        SoundSystem.Instance.PlayBGM("EngageBGM");

        TimeManager.Instance.StopTime();
    }

    private bool isSetPlayerSkillsItems = false;

    private void SetPlayerSkillsItems(Unit _player)
    {
        if (isSetPlayerSkillsItems) return;
        
        foreach (var data in _choiceTable.ChoiceDatas)
        {
            if (data.choiceType == ChoiceType.Item)
            {
                var item = ResourceManager.Instance.Spawn(data.itemData.Prefab).GetComponent<Item>();
                _player.EquipItem(item);
            }
            else if (data.choiceType == ChoiceType.Skill)
            {
                _player.AddSkill(data.skillData);
            }
            else
            {
                _player.UpdateStats("Enagage", data.unitStatUpgradeData);
            }
        }

        isSetPlayerSkillsItems = true;
    }

    private void AllReady(object gameEvent)
    {
        _processSystem.OnNextProcess<EngageProcess>();
    }
}