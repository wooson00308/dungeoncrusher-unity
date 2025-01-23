using UnityEngine;

[CreateAssetMenu(fileName = "ExplodeSoulSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/ExplodeSoulSkillFxEventData")]
public class ExplodeSoulSkillFxEventData : SkillFxEventData
{
    [SerializeField] private ProjectileData explodeEffectPrefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        if (owner.Target == null) return;
        Debug.Log("Explode");
        var spawnPrefab = ResourceManager.Instance.Spawn(explodeEffectPrefab.Prefab.gameObject);
        spawnPrefab.transform.position = owner.Target.transform.position;
        var projectile = spawnPrefab.GetComponent<Projectile>();
        projectile.OnFire(owner, explodeEffectPrefab);
        spawnPrefab.transform.position = owner.Target.transform.position;
    }
}