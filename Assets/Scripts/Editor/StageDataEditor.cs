#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

[CustomEditor(typeof(StageData))]
public class StageDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("CSV 내보내기"))
        {
            ExportCSV();
        }

        if (GUILayout.Button("CSV 불러오기"))
        {
            ImportCSV();
        }
    }

    private void ExportCSV()
    {
        StageData stageData = (StageData)target;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("스테이지 번호,유닛 번호,스테이지 지속 시간 (초),스폰 시 생성되는 기본 몬스터 수,첫 스폰 타임 (ms),추가 스폰 주기 (초),추가 스폰 몬스터 수,Under_x2 기준 값 (X2),Under_x4 기준 값 (X4),추가 스폰 주기 최소값 (MinX),추가 스폰 최대 횟수 (-1: 무제한 | 0: 없음 | n: 제한 횟수),Under_x2 감소 비율 (÷2),Under_x4 감소 비율 (÷4)");

        for (int stageIndex = 0; stageIndex < stageData.stageInfos.Count; stageIndex++)
        {
            StageInfo stageInfo = stageData.stageInfos[stageIndex];
            for (int unitIndex = 0; unitIndex < stageInfo.stageUnitDatas.Count; unitIndex++)
            {
                StageUnitData unitData = stageInfo.stageUnitDatas[unitIndex];
                float underX2Factor = unitData.reductionFormula != null ? unitData.reductionFormula.underX2Factor : 0;
                float underX4Factor = unitData.reductionFormula != null ? unitData.reductionFormula.underX4Factor : 0;

                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                    stageIndex,
                    unitIndex,
                    stageInfo.durationTime,
                    unitData.spawnCount,
                    unitData.firstSpawnTime,
                    unitData.additionalSpawnCycle,
                    unitData.additionalSpawnCount,
                    unitData.underX2Threshold,
                    unitData.underX4Threshold,
                    unitData.minimumSpawnCycle,
                    unitData.maxAdditionalSpawns,
                    underX2Factor,
                    underX4Factor));
            }
        }

        string path = EditorUtility.SaveFilePanel("CSV 파일 저장", "", "StageData.csv", "csv");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
            ShowNotification("CSV 내보내기 완료\nCSV 파일이 다음 경로에 저장되었습니다:\n" + path);
        }
    }

    private void ImportCSV()
    {
        string path = EditorUtility.OpenFilePanel("CSV 파일 불러오기", "", "csv");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(path);
            if (lines.Length < 2)
            {
                Debug.LogWarning("CSV 파일에 데이터가 없거나 헤더만 있습니다.");
                return;
            }

            Dictionary<int, StageInfo> stageDict = new Dictionary<int, StageInfo>();
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] columns = lines[i].Split(',');
                if (columns.Length < 13)
                {
                    Debug.LogWarning("CSV 라인의 컬럼 수가 부족합니다: " + lines[i]);
                    continue;
                }

                int stageIndex = int.Parse(columns[0]);
                int stageDurationTime = int.Parse(columns[2]);
                int spawnCount = int.Parse(columns[3]);
                float firstSpawnTime = float.Parse(columns[4]);
                float additionalSpawnCycle = float.Parse(columns[5]);
                int additionalSpawnCount = int.Parse(columns[6]);
                int underX2Threshold = int.Parse(columns[7]);
                int underX4Threshold = int.Parse(columns[8]);
                float minimumSpawnCycle = float.Parse(columns[9]);
                int maxAdditionalSpawns = int.Parse(columns[10]);
                float underX2Factor = float.Parse(columns[11]);
                float underX4Factor = float.Parse(columns[12]);

                if (!stageDict.TryGetValue(stageIndex, out StageInfo stageInfo))
                {
                    stageInfo = new StageInfo
                    {
                        durationTime = stageDurationTime,
                        stageUnitDatas = new List<StageUnitData>()
                    };
                    stageDict.Add(stageIndex, stageInfo);
                }
                else if (stageInfo.durationTime != stageDurationTime)
                {
                    stageInfo.durationTime = stageDurationTime;
                }

                StageUnitData unitData = new StageUnitData
                {
                    spawnCount = spawnCount,
                    firstSpawnTime = firstSpawnTime,
                    additionalSpawnCycle = additionalSpawnCycle,
                    additionalSpawnCount = additionalSpawnCount,
                    underX2Threshold = underX2Threshold,
                    underX4Threshold = underX4Threshold,
                    minimumSpawnCycle = minimumSpawnCycle,
                    maxAdditionalSpawns = maxAdditionalSpawns,
                    reductionFormula = new SpawnCycleReductionFormula
                    {
                        underX2Factor = underX2Factor,
                        underX4Factor = underX4Factor
                    }
                };

                stageInfo.stageUnitDatas.Add(unitData);
            }

            List<int> keys = new List<int>(stageDict.Keys);
            keys.Sort();
            List<StageInfo> stageInfos = new List<StageInfo>();
            foreach (int key in keys)
            {
                stageInfos.Add(stageDict[key]);
            }

            StageData stageData = (StageData)target;
            stageData.stageInfos = stageInfos;

            EditorUtility.SetDirty(stageData);
            AssetDatabase.SaveAssets();

            ShowNotification("CSV 불러오기 완료\nCSV 파일이 성공적으로 불러와졌습니다:\n" + path);
        }
        catch (Exception ex)
        {
            ShowNotification("CSV 불러오기 실패\n" + ex.Message);
        }
    }

    private void ShowNotification(string message)
    {
        EditorWindow window = EditorWindow.focusedWindow;
        if (window != null)
        {
            window.ShowNotification(new GUIContent(message));
        }
        else
        {
            Debug.Log(message);
        }
    }
}
#endif
