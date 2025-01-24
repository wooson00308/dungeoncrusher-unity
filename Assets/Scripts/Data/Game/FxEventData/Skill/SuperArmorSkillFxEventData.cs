using UnityEngine;

[CreateAssetMenu(fileName = "SuperArmorSkillFxEventData",
    menuName = "Data/SkillData/FxEventData/SuperArmorSkillFxEventData")]
public class SuperArmorSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.IsSuperArmor = true;
        var spawnPrefab = ResourceManager.Instance.Spawn(Prefab.gameObject);
        spawnPrefab.transform.SetParent(owner.Model.transform);
        spawnPrefab.transform.position = owner.Model.transform.position;
    }

    public override void OnEndEvent(Unit owner, object args = null)
    {
        owner.IsSuperArmor = false;
    }
}