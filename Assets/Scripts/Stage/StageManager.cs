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
        // 1. 첫 스폰 시간 대기
        yield return new WaitForSeconds(unitData.firstSpawnTime / 1000f);

        // 2. 첫 스폰 실행
        SpawnMonsters(unitData.spawnCount, unitData);

        int additionalSpawns = 0;

        // 추가 스폰 기준 주기
        float spawnCycle = unitData.additionalSpawnCycle;

        // 무제한(-1) 혹은 아직 추가 스폰 횟수가 남아 있을 때
        while (!_isStageCleared &&
              (unitData.maxAdditionalSpawns == -1 || additionalSpawns < unitData.maxAdditionalSpawns))
        {
            float elapsedTime = 0f; // 이번 사이클의 경과 시간

            // 스폰 전까지 “조금씩” 기다리면서 매 프레임마다 몬스터 수 체크
            while (elapsedTime < spawnCycle && !_isStageCleared)
            {
                // 1) 일단 한 프레임 기다림
                yield return null;

                // 2) 경과 시간 누적
                elapsedTime += Time.deltaTime;

                // 3) 몬스터 수에 따른 새 스폰 주기 계산
                float newSpawnCycle = spawnCycle;
                int currentMonsterCount = GetCurrentMonsterCount();

                // 몬스터가 적으면 스폰 대기 시간을 줄이기
                if (currentMonsterCount < unitData.underX4Threshold)
                {
                    newSpawnCycle = Mathf.Max(
                        unitData.minimumSpawnCycle,
                        spawnCycle * unitData.reductionFormula.underX4Factor
                    );
                }
                else if (currentMonsterCount < unitData.underX2Threshold)
                {
                    newSpawnCycle = Mathf.Max(
                        unitData.minimumSpawnCycle,
                        spawnCycle * unitData.reductionFormula.underX2Factor
                    );
                }

                // 새로 계산된 스폰 주기가 작아졌다면 spawnCycle에 반영
                if (newSpawnCycle < spawnCycle)
                {
                    spawnCycle = newSpawnCycle;
                }

                // 만약 이미 경과한 시간이 새 스폰 주기를 초과했으면, 즉시 다음 스폰 진행
                if (elapsedTime >= spawnCycle)
                {
                    break;
                }
            }

            // 스테이지가 끝났다면 코루틴 종료
            if (_isStageCleared)
                yield break;

            // 몬스터 스폰
            SpawnMonsters(unitData.additionalSpawnCount, unitData);
            additionalSpawns++;

            // 다음 루프(다음 추가 스폰) 전에, 스폰 주기가 너무 줄어들지 않았는지 마지막 한 번 더 보정
            spawnCycle = Mathf.Max(unitData.minimumSpawnCycle, spawnCycle);
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
