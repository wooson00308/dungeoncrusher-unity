using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

[CreateAssetMenu(fileName = "LightingSpearSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/LightingSpearSkillFxEventData")]
public class LightingSpearSkillFxEventData : SkillFxEventData
{
    [SerializeField] private ProjectileData _lightingSpearPrefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var spawnPrefab = ResourceManager.Instance.Spawn(_lightingSpearPrefab.Prefab.gameObject);
        var projectile = spawnPrefab.GetComponent<Projectile>();
        projectile.OnFire(owner, _lightingSpearPrefab);
        spawnPrefab.transform.position = owner.Target.transform.position;
        //unit.OnHit(owner.Attack.Value /*마력으로*/, owner);
    }

}