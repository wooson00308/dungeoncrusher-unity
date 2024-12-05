using UnityEngine;

public class Skill : MonoBehaviour
{
    private Unit _owner;
    private int _skillLevel = 1;
    [SerializeField] private SkillData _skillData;
    public SkillData SkillData => _skillData;

    private bool _isInitialized = false;

    public void Setup(Unit owner)
    {
        _owner = owner;
        _isInitialized = true;
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(_skillData.SkillEventType.ToString(), TrySpawnSkillEffect);
    }

    private void OnDisable()
    {
        _skillLevel = 1;
        _isInitialized = false;

        GameEventSystem.Instance.Unsubscribe(_skillData.SkillEventType.ToString(), TrySpawnSkillEffect);
    }

    public void LevelUp(int value = 1)
    {
        _skillLevel += value;
    }

    public void TrySpawnSkillEffect(GameEvent gameEvent)
    {
        if (!_isInitialized) return;

        UnitEventArgs args = (UnitEventArgs)gameEvent.args;
        var skillLevelDetails = _skillData.GetSkillLevelData(_skillLevel);

        if (args == null) return;
        if (args.publisher.Id != _skillData.UnitId) return;
        if (!args.publisher.GetInstanceID().Equals(_owner.GetInstanceID())) return;
        if (gameEvent.eventType != _skillData.SkillEventType.ToString()) return;
        if (args.publisher.Target == null) return;

        float random = Random.Range(0, 100f);

        if (random < skillLevelDetails.activationChance) return;

        string prefabId = skillLevelDetails.prefabId;
        var skillPrefab = ResourceManager.Instance.SpawnFromPath($"Skill/Fx/{prefabId}");

        if (skillPrefab == null)
        {
            Debug.LogError($"{prefabId}가 엄서요");
            return;
        }

        var skillFx = skillPrefab.GetComponent<SkillEffect>();
        if (skillFx == null)
        {
            Debug.LogError($"{prefabId}의 스킬 컴포넌트가 엄서요");
            return;
        }

        skillPrefab.transform.position = args.publisher.Target.transform.position;

        var publisherTarget = args.publisher.GetComponent<TargetDetector>().Target;
        skillFx.Initialized(_skillLevel, args.publisher, _skillData, publisherTarget);
    }
}