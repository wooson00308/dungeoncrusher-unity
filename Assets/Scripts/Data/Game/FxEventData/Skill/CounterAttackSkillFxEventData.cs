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
        
        if (owner.Attacker != null)
        {
            spawnPrefab.transform.position = owner.Attacker.transform.position;
        }

        var percent = counterAttackPercent / 100f;
        owner.Target.OnHit((int)(owner.Attack.Value * percent));
    }
}