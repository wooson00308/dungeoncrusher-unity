using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMini<StageManager>
{
    private int _currentStage = 1;
    public int CurrentStage => _currentStage;
    public StageData _stageDatas;

    public bool IsAllStageClear => _stageDatas.stageInfos.Count <= _currentStage;

    private Coroutine _spawnCoroutine;
    private Coroutine _stageTimerCoroutine;
    private bool _isStageCleared = false;

    public void ClearStage()
    {
        _currentStage++;
        _isStageCleared = true;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }

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
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }

        if (_stageTimerCoroutine != null)
        {
            StopCoroutine(_stageTimerCoroutine);
        }

        _isStageCleared = false;
        _spawnCoroutine = StartCoroutine(MonsterSpawnRoutine());
        _stageTimerCoroutine = StartCoroutine(StageTimerRoutine());
    }

    private IEnumerator MonsterSpawnRoutine()
    {
        StageInfo currentStageInfo = GetStageData();
        if (currentStageInfo == null)
            yield break;

        // 첫 스폰
        yield return new WaitForSeconds(currentStageInfo.firstSpawnTime / 1000f);
        SpawnMonsters(currentStageInfo.spawnCount, currentStageInfo.stageUnitDatas);

        int additionalSpawns = 0;
        float spawnCycle = currentStageInfo.additionalSpawnCycle;

        while (!_isStageCleared && (currentStageInfo.maxAdditionalSpawns == -1 || additionalSpawns < currentStageInfo.maxAdditionalSpawns))
        {
            int currentMonsterCount = GetCurrentMonsterCount();

            // 스폰 주기 감소 계산
            if (currentMonsterCount < currentStageInfo.underX4Threshold)
            {
                spawnCycle = Mathf.Max(currentStageInfo.minimumSpawnCycle, spawnCycle * currentStageInfo.reductionFormula.underX4Factor);
            }
            else if (currentMonsterCount < currentStageInfo.underX2Threshold)
            {
                spawnCycle = Mathf.Max(currentStageInfo.minimumSpawnCycle, spawnCycle * currentStageInfo.reductionFormula.underX2Factor);
            }

            yield return new WaitForSeconds(spawnCycle);

            SpawnMonsters(currentStageInfo.additionalSpawnCount, currentStageInfo.stageUnitDatas);
            additionalSpawns++;
        }
    }

    private IEnumerator StageTimerRoutine()
    {
        StageInfo currentStageInfo = GetStageData();
        if (currentStageInfo == null)
            yield break;

        float remainingTime = currentStageInfo.durationTime;
        while (remainingTime > 0 && !_isStageCleared)
        {
            yield return new WaitForSeconds(1f);
            remainingTime--;
            Debug.Log($"Time Remaining: {remainingTime}s");
        }

        if (!_isStageCleared)
        {
            if (HasBossOnField())
            {
                Debug.Log("Boss exists, stage will not end.");
                yield break;
            }

            ClearAllMonsters();

            if (IsAllStageClear)
            {
                ProcessSystem.Instance.OnNextProcess<GameClearProcess>();
            }
            else
            {
                ClearStage();
                ProcessSystem.Instance.OnNextProcess<ReadyProcess>();
            }
        }
    }

    private void SpawnMonsters(int count, List<StageUnitData> unitDatas)
    {
        if (unitDatas == null || unitDatas.Count == 0)
            return;

        float totalWeight = 0f;
        foreach (var data in unitDatas)
        {
            totalWeight += data.spawnRate;
        }

        foreach (var data in unitDatas)
        {
            int spawnCount = Mathf.FloorToInt((data.spawnRate / totalWeight) * count);
            if (spawnCount > 0)
            {
                UnitFactory.Instance.Spawn(data.stageUnit, Team.Enemy, spawnCount);
                //Debug.Log($"{spawnCount} monsters spawned of type: {data.stageUnit.name}");
            }
        }
    }

    private void ClearAllMonsters()
    {
        UnitFactory.Instance.KillTeamUnits(Team.Enemy);
        //Debug.Log("All monsters cleared from the field.");
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
