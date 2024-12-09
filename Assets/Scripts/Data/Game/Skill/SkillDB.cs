using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDB", menuName = "SkillData/Create Skill DB")]
public class SkillDB : ScriptableObject
{
    [Tooltip("��ų ������ ���� ���")]
    [SerializeField] private string _skillSavePath = "Assets/Resources/Data/Skill";

    [Tooltip("��ϵ� ��ų ������ ���")]
    [SerializeField] private List<SkillData> _skillDatas = new List<SkillData>();

    public string SkillSavePath 
    {
        get { return _skillSavePath; }
        set { _skillSavePath = value; }
    }
    public List<SkillData> SkillDatas => _skillDatas;

    public void AddSkillData(SkillData skillData)
    {
        if (!_skillDatas.Contains(skillData))
        {
            _skillDatas.Add(skillData);
        }
    }

    public void RemoveSkillData(SkillData skillData)
    {
        if (_skillDatas.Contains(skillData))
        {
            _skillDatas.Remove(skillData);
        }
    }
}
