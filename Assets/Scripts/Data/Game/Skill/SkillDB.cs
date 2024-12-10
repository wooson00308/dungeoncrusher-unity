using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDB", menuName = "SkillData/Create Skill DB")]
public class SkillDB : ScriptableObject
{
    [Tooltip("스킬 데이터 저장 경로")]
    [SerializeField] private string _skillSavePath = "Assets/Resources/Data/Skill";

    [Tooltip("등록된 스킬 데이터 목록")]
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
