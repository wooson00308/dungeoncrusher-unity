using UnityEngine;

[CreateAssetMenu(fileName = "DashSkillFxEventData", menuName = "Data/SkillData/FxEventData/DashSkillFxEventData")]
public class DashSkillFxEventData : SkillFxEventData
{
    public float DashSpeed;
    public float AdditionalDistance;
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        // �뽬 ��ų ����
        var skillLevelData = skill.CurrentLevelData;

        owner.DashToTarget(this, () =>
        {
            var target = owner.Target;

            var damage = skillLevelData.ADRatio;
            target?.OnHit((int)damage, owner);
        });
    }
}
