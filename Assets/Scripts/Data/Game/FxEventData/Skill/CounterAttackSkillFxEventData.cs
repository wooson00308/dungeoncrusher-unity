using UnityEngine;

[CreateAssetMenu(fileName = "CounterAttackSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/CounterAttackSkillFxEventData")]
public class CounterAttackSkillFxEventData : SkillFxEventData
{
    [SerializeField] private int counterAttackPercent;
    [SerializeField] private GameObject prefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var spawnPrefab = ResourceManager.Instance.Spawn(prefab);

        if (owner.Target != null)
        {
            spawnPrefab.transform.position = owner.Target.transform.position;
        }

        var percent = counterAttackPercent / 100f;
        (int damage, bool isCritical) = owner.LastDamage();
        owner.Target.OnHit((int)(damage * percent), isCritical: isCritical);
    }
}