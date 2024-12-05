using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReadyProcess : Process
{
    [SerializeField] private List<UnitData> _datas = new();

    private bool _isSpawnPlayers = true; // 플레이어 스폰여부

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(ProcessEvents.Engage.ToString(), AllReady);
        UIManager.Instance.ShowLayoutUI<ReadyUI>();

        Ready2Units();
    }

    private void OnDisable()
    {
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

            foreach (var unitData in _datas)
            {
                var units = UnitFactory.Instance.Spawn(unitData, Team.Friendly, 1);
                units[0].AddSkill(Resources.Load<SkillData>("Data/Skill/Data_Skill_997"));
                units[0].EquipItem(Resources.Load<Item>("Item/Prf_Item_990"));
            }
        }

        GameEventSystem.Instance.Publish(ProcessEvents.SetActive.ToString(), new GameEvent { args = false });
        GameEventSystem.Instance.Publish(ProcessEvents.Ready.ToString());
    }

    private void AllReady(GameEvent gameEvent)
    {
        _processSystem.OnNextProcess<EngageProcess>();
    }
}