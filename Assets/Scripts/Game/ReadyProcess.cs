using Unity.Cinemachine;
using UnityEngine;

public class ReadyProcess : Process
{
    [SerializeField] private UnitData _playerData;
    [SerializeField] private ChoiceTable _choiceTable;
    [SerializeField] private CinemachineCamera _camera;
    Unit _player;
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

        SetPlayerSkillsNItems(_player);

        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_SetActive, false);
        GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_Ready);
        SoundSystem.Instance.PlayBGM("EngageBGM");

        TimeManager.Instance.StopTime();
    }

    private bool _isSetPlayerSkillsItems = false;

    private void SetPlayerSkillsNItems(Unit _player)
    {
        if (_isSetPlayerSkillsItems) return;
        
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

        _isSetPlayerSkillsItems = true;
    }

    private void AllReady(object gameEvent)
    {
        _processSystem.OnNextProcess<EngageProcess>();
    }
}