
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skill_1116", menuName = "SkillData/Create FireballSkill")]
public class FireballSkillData : SkillData
{
    public override bool IsValidTarget(Unit unit)
    {
        return true;
    }

    public override void OnAction(Skill skill, Unit user, List<Unit> targets)
    {
        //HashSet<Unit> enemies = UnitFactory.Instance.GetUnitsExcludingTeam(user.Team);
        //var target = enemies.OrderBy(x => Random.value).Take(1).ToList();
        //List<Unit> _targets = enemies
        //        .Where(enemy => Vector3.Distance(enemy.transform.position, target[0].transform.position) <=
        //                        skill.SkillData.GetSkillLevelData(skill.Level).range)
        //        .OrderBy(x => Random.value)
        //        .Take(skill.SkillData.GetSkillLevelData(skill.Level).targetNum)
        //        .ToList();
        float skillValue = GetSkillLevelData(skill.Level).skillValue;
        int damage = (int)(user.Attack.Value * skillValue * 0.01f);
        Visualizer.Instance.ShowRange(targets[0].transform.position, GetSkillLevelData(skill.Level).range);
        foreach (var _target in targets)
        {
            _target?.OnHit((int)damage, user);
            if (skill.Level > 1)
            {
                if (!_target.IsStun)
                {
                    if (Random.Range(1, 100) <= 20)
                        _target.OnStun();
                }
            }
            if (skill.Level > 2)
            {
                if (!_target.IsStun)
                {
                    _target?.OnStun();
                }
            }
        }
    }
}