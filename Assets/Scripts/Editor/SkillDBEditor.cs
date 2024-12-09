using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillDB))]
public class SkillDBEditor : Editor
{
    private SkillDB skillDatabase;
    private string newSkillName = "NewSkill";
    private System.Type selectedType;
    private string[] availableTypes;

    private void OnEnable()
    {
        skillDatabase = (SkillDB)target;
        availableTypes = GetSkillDataTypes();
        RefreshSkillDatabase();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ��ų ���� ��� �ʵ�
        EditorGUILayout.LabelField("Skill Save Path", EditorStyles.boldLabel);
        skillDatabase.SkillSavePath = EditorGUILayout.TextField("Path", skillDatabase.SkillSavePath);

        if (GUILayout.Button("Refresh Database"))
        {
            RefreshSkillDatabase();
        }

        // ��ų ���� UI
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add New Skill", EditorStyles.boldLabel);

        newSkillName = EditorGUILayout.TextField("Skill Name", newSkillName);

        // ��ӹ��� ��ų ������ Ÿ�� ����
        int selectedIndex = ArrayUtility.IndexOf(availableTypes, selectedType?.Name);
        selectedIndex = EditorGUILayout.Popup("Skill Type", selectedIndex, availableTypes);
        if (selectedIndex >= 0 && selectedIndex < availableTypes.Length)
        {
            selectedType = GetTypeByName(availableTypes[selectedIndex]);
        }

        if (GUILayout.Button("Create Skill"))
        {
            CreateSkillData(newSkillName);
        }

        EditorGUILayout.Space();

        // ��ų ������ ����Ʈ ǥ�� �� ���� ��ư �߰�
        EditorGUILayout.LabelField("Registered Skills", EditorStyles.boldLabel);
        for (int i = 0; i < skillDatabase.SkillDatas.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField($"Skill {i + 1}", skillDatabase.SkillDatas[i], typeof(SkillData), false);

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                RemoveSkillData(skillDatabase.SkillDatas[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void RefreshSkillDatabase()
    {
        skillDatabase.SkillDatas.Clear();

        // ������ ��ų ������ ���� �˻�
        string path = skillDatabase.SkillSavePath;
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
                skillDatabase.AddSkillData(skillData);
            }
        }

        Debug.Log("Skill database refreshed.");
    }

    private void CreateSkillData(string skillName)
    {
        if (selectedType == null || string.IsNullOrEmpty(skillName))
        {
            Debug.LogWarning("Skill Type or Name is invalid.");
            return;
        }

        // ��ų ���� ��ο� ���� �̸� ����
        string directoryPath = skillDatabase.SkillSavePath;
        string savePath = $"{directoryPath}/{skillName}.asset";

        // ���丮 ���� ���� Ȯ�� �� ����
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"Directory created at: {directoryPath}");
        }

        // ���ο� ��ũ���ͺ� ������Ʈ ����
        SkillData newSkill = (SkillData)ScriptableObject.CreateInstance(selectedType);
        newSkill.name = skillName;

        // ������ ��ο� ����
        AssetDatabase.CreateAsset(newSkill, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // �����ͺ��̽��� �߰�
        skillDatabase.AddSkillData(newSkill);

        Debug.Log($"Skill {skillName} created and saved to {savePath}");
    }

    private void RemoveSkillData(SkillData skillData)
    {
        if (skillData == null)
        {
            Debug.LogWarning("SkillData is null.");
            return;
        }

        // �����ͺ��̽����� ����
        skillDatabase.RemoveSkillData(skillData);

        // ���� ����
        string assetPath = AssetDatabase.GetAssetPath(skillData);
        if (!string.IsNullOrEmpty(assetPath))
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Skill {skillData.name} removed from database and deleted.");
        }
        else
        {
            Debug.LogWarning($"Could not find asset path for {skillData.name}.");
        }
    }

    private string[] GetSkillDataTypes()
    {
        var skillDataType = typeof(SkillData);
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
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

    private System.Type GetTypeByName(string typeName)
    {
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
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
