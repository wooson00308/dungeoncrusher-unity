using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    [Header("스테이지 정보 리스트")]
    public List<StageInfo> stageInfos = new();
}

[Serializable]
public class StageInfo
{
    [Header("스테이지 지속 시간 (초)")]
    public int durationTime;

    [Header("스폰 시 생성되는 기본 몬스터 수")]
    public int spawnCount;

    [Header("첫 스폰 타임 (ms)")]
    public float firstSpawnTime;

    [Header("추가 스폰 주기 (초)")]
    public float additionalSpawnCycle;

    [Header("추가 스폰 몬스터 수")]
    public int additionalSpawnCount;

    [Header("Under_x2 기준 값 (X2)")]
    public int underX2Threshold;

    [Header("Under_x4 기준 값 (X4)")]
    public int underX4Threshold;

    [Header("추가 스폰 주기 최소값 (MinX)")]
    public float minimumSpawnCycle;

    [Header("추가 스폰 최대 횟수 (-1: 무제한, 0: 없음, n: 제한 횟수)")]
    public int maxAdditionalSpawns;

    [Header("스폰 주기 감소 공식 (Under_x2 및 Under_x4 감소 비율)")]
    public SpawnCycleReductionFormula reductionFormula;

    [Header("스테이지 유닛 데이터 리스트")]
    public List<StageUnitData> stageUnitDatas = new();
}

[Serializable]
public class StageUnitData
{
    [Header("생성될 유닛 데이터")]
    public UnitData stageUnit;

    [Header("몬스터 스폰 비율 가중치")]
    [Range(0f, 100)]
    public float spawnRate;
}

[Serializable]
public class SpawnCycleReductionFormula
{
    [Header("Under_x2 감소 비율 (÷2)")]
    public float underX2Factor = 0.5f;

    [Header("Under_x4 감소 비율 (÷4)")]
    public float underX4Factor = 0.25f;
}
