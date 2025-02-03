using Unity.Cinemachine;
using UnityEngine;

public class ReadyProcess : Process
{
    [SerializeField] private UnitData _playerData;
    [SerializeField] private ChoiceTable _choiceTable;
    [SerializeField] private CinemachineCamera _camera;

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Engage, AllReady);

        Ready2Units();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Engage, AllReady);
        UIManager.Instance.CloseLayoutUI<ReadyUI>();
    }

    private async void Ready2Units()
    {
        if (UnitFactory.Instance.GetTeamUnits(Team.Friendly) == null ||
            UnitFactory.Instance.GetTeamUnits(Team.Friendly).Count == 0)
        {
            UnitFactory.Instance.Spawn(_playerData, Team.Friendly, 1);
        }

        // if (_processSystem.IsSpawnPlayer)
        // {
        //     _processSystem.IsSpawnPlayer = false;

        while (UnitFactory.Instance._parent.Find("Prf_Unit_3") == null)
        {
            UnitFactory.Instance.Spawn(_playerData, Team.Friendly, 1);
            await Awaitable.EndOfFrameAsync();
        }

        if (UnitFactory.Instance._parent.Find("Prf_Unit_3") == null)
        {
            Debug.Log("null");
        }

        var player = UnitFactory.Instance.GetPlayer();

        _camera.Follow = player.transform;

        foreach (var data in _choiceTable.ChoiceDatas)
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
        // }

        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_SetActive, false);
        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_Ready);
        SoundSystem.Instance.PlayBGM("EngageBGM");

        TimeManager.Instance.StopTime();

        UIManager.Instance.ShowLayoutUI<ReadyUI>();
    }

    private void AllReady(object gameEvent)
    {
        _processSystem.OnNextProcess<EngageProcess>();
    }
}