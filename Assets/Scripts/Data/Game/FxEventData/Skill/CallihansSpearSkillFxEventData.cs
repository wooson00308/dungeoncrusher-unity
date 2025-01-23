using UnityEngine;

[CreateAssetMenu(fileName = "CallihansSpearSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/CallihansSpearSkillFxEventData")]
public class CallihansSpearSkillFxEventData : SkillFxEventData
{
    [SerializeField] private ProjectileData _callihansSpearPrefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var spawnPrefab = ResourceManager.Instance.Spawn(_callihansSpearPrefab.Prefab.gameObject);
        var projectile = spawnPrefab.GetComponent<Projectile>();
        projectile.OnFire(owner, _callihansSpearPrefab);
        spawnPrefab.transform.position = owner.Target.transform.position;
    }
}