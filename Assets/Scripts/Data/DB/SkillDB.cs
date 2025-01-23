using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SkillDB", menuName = "Data/DB/SkillDB")]
public class SkillDB : ScriptableObject
{
    public bool IsRateSkill = false;
    public List<SkillData> SkillDatas = new();
    public Stack<SkillData> AddedSkillDatas = new();
    public Stack<ConditionData> AddedConditionDatas = new();
}