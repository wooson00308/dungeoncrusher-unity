using UnityEngine;

[CreateAssetMenu(fileName = "GreiasWrathSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/GreiasWrathSkillFxEventData")]
public class GreiasWrathSkillFxEventData : SkillFxEventData
{
    [SerializeField] private ProjectileData _prefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        Debug.Log("GreiasWrathSkillFxEventData");
        var spawnPrefab = ResourceManager.Instance.Spawn(_prefab.Prefab.gameObject);
        var projectile = spawnPrefab.GetComponent<Projectile>();
        projectile.OnFire(owner, _prefab);
        if (owner.Target == null) return;
        spawnPrefab.transform.position = owner.Target.transform.position;
    }
}