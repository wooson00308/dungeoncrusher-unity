using UnityEngine;

[CreateAssetMenu(fileName = "ExtraSkillFxEventData", menuName = "Scriptable Objects/ExtraSkillFxEventData")]
public class ExtraSkillFxEventData : SkillFxEventData
{
    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var fx = ResourceManager.Instance.Spawn(Prefab.gameObject);

        var triggers = fx.GetComponentsInChildren<TriggerFx>();
        foreach (var trigger in triggers)
        {
            trigger.Initialized(owner);
        }

        fx.transform.position = owner.transform.position;
        fx.transform.rotation = owner.transform.rotation;
    }
}
