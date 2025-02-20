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

        if (GUILayout.Button("CSV ��������"))
        {
            ExportCSV();
        }

        if (GUILayout.Button("CSV �ҷ�����"))
        {
            ImportCSV();
        }
    }

    private void ExportCSV()
    {
        StageData stageData = (StageData)target;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("�������� ��ȣ,���� ��ȣ,�������� ���� �ð� (��),���� �� �����Ǵ� �⺻ ���� ��,ù ���� Ÿ�� (ms),�߰� ���� �ֱ� (��),�߰� ���� ���� ��,Under_x2 ���� �� (X2),Under_x4 ���� �� (X4),�߰� ���� �ֱ� �ּҰ� (MinX),�߰� ���� �ִ� Ƚ�� (-1: ������ | 0: ���� | n: ���� Ƚ��),Under_x2 ���� ���� (��2),Under_x4 ���� ���� (��4)");

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

        string path = EditorUtility.SaveFilePanel("CSV ���� ����", "", "StageData.csv", "csv");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
            ShowNotification("CSV �������� �Ϸ�\nCSV ������ ���� ��ο� ����Ǿ����ϴ�:\n" + path);
        }
    }

    private void ImportCSV()
    {
        string path = EditorUtility.OpenFilePanel("CSV ���� �ҷ�����", "", "csv");
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

            Dictionary<int, StageInfo> stageDict = new Dictionary<int, StageInfo>();
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

            ShowNotification("CSV �ҷ����� �Ϸ�\nCSV ������ ���������� �ҷ��������ϴ�:\n" + path);
        }
        catch (Exception ex)
        {
            ShowNotification("CSV �ҷ����� ����\n" + ex.Message);
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
