using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillDB))]
public class SkillDBEditor : Editor
{
    private SkillDB _skillDatabase;
    private string _newSkillName = "NewSkill";
    private Type _selectedType;
    private string[] _availableTypes;

    private Dictionary<SkillData, bool> _foldoutStates = new(); // 폴드아웃 상태 저장

    private const string SKILL_BASE_PATH = "Assets/Resources/Skill";
    private const string SKILL_PREFAB_PATH = SKILL_BASE_PATH + "/";
    private const string SKILL_FX_PREFAB_PATH = SKILL_BASE_PATH + "/Fx/Base/Prf_Skill_Fx_Base.prefab";
    private const string SKILL_FX_PATH = SKILL_BASE_PATH + "/Fx/";

    private void OnEnable()
    {
        _skillDatabase = (SkillDB)target;
        _availableTypes = GetSkillDataTypes();
        RefreshSkillDatabase();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Skill Save Path", EditorStyles.boldLabel);
        _skillDatabase.SkillSavePath = EditorGUILayout.TextField("Path", _skillDatabase.SkillSavePath);

        if (GUILayout.Button("Refresh Database"))
        {
            RefreshSkillDatabase();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add New Skill", EditorStyles.boldLabel);

        _newSkillName = EditorGUILayout.TextField("Skill Name", _newSkillName);

        int selectedIndex = ArrayUtility.IndexOf(_availableTypes, _selectedType?.Name);
        selectedIndex = EditorGUILayout.Popup("Skill Type", selectedIndex, _availableTypes);
        if (selectedIndex >= 0 && selectedIndex < _availableTypes.Length)
        {
            _selectedType = GetTypeByName(_availableTypes[selectedIndex]);
        }

        if (GUILayout.Button("Create Skill"))
        {
            CreateSkillData(_newSkillName);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Registered Skills", EditorStyles.boldLabel);

        foreach (var skillData in _skillDatabase.SkillDatas)
        {
            if (!_foldoutStates.ContainsKey(skillData))
                _foldoutStates[skillData] = false;

            _foldoutStates[skillData] = EditorGUILayout.Foldout(_foldoutStates[skillData], skillData.name, true);

            if (_foldoutStates[skillData])
            {
                EditorGUI.indentLevel++;

                // 스킬 데이터 표시
                EditorGUILayout.ObjectField("Skill Data", skillData, typeof(SkillData), false);

                // Skill Prefab 정보 표시
                EditorGUILayout.ObjectField("Skill Prefab", skillData.Prefab, typeof(GameObject), false);

                // Skill FX Prefab 정보 표시
                if (skillData.SkillLevelDatas != null)
                {
                    for (int i = 0; i < skillData.SkillLevelDatas.Count; i++)
                    {
                        EditorGUILayout.ObjectField($"Skill FX Prefab (Level {i + 1})",
                            skillData.SkillLevelDatas[i].skillFxPrefab, typeof(GameObject), false);
                    }
                }

                // 제거 버튼
                if (GUILayout.Button("Remove Skill"))
                {
                    RemoveSkillData(skillData);
                    break; // 삭제 후 목록이 변경되므로 루프 종료
                }

                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }


    private void RefreshSkillDatabase()
    {
        _skillDatabase.SkillDatas.Clear();

        string path = _skillDatabase.SkillSavePath;
        if (!Directory.Exists(path))
        {
            Debug.LogWarning($"Path {path} does not exist.");
            return;
        }

        string[] assetFiles = Directory.GetFiles(path, "*.asset");
        foreach (string file in assetFiles)
        {
            SkillData skillData = AssetDatabase.LoadAssetAtPath<SkillData>(file);
            if (skillData != null)
            {
                _skillDatabase.AddSkillData(skillData);
            }
        }

        Debug.Log("Skill database refreshed.");
    }

    private void CreateSkillData(string skillName)
    {
        if (_selectedType == null || string.IsNullOrEmpty(skillName))
        {
            Debug.LogWarning("Skill Type or Name is invalid.");
            return;
        }

        // 중복 이름 체크
        foreach (var skillData in _skillDatabase.SkillDatas)
        {
            if (skillData.name == skillName)
            {
                Debug.LogWarning($"Skill with the name '{skillName}' already exists in the database.");
                return; // 중복 이름 방어
            }
        }

        string directoryPath = _skillDatabase.SkillSavePath;
        string savePath = $"{directoryPath}/{skillName}.asset";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"Directory created at: {directoryPath}");
        }

        SkillData newSkillData = (SkillData)ScriptableObject.CreateInstance(_selectedType);
        newSkillData.name = skillName;

        AssetDatabase.CreateAsset(newSkillData, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        _skillDatabase.AddSkillData(newSkillData);

        CreateSkillPrefab(skillName, newSkillData);
        CreateSkillFxPrefabs(skillName, newSkillData);

        Debug.Log($"Skill {skillName} created and saved to {savePath}");
    }


    private GameObject CreateSkillPrefab(string skillName, SkillData skillData)
    {
        GameObject skillPrefab = new(skillName);
        Skill skillComponent = skillPrefab.AddComponent<Skill>();
        skillComponent.SetSkillData(skillData);

        string prefabPath = SKILL_PREFAB_PATH + $"{skillName}.prefab";
        string directoryPath = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        skillData.Prefab = PrefabUtility.SaveAsPrefabAsset(skillPrefab, prefabPath);
        GameObject.DestroyImmediate(skillPrefab);

        Debug.Log($"Skill prefab created at: {prefabPath}");
        return skillPrefab;
    }

    private void CreateSkillFxPrefabs(string skillName, SkillData skillData)
    {
        GameObject fxBase = AssetDatabase.LoadAssetAtPath<GameObject>(SKILL_FX_PREFAB_PATH);
        if (fxBase == null)
        {
            Debug.LogError($"Base FX prefab not found at {SKILL_FX_PREFAB_PATH}");
            return;
        }

        string fxPrefabName = $"Prf_Skill_Fx_{skillName}";
        string fxPrefabPath = SKILL_FX_PATH + $"{fxPrefabName}.prefab";

        GameObject fxPrefab = PrefabUtility.InstantiatePrefab(fxBase) as GameObject;
        var skillFxPrefab = PrefabUtility.SaveAsPrefabAsset(fxPrefab, fxPrefabPath);

        Debug.Log($"Skill FX prefab created at: {fxPrefabPath}");

        skillData.SkillLevelDatas = new List<SkillLevelData>();

        for (int i = 0; i < 3; i++)
        {
            fxPrefab.name = fxPrefabName;

            var skillLevelData = new SkillLevelData
            {
                activationChance = 50,
                skillValue = 100,
                skillFxPrefab = skillFxPrefab
            };
            skillData.SkillLevelDatas.Add(skillLevelData);
        }

        GameObject.DestroyImmediate(fxPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RemoveSkillData(SkillData skillData)
    {
        if (skillData == null)
        {
            Debug.LogWarning("SkillData is null.");
            return;
        }

        // 데이터베이스에서 스킬 데이터 제거
        _skillDatabase.RemoveSkillData(skillData);

        // 스킬 데이터 파일 삭제
        string assetPath = AssetDatabase.GetAssetPath(skillData);
        if (!string.IsNullOrEmpty(assetPath))
        {
            AssetDatabase.DeleteAsset(assetPath);
            Debug.Log($"Skill data asset removed: {assetPath}");
        }
        else
        {
            Debug.LogWarning($"Could not find asset path for skill data.");
        }

        // 스킬 데이터에 연결된 프리팹 삭제
        RemovePrefabsFromSkillData(skillData);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RemovePrefabsFromSkillData(SkillData skillData)
    {
        // Skill Prefab 삭제
        if (skillData.Prefab != null)
        {
            string prefabPath = AssetDatabase.GetAssetPath(skillData.Prefab);
            if (!string.IsNullOrEmpty(prefabPath))
            {
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log($"Skill prefab removed: {prefabPath}");
            }
        }

        // Skill FX Prefab 삭제 (레벨 데이터에 연결된 프리팹들)
        foreach (var levelData in skillData.SkillLevelDatas)
        {
            if (levelData.skillFxPrefab != null)
            {
                string fxPrefabPath = AssetDatabase.GetAssetPath(levelData.skillFxPrefab);
                if (!string.IsNullOrEmpty(fxPrefabPath))
                {
                    AssetDatabase.DeleteAsset(fxPrefabPath);
                    Debug.Log($"Skill FX prefab removed: {fxPrefabPath}");
                }
            }
        }
    }


    private string[] GetSkillDataTypes()
    {
        var skillDataType = typeof(SkillData);
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
