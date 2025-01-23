#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillDB))]
public class SkillDBEditor : DBEditor
{
    private SerializedProperty _skillDatas;
    private SkillDB _skillDB;

    private string _skillPath = "Skill";

    private void OnEnable()
    {
        _skillDatas = serializedObject.FindProperty("SkillDatas");
        _skillDB = target as SkillDB;

        RefreshData(_skillPath);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var style = EditorStyles.helpBox;
        EditorGUILayout.BeginVertical(style);

        EditorGUILayout.PropertyField(_skillDatas, true);

        EditorGUILayout.LabelField("제작하고 싶은 스킬 데이터 이름");
        _dataName = EditorGUILayout.TextField("", _dataName);

        _skillDB.IsRateSkill = EditorGUILayout.Toggle("IsRateSkill", _skillDB.IsRateSkill);

        CreateButton();

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("마지막 스킬 데이터 제거");
        DeleteButton();

        EditorGUILayout.EndVertical();
    }

    protected override void CreateButton()
    {
        if (GUILayout.Button("Create Skill Data"))
        {
            if (_dataName == "")
            {
                _dataName = "DummySkill";
            }

            var skillDataPath = $"{resourcesPath}{_dataPath}{_skillPath}/SkillData/{_dataName}Data.asset";
            string conditionDataPath;

            if (!AssetDatabase.AssetPathExists(skillDataPath))
            {
                var createData = CreateInstance<SkillData>();

                ConditionData conditionData;

                if (_skillDB.IsRateSkill)
                {
                    conditionData = CreateInstance<UnitEventRateConditionData>();
                    conditionDataPath =
                        $"{resourcesPath}{_dataPath}{_skillPath}/Condition/{_dataName}RateCondition.asset";
                }
                else
                {
                    conditionData = CreateInstance<UnitEventConditionData>();
                    conditionDataPath = $"{resourcesPath}{_dataPath}{_skillPath}/Condition/{_dataName}Condition.asset";
                }

                AssetDatabase.CreateAsset(conditionData, conditionDataPath);
                AssetDatabase.CreateAsset(createData, skillDataPath);

                var skillLevelData = new SkillLevelData();
                createData.LevelDatas.Add(skillLevelData);
                skillLevelData.Conditions.Add(conditionData);

                _skillDB.AddedSkillDatas.Push(createData);
                _skillDB.AddedConditionDatas.Push(conditionData);

                RefreshData(_skillPath);
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
            if (_skillDB.AddedSkillDatas.Count > 0)
            {
                AssetDatabase.DeleteAsset(GetAssetPath());
                AssetDatabase.DeleteAsset(GetConditionPath());
            }
            else
            {
                Debug.LogWarning("버튼으로 추가된 데이터가 없습니다.");
            }

            RefreshData(_skillPath);
        }
    }

    protected override void RefreshData(string path)
    {
        var loadAllData = Resources.LoadAll($"{_dataPath}{path}");
        _skillDB.SkillDatas = loadAllData.Select(data => data as SkillData).ToList();
        _skillDB.SkillDatas = _skillDB.SkillDatas.Where(data => data != null).ToList();
    }

    protected override string GetAssetPath()
    {
        string path = AssetDatabase.GetAssetPath(_skillDB.AddedSkillDatas.Pop());
        return path;
    }

    private string GetConditionPath()
    {
        string path = AssetDatabase.GetAssetPath(_skillDB.AddedConditionDatas.Pop());
        return path;
    }
}
#endif