using UnityEngine;

[CreateAssetMenu(fileName = "SkillSpawnEventData", menuName = "Data/SkillData/FxEventData/SkillSpawnFxEventData")]
public class SkillSpawnFxEventData : SkillFxEventData
{
    [SerializeField] private Vector3 offSet;
    [SerializeField] private bool targetPos;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var fx = ResourceManager.Instance.Spawn(Prefab.gameObject);

        var triggers = fx.GetComponentsInChildren<TriggerFx>();
        foreach (var trigger in triggers)
        {
            trigger.Initialized(owner);
        }

        Vector3 pos;

        if (!targetPos)
        {
            pos = SetFxPos(owner);
        }
        else
        {
            pos = SetFxPos(owner.Target);
        }

        fx.transform.SetPositionAndRotation(pos, owner.transform.rotation);
    }

    private Vector3 SetFxPos(Unit owner)
    {
        if (offSet == Vector3.zero)
        {
            return owner.transform.position;
        }
        else
        {
            return owner.transform.position + offSet;
        }
    }
}