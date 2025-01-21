using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportShotSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SupportShotSkillFxEventData")]
public class SupportShotSkillFxEventData : SkillFxEventData
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 offset;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var spawnPrefab = ResourceManager.Instance.Spawn(prefab);
        spawnPrefab.transform.position = owner.transform.position + (Vector3)offset;
        //spawnPrefab.transform.rotation = owner.transform.rotation;
        
        var target = owner.Target;
        var damage = owner.Attack.Value * skill.CurrentLevelData.ADRatio;

        if (target != null)
        {
            var dir = target.transform.position - owner.transform.position;
            dir.Normalize();
            var yRot = dir.x > 0 ? 0 : 180;
            var rotation = Quaternion.Euler(0, yRot, 0);
            spawnPrefab.transform.rotation = rotation;
        }
        // if (target != null)
        // {
        if (target?.Health.Max * 0.3f > target?.Health.Value)
        {
            target?.OnDeath(owner, true);
            // target?.OnHit(target.Health.Value, user);
            return;
        }

        if (damage >= target.Health.Value)
        {
            target?.OnDeath(owner, true);
        }

        target?.OnHit(damage, owner);
        // }
    }
}