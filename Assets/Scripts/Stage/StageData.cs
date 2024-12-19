using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    [Header("�������� ���� ����Ʈ")]
    public List<StageInfo> stageInfos = new();
}

[Serializable]
public class StageInfo
{
    [Header("�������� ���� �ð� (��)")]
    public int durationTime;

    [Header("���� �� �����Ǵ� �⺻ ���� ��")]
    public int spawnCount;

    [Header("ù ���� Ÿ�� (ms)")]
    public float firstSpawnTime;

    [Header("�߰� ���� �ֱ� (��)")]
    public float additionalSpawnCycle;

    [Header("�߰� ���� ���� ��")]
    public int additionalSpawnCount;

    [Header("Under_x2 ���� �� (X2)")]
    public int underX2Threshold;

    [Header("Under_x4 ���� �� (X4)")]
    public int underX4Threshold;

    [Header("�߰� ���� �ֱ� �ּҰ� (MinX)")]
    public float minimumSpawnCycle;

    [Header("�߰� ���� �ִ� Ƚ�� (-1: ������, 0: ����, n: ���� Ƚ��)")]
    public int maxAdditionalSpawns;

    [Header("���� �ֱ� ���� ���� (Under_x2 �� Under_x4 ���� ����)")]
    public SpawnCycleReductionFormula reductionFormula;

    [Header("�������� ���� ������ ����Ʈ")]
    public List<StageUnitData> stageUnitDatas = new();
}

[Serializable]
public class StageUnitData
{
    [Header("������ ���� ������")]
    public UnitData stageUnit;

    [Header("���� ���� ���� ����ġ")]
    [Range(0f, 100)]
    public float spawnRate;
}

[Serializable]
public class SpawnCycleReductionFormula
{
    [Header("Under_x2 ���� ���� (��2)")]
    public float underX2Factor = 0.5f;

    [Header("Under_x4 ���� ���� (��4)")]
    public float underX4Factor = 0.25f;
}
