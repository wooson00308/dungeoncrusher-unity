using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skill_981", menuName = "SkillData/Create Dash")]
public class DashData : SkillData
{
    [Header("Dash Config")]
    [SerializeField] private float _dashRange;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _additionalDistance;
    public float DashRange => _dashRange;
    public float DashSpeed => _dashSpeed;
    public float AdditionalDistance => _additionalDistance;

    public override bool IsValidTarget(Unit unit)
    {
        if (unit.Target == null) 
            return false;
        
        if(Vector3.Distance(unit.transform.position, unit.Target.transform.position) > _dashRange)
        {
            return false;
        }

        return true;
    }

    public override void OnAction(Skill skill, Unit user, List<Unit> targets)
    {
        user.DashToTarget(this, () =>
        {
            var target = user.Target;

            var damage = GetSkillLevelData(skill.Level).skillValue;
            user.Target.OnHit((int)damage, user);

            if(skill.Level > 1)
            {
                if (target.IsDeath)
                {
                    // 쿨타임 초기화
                    skill.ResetCooltime();
                }

                if(skill.Level > 2)
                {
                    if(target != null)
                    {
                        target.OnAerial();
                        // 무력화 효과
                    } 
                }
            }
        });
    }
}
