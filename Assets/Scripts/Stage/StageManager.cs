using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMini<StageManager>
{
    private int _currentStage = 1;
    public int CurrentStage => _currentStage;
    public StageData _stageDatas;

    public bool IsAllStageClear => _stageDatas.stageInfos.Count < _currentStage;

    private List<Coroutine> _spawnCoroutines = new List<Coroutine>();
    private Coroutine _stageTimerCoroutine;
    private bool _isStageCleared = false;

    private float _engageTime;
    public float EnageTime => _engageTime;

    public void ClearStage()
    {
        _currentStage++;
        _isStageCleared = true;

        foreach (var coroutine in _spawnCoroutines)
        {
            StopCoroutine(coroutine);
        }
        _spawnCoroutines.Clear();

        if (_stageTimerCoroutine != null)
        {
            StopCoroutine(_stageTimerCoroutine);
        }
    }

    public StageInfo GetStageData()
    {
        if (_stageDatas.stageInfos.Count < _currentStage)
        {
            Debug.LogError("stageData 갯수보다 현재 스테이지가 더 높습니다");
            return null;
        }

        return _stageDatas.stageInfos[_currentStage - 1];
    }

    public void StartStage()
    {
        foreach (var coroutine in _spawnCoroutines)
        {
            StopCoroutine(coroutine);
        }
        _spawnCoroutines.Clear();

        if (_stageTimerCoroutine != null)
        {
            StopCoroutine(_stageTimerCoroutine);
        }

        _isStageCleared = false;

        StageInfo currentStageInfo = GetStageData();
        if (currentStageInfo != null)
        {
            foreach (var unitData in currentStageInfo.stageUnitDatas)
            {
                var spawnCoroutine = StartCoroutine(MonsterSpawnRoutine(unitData));
                _spawnCoroutines.Add(spawnCoroutine);
            }

            _stageTimerCoroutine = StartCoroutine(StageTimerRoutine());
        }
    }

    private IEnumerator MonsterSpawnRoutine(StageUnitData unitData)
    {
        yield return new WaitForSeconds(unitData.firstSpawnTime / 1000f);
        SpawnMonsters(unitData.spawnCount, unitData);

        int additionalSpawns = 0;

        while (!_isStageCleared && (unitData.maxAdditionalSpawns == -1 || additionalSpawns < unitData.maxAdditionalSpawns))
        {
            float currentTime = 0;
            float spawnCycle = unitData.additionalSpawnCycle;

            while (currentTime < spawnCycle)
            {
                int currentMonsterCount = GetCurrentMonsterCount();

                if (currentMonsterCount < unitData.underX4Threshold)
                {
                    spawnCycle = Mathf.Max(unitData.minimumSpawnCycle, spawnCycle * unitData.reductionFormula.underX4Factor);
                }
                else if (currentMonsterCount < unitData.underX2Threshold)
                {
                    spawnCycle = Mathf.Max(unitData.minimumSpawnCycle, spawnCycle * unitData.reductionFormula.underX2Factor);
                }

                currentTime += Time.deltaTime;

                yield return null;
            }

            SpawnMonsters(unitData.additionalSpawnCount, unitData);
            additionalSpawns++;

            yield return null;
        }
    }

    private IEnumerator StageTimerRoutine()
    {
        StageInfo currentStageInfo = GetStageData();
        if (currentStageInfo == null)
            yield break;

        int maxDuration = currentStageInfo.durationTime;

        _engageTime = maxDuration;
        while (_engageTime > 0 && !_isStageCleared)
        {
            yield return new WaitForSeconds(1f);
            _engageTime--;
            Debug.Log($"Time Remaining: {_engageTime}s");
        }

        if (!_isStageCleared)
        {
            if (HasBossOnField())
            {
                Debug.Log("Boss exists, stage will not end.");
                yield break;
            }

            ClearAllMonsters();
            ClearStage();
            if (IsAllStageClear)
            {
                ProcessSystem.Instance.OnNextProcess<GameClearProcess>();
            }
            else
            {
                ProcessSystem.Instance.OnNextProcess<ReadyProcess>();
            }
        }
    }

    private void SpawnMonsters(int count, StageUnitData unitData)
    {
        if (unitData == null)
            return;

        UnitFactory.Instance.Spawn(unitData.stageUnit, Team.Enemy, count);
        Debug.Log($"{count} monsters spawned of type: {unitData.stageUnit.name}");
    }

    private void ClearAllMonsters()
    {
        UnitFactory.Instance.DestroyTeamUnits(Team.Enemy);
        Debug.Log("All monsters cleared from the field.");
    }

    private bool HasBossOnField()
    {
        var enemyUnits = UnitFactory.Instance.GetTeamUnits(Team.Enemy);
        foreach (var unit in enemyUnits)
        {
            if (unit.IsBoss)
            {
                return true;
            }
        }
        return false;
    }

    private int GetCurrentMonsterCount()
    {
        return UnitFactory.Instance.GetTeamUnits(Team.Enemy).Count;
    }
}
