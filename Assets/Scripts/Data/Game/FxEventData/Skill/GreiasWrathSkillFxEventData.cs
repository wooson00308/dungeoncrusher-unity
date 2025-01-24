using UnityEngine;

[CreateAssetMenu(fileName = "GreiasWrathSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/GreiasWrathSkillFxEventData")]
public class GreiasWrathSkillFxEventData : SkillFxEventData
{
    [SerializeField] private ProjectileData _prefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        // Debug.Log("GreiasWrathSkillFxEventData");
        if (owner.Target == null) return;
        var spawnPrefab = ResourceManager.Instance.Spawn(_prefab.Prefab.gameObject);
        var projectile = spawnPrefab.GetComponent<Projectile>();
        projectile.OnFire(owner, _prefab);
        spawnPrefab.transform.position = owner.Target.transform.position;
    }
}                                            