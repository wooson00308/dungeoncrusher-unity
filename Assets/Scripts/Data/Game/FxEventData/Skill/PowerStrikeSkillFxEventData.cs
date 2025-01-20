using UnityEngine;

[CreateAssetMenu(fileName = "PowerStrikeSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/PowerStrikeSkillFxEventData")]
public class PowerStrikeSkillFxEventData : SkillFxEventData
{
    [SerializeField] private ProjectileData powerStrikePrefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var spawnPrefab = ResourceManager.Instance.Spawn(powerStrikePrefab.Prefab.gameObject);
        var projectile = spawnPrefab.GetComponent<Projectile>();
        projectile.OnFire(owner, powerStrikePrefab);
        spawnPrefab.transform.position = owner.Target.transform.position;
    }
}