#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProjectileDB))]
public class ProjectileDBEditor : DBEditor
{
    private SerializedProperty _projectileData;
    private ProjectileDB _projectileDB;

    private string _projectilePath = "Projectile";

    private void OnEnable()
    {
        _projectileData = serializedObject.FindProperty("ProjectileDatas");
        _projectileDB = target as ProjectileDB;

        RefreshData(_projectilePath);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var style = EditorStyles.helpBox;
        EditorGUILayout.BeginVertical(style);

        EditorGUILayout.PropertyField(_projectileData, true);

        EditorGUILayout.LabelField("제작하고 싶은 투사체 데이터 이름");
        _dataName = EditorGUILayout.TextField("", _dataName);

        _projectileDB.MoveSpeed = EditorGUILayout.FloatField("MoveSpeed", _projectileDB.MoveSpeed);
        _projectileDB.DetectRange = EditorGUILayout.FloatField("DetectRange", _projectileDB.DetectRange);

        CreateButton();

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("마지막 투사체 데이터 제거");
        DeleteButton();

        EditorGUILayout.EndVertical();
    }

    protected override void CreateButton()
    {
        if (GUILayout.Button("Create Projectile Data"))
        {
            if (_dataName == "")
            {
                _dataName = "DummyProjectileData";
            }

            var path = $"{resourcesPath}{_dataPath}{_projectilePath}/{_dataName}.asset";

            if (!AssetDatabase.AssetPathExists(path))
            {
                var createData = ScriptableObject.CreateInstance<ProjectileData>();

                createData.moveSpeed = _projectileDB.MoveSpeed;
                createData.detectRange = _projectileDB.DetectRange;

                _projectileDB.AddedProjectiles.Push(createData);

                AssetDatabase.CreateAsset(createData, path);
                RefreshData(_projectilePath);
            }
            else
            {
                Debug.LogWarning($"{_dataName}과 같은 이름의 데이터가 있습니다.");
            }
        }
    }

    protected override void DeleteButton()
    {
        if (GUILayout.Button("Delete Projectile Data"))
        {
            if (_projectileDB.AddedProjectiles.Count > 0)
            {
                AssetDatabase.DeleteAsset(GetAssetPath());
            }
            else
            {
                Debug.LogWarning("버튼으로 추가된 데이터가 없습니다.");
            }

            RefreshData(_projectilePath);
        }
    }

    protected override void RefreshData(string path)
    {
        var loadAllData = Resources.LoadAll($"{_dataPath}{path}");
        _projectileDB.ProjectileDatas = loadAllData.Select(data => data as ProjectileData).ToList();
        _projectileDB.ProjectileDatas = _projectileDB.ProjectileDatas.Where(data => data != null).ToList();
    }

    protected override string GetAssetPath()
    {
        string path = AssetDatabase.GetAssetPath(_projectileDB.AddedProjectiles.Pop());
        return path;
    }
}
#endif