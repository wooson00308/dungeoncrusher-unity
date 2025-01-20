using UnityEngine;

[CreateAssetMenu(fileName = "SuperArmorSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SuperArmorSkillFxEventData")]
public class SuperArmorSkillFxEventData : SkillFxEventData
{
    public GameObject Prefab;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        owner.IsSuperArmor = true;
        ResourceManager.Instance.Spawn(Prefab);
    }

    public override void OnEndEvent(Unit owner, object args = null)
    {
        owner.IsSuperArmor = false;
    }
}