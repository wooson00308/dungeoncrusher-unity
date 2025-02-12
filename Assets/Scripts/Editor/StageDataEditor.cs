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
        // �⺻ �ν����� GUI ǥ��
        base.OnInspectorGUI();

        // CSV �������� ��ư �߰�
        if (GUILayout.Button("Export CSV"))
        {
            ExportCSV();
        }

        // CSV �ҷ����� ��ư �߰�
        if (GUILayout.Button("Import CSV"))
        {
            ImportCSV();
        }
    }

    private void ExportCSV()
    {
        StageData stageData = (StageData)target;
        StringBuilder sb = new StringBuilder();

        // CSV ��� (���ϴ� �׸� �°� ���� ����)
        sb.AppendLine("StageIndex,StageDurationTime,UnitIndex,SpawnCount,FirstSpawnTime,AdditionalSpawnCycle,AdditionalSpawnCount,UnderX2Threshold,UnderX4Threshold,MinimumSpawnCycle,MaxAdditionalSpawns,UnderX2Factor,UnderX4Factor");

        // StageData�� �� �������� ������ ��ȸ�ϸ� CSV ������ ����
        for (int stageIndex = 0; stageIndex < stageData.stageInfos.Count; stageIndex++)
        {
            StageInfo stageInfo = stageData.stageInfos[stageIndex];

            for (int unitIndex = 0; unitIndex < stageInfo.stageUnitDatas.Count; unitIndex++)
            {
                StageUnitData unitData = stageInfo.stageUnitDatas[unitIndex];

                // reductionFormula�� null�� ��� ����Ͽ� �⺻��(0) ó��
                float underX2Factor = unitData.reductionFormula != null ? unitData.reductionFormula.underX2Factor : 0;
                float underX4Factor = unitData.reductionFormula != null ? unitData.reductionFormula.underX4Factor : 0;

                // CSV�� �� �� ����
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

        // CSV ������ ������ ��ġ ����
        string path = EditorUtility.SaveFilePanel("Save CSV", "", "StageData.csv", "csv");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, sb.ToString());
            Debug.Log("CSV exported to: " + path);
        }
    }

    private void ImportCSV()
    {
        // CSV ���� ���� ��ȭ���� ȣ��
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
                Debug.LogWarning("CSV ���Ͽ� �����Ͱ� ���ų� ����� �ֽ��ϴ�.");
                return;
            }

            // StageIndex�� key�� StageInfo�� ������ ��ųʸ�
            Dictionary<int, StageInfo> stageDict = new Dictionary<int, StageInfo>();

            // ù ���� ����̹Ƿ� �ǳʶݴϴ�.
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] columns = lines[i].Split(',');
                if (columns.Length < 13)
                {
                    Debug.LogWarning("CSV ������ �÷� ���� �����մϴ�: " + lines[i]);
                    continue;
                }

                // CSV �÷� ����
                // 0: StageIndex
                // 1: StageDurationTime
                // 2: UnitIndex (���ʿ�)
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
                // int unitIndex = int.Parse(columns[2]); // ������� ����
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

                // �ش� StageIndex�� StageInfo�� ������ ���� ����
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
                    // �̹� �����ϴ� StageInfo�� ���� ��� durationTime�� �ٸ��ٸ� ����ϴ�.
                    if (stageInfo.durationTime != stageDurationTime)
                    {
                        stageInfo.durationTime = stageDurationTime;
                    }
                }

                // StageUnitData ���� �� ������ �Ҵ�
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

            // StageIndex ������ �����Ͽ� StageInfo ����Ʈ ����
            List<int> keys = new List<int>(stageDict.Keys);
            keys.Sort();
            List<StageInfo> stageInfos = new List<StageInfo>();
            foreach (int key in keys)
            {
                stageInfos.Add(stageDict[key]);
            }

            // �ҷ��� �����͸� StageData�� �Ҵ�
            StageData stageData = (StageData)target;
            stageData.stageInfos = stageInfos;

            EditorUtility.SetDirty(stageData);
            AssetDatabase.SaveAssets();

            Debug.Log("CSV imported successfully from: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogError("CSV import ����: " + ex.Message);
        }
    }
}
#endif
