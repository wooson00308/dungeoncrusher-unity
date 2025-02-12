using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[CreateAssetMenu(fileName = "CursedStepSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/CursedStepSkillFxEventData")]

public class CursedStepSkillFxEventData : SkillFxEventData
{
    [SerializeField] private ProjectileData _cursedStepPrefab;
    public static bool isProcessing = false;
    public static List< Unit> poisonUnit = new List<Unit>();
    private bool start = false;
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var spawnPrefab = ResourceManager.Instance.Spawn(_cursedStepPrefab.Prefab.gameObject);
        var projectile = spawnPrefab.GetComponent<Projectile>();
        projectile.OnFire(owner, _cursedStepPrefab);
        spawnPrefab.transform.position = owner.transform.position;
    }

    public async void ApplyPoisonDamage()
    {
        if (isProcessing) return; 

        isProcessing = true; 

        foreach (var list in poisonUnit.ToList())
        {
            if (!list.gameObject.activeSelf)
            {
                poisonUnit.Remove(list);
            }
            list?.OnHit(1);
        }

        await Awaitable.WaitForSecondsAsync(0.25f); 
        isProcessing = false; 
    }
}