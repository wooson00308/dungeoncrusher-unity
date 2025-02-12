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
        // 기본 인스펙터 GUI 표시
        base.OnInspectorGUI();

        // CSV 내보내기 버튼 추가
        if (GUILayout.Button("Export CSV"))
        {
            ExportCSV();
        }

        // CSV 불러오기 버튼 추가
        if (GUILayout.Button("Import CSV"))
        {
            ImportCSV();
        }
    }

    private void ExportCSV()
    {
        StageData stageData = (StageData)target;
        StringBuilder sb = new StringBuilder();

        // CSV 헤더 (원하는 항목에 맞게 수정 가능)
        sb.AppendLine("StageIndex,StageDurationTime,UnitIndex,SpawnCount,FirstSpawnTime,AdditionalSpawnCycle,AdditionalSpawnCount,UnderX2Threshold,UnderX4Threshold,MinimumSpawnCycle,MaxAdditionalSpawns,UnderX2Factor,UnderX4Factor");

        // StageData의 각 스테이지 정보를 순회하며 CSV 데이터 생성
        for (int stageIndex = 0; stageIndex < stageData.stageInfos.Count; stageIndex++)
        {
            StageInfo stageInfo = stageData.stageInfos[stageIndex];

            for (int unitIndex = 0; unitIndex < stageInfo.stageUnitDatas.Count; unitIndex++)
            {
                StageUnitData unitData = stageInfo.stageUnitDatas[unitIndex];

                // reductionFormula가 null일 경우 대비하여 기본값(0) 처리
                float underX2Factor = unitData.reductionFormula != null ? unitData.reductionFormula.underX2Factor : 0;
                float underX4Factor = unitData.reductionFormula != null ? unitData.reductionFormula.underX4Factor : 0;

                // CSV의 한 행 생성
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                    stageIndex,
                    stageInfo.durationTime,
                    unitIndex,
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

        // CSV 파일을 저장할 위치 선택
        string path = EditorUtility.SaveFilePanel("Save CSV", "", "StageData.csv", "csv");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, sb.ToString());
            Debug.Log("CSV exported to: " + path);
        }
    }

    private void ImportCSV()
    {
        // CSV 파일 열기 대화상자 호출
        string path = EditorUtility.OpenFilePanel("Import CSV", "", "csv");
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

            // StageIndex를 key로 StageInfo를 저장할 딕셔너리
            Dictionary<int, StageInfo> stageDict = new Dictionary<int, StageInfo>();

            // 첫 줄은 헤더이므로 건너뜁니다.
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

                // CSV 컬럼 순서
                // 0: StageIndex
                // 1: StageDurationTime
                // 2: UnitIndex (불필요)
                // 3: SpawnCount
                // 4: FirstSpawnTime
                // 5: AdditionalSpawnCycle
                // 6: AdditionalSpawnCount
                // 7: UnderX2Threshold
                // 8: UnderX4Threshold
                // 9: MinimumSpawnCycle
                // 10: MaxAdditionalSpawns
                // 11: UnderX2Factor
                // 12: UnderX4Factor
                int stageIndex = int.Parse(columns[0]);
                int stageDurationTime = int.Parse(columns[1]);
                // int unitIndex = int.Parse(columns[2]); // 사용하지 않음
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

                // 해당 StageIndex의 StageInfo가 없으면 새로 생성
                if (!stageDict.TryGetValue(stageIndex, out StageInfo stageInfo))
                {
                    stageInfo = new StageInfo
                    {
                        durationTime = stageDurationTime,
                        stageUnitDatas = new List<StageUnitData>()
                    };
                    stageDict.Add(stageIndex, stageInfo);
                }
                else
                {
                    // 이미 존재하는 StageInfo가 있을 경우 durationTime이 다르다면 덮어씁니다.
                    if (stageInfo.durationTime != stageDurationTime)
                    {
                        stageInfo.durationTime = stageDurationTime;
                    }
                }

                // StageUnitData 생성 및 데이터 할당
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

            // StageIndex 순으로 정렬하여 StageInfo 리스트 생성
            List<int> keys = new List<int>(stageDict.Keys);
            keys.Sort();
            List<StageInfo> stageInfos = new List<StageInfo>();
            foreach (int key in keys)
            {
                stageInfos.Add(stageDict[key]);
            }

            // 불러온 데이터를 StageData에 할당
            StageData stageData = (StageData)target;
            stageData.stageInfos = stageInfos;

            EditorUtility.SetDirty(stageData);
            AssetDatabase.SaveAssets();

            Debug.Log("CSV imported successfully from: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogError("CSV import 실패: " + ex.Message);
        }
    }
}
#endif
