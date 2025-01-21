using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDB_old", menuName = "SkillData/Create SkillDB_old")]
public class SkillDB_old : ScriptableObject
{
    [Tooltip("스킬 데이터 저장 경로")]
    [SerializeField] private string _skillSavePath = "Assets/Resources/Data/Skill/old";

    [Tooltip("등록된 스킬 데이터 목록")]
    [SerializeField] private List<SkillData_old> _skillDatas = new List<SkillData_old>();

    public string SkillSavePath 
    {
        get { return _skillSavePath; }
        set { _skillSavePath = value; }
    }
    public List<SkillData_old> SkillDatas => _skillDatas;

    public void AddSkillData(SkillData_old skillData)
    {
        if (!_skillDatas.Contains(skillData))
        {
            _skillDatas.Add(skillData);
        }
    }

    public void RemoveSkillData(SkillData_old skillData)
    {
        if (_skillDatas.Contains(skillData))
        {
            _skillDatas.Remove(skillData);
        }
    }
}
