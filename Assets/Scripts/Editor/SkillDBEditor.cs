#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillDB))]
public class SkillDBEditor : DBEditor
{
    private SerializedProperty _skillDatas;
    private SkillDB _skillDB;
    private Type _selectedConditionType;
    private Type _selectedFxEventType;
    private string[] _availableConditionTypes;
    private string[] _availableFxEventTypes;

    private string _skillPath = "Skill";

    private void OnEnable()
    {
        _skillDatas = serializedObject.FindProperty("SkillDatas");
        _skillDB = target as SkillDB;
        _availableConditionTypes = GetConditionTypes();
        _availableFxEventTypes = GetFxEventTypes();
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

        int selectedConditionIndex = ArrayUtility.IndexOf(_availableConditionTypes, _selectedConditionType?.Name);
        selectedConditionIndex =
            EditorGUILayout.Popup("Condition Type", selectedConditionIndex, _availableConditionTypes);
        if (selectedConditionIndex >= 0 && selectedConditionIndex < _availableConditionTypes.Length)
        {
            _selectedConditionType = GetTypeByName(_availableConditionTypes[selectedConditionIndex]);
        }

        int selectedFxEventIndex = ArrayUtility.IndexOf(_availableFxEventTypes, _selectedFxEventType?.Name);
        selectedFxEventIndex = EditorGUILayout.Popup("FxEvent Type", selectedFxEventIndex, _availableFxEventTypes);
        if (selectedFxEventIndex >= 0 && selectedFxEventIndex < _availableFxEventTypes.Length)
        {
            _selectedFxEventType = GetTypeByName(_availableFxEventTypes[selectedFxEventIndex]);
        }

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
            string fxEventDataPath;

            if (!AssetDatabase.AssetPathExists(skillDataPath))
            {
                var createData = CreateInstance<SkillData>();

                ConditionData conditionData;
                conditionData = (ConditionData)CreateInstance(_selectedConditionType);
                conditionDataPath =
                    $"{resourcesPath}{_dataPath}{_skillPath}/Condition/{_dataName}ConditionData.asset";

                SkillFxEventData fxEventData;
                fxEventData = (SkillFxEventData)CreateInstance(_selectedFxEventType);
                fxEventDataPath =
                    $"{resourcesPath}{_dataPath}{_skillPath}/FxEvent/Skill/{_dataName}FxEventData.asset";

                AssetDatabase.CreateAsset(conditionData, conditionDataPath);
                AssetDatabase.CreateAsset(fxEventData, fxEventDataPath);
                AssetDatabase.CreateAsset(createData, skillDataPath);

                var skillLevelData = new SkillLevelData();
                createData.LevelDatas.Add(skillLevelData);
                skillLevelData.Conditions.Add(conditionData);
                skillLevelData.UseSkillFxDatas.Add(fxEventData);

                _skillDB.AddedSkillDatas.Push(createData);
                _skillDB.AddedConditionDatas.Push(conditionData);
                _skillDB.AddedFxEventDatas.Push(fxEventData);

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

    private string[] GetConditionTypes()
    {
        var skillDataType = typeof(UnitEventConditionData);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = new List<string>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(skillDataType) && !type.IsAbstract)
                {
                    types.Add(type.Name);
                }
            }
        }

        return types.ToArray();
    }

    private string[] GetFxEventTypes()
    {
        var skillDataType = typeof(SkillFxEventData);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = new List<string>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(skillDataType) && !type.IsAbstract)
                {
                    types.Add(type.Name);
                }
            }
        }

        return types.ToArray();
    }

    private Type GetTypeByName(string typeName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }
}
#endif