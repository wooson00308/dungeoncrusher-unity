using UnityEngine;

[CreateAssetMenu(fileName = "SupportShotSkillFxEventData",
    menuName = "Scriptable Objects/Skill/FxEvent/SupportShotSkillFxEventData")]
public class SupportShotSkillFxEventData : SkillFxEventData
{
    [SerializeField] private FxEventData data;
    [SerializeField] private Vector2 offset;

    public override void OnSkillEvent(Unit owner, Skill skill)
    {
        var spawnPrefab = ResourceManager.Instance.Spawn(data.Prefab.gameObject).GetComponent<FxEventAnimator>();
        spawnPrefab.transform.position = owner.transform.position + (Vector3)offset;

        var target = owner.Target;

        if(target != null)
        {
            var dir = target.transform.position - owner.transform.position;
            dir.Normalize();
            var yRot = dir.x > 0 ? 0 : 180;
            var rotation = Quaternion.Euler(0, yRot, 0);
            spawnPrefab.transform.rotation = rotation;
        }

        spawnPrefab.Initialized(data, owner, skill);
    }
}