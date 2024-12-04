using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "StageData", menuName = "Data/StageData")]
public class StageData : ScriptableObject
{
    public List<StageInfo> stageInfos = new();
}

[Serializable]
public class StageInfo
{
    public List<StageUnitData> stageUnitDatas = new();
}

[Serializable]
public class StageUnitData
{
    public UnitData stageUnits;
    public int spawnCount;
}