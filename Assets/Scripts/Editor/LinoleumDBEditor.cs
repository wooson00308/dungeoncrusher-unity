#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LinoleumDB))]
public class LinoleumDBEditor : DBEditor
{
    private SerializedProperty _linoleumDatas;
    private LinoleumDB _linoleumDB;

    private string _linoleumPath = "Linoleum";

    private void OnEnable()
    {
        _linoleumDatas = serializedObject.FindProperty("LinoleumDatas");
        _linoleumDB = target as LinoleumDB;

        RefreshData(_linoleumPath);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var style = EditorStyles.helpBox;
        EditorGUILayout.BeginVertical(style);

        EditorGUILayout.PropertyField(_linoleumDatas, true);

        EditorGUILayout.LabelField("제작하고 싶은 장판 데이터 이름");
        _dataName = EditorGUILayout.TextField("", _dataName);

        _linoleumDB.TickDamagePercent = EditorGUILayout.FloatField("TickDamagePercent", _linoleumDB.TickDamagePercent);
        _linoleumDB.TickInterval = EditorGUILayout.FloatField("TickInterval", _linoleumDB.TickInterval);
        _linoleumDB.DetectRange = EditorGUILayout.FloatField("DetectRange", _linoleumDB.DetectRange);

        CreateButton();

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("마지막 투사체 데이터 제거");
        DeleteButton();

        EditorGUILayout.EndVertical();
    }

    protected override void CreateButton()
    {
        if (GUILayout.Button("Create Linoleum Data"))
        {
            if (_dataName == "")
            {
                _dataName = "DummyLinoleumData";
            }

            var path = $"{resourcesPath}{_dataPath}{_linoleumPath}/{_dataName}.asset";

            if (!AssetDatabase.AssetPathExists(path))
            {
                var createData = ScriptableObject.CreateInstance<LinoleumData>();

                createData.tickDamagePercent = _linoleumDB.TickDamagePercent;
                createData.tickInterval = _linoleumDB.TickInterval;
                createData.detectRange = _linoleumDB.DetectRange;

                _linoleumDB.AddedLinoleum.Push(createData);

                AssetDatabase.CreateAsset(createData, path);
                RefreshData(_linoleumPath);
            }
            else
            {
                Debug.LogWarning($"{_dataName}과 같은 이름의 데이터가 있습니다.");
            }
        }
    }

    protected override void DeleteButton()
    {
        if (GUILayout.Button("Delete Linoleum Data"))
        {
            if (_linoleumDB.AddedLinoleum.Count > 0)
            {
                AssetDatabase.DeleteAsset(GetAssetPath());
            }
            else
            {
                Debug.LogWarning("버튼으로 추가된 데이터가 없습니다.");
            }

            RefreshData(_linoleumPath);
        }
    }

    protected override void RefreshData(string path)
    {
        var loadAllData = Resources.LoadAll($"{_dataPath}{path}");
        _linoleumDB.LinoleumDatas = loadAllData.Select(data => data as LinoleumData).ToList();
        _linoleumDB.LinoleumDatas = _linoleumDB.LinoleumDatas.Where(data => data != null).ToList();
    }

    protected override string GetAssetPath()
    {
        string path = AssetDatabase.GetAssetPath(_linoleumDB.AddedLinoleum.Pop());
        return path;
    }
}
#endif