using UnityEngine;

public class Skill : MonoBehaviour
{
    private Unit _owner;
    private int _skillLevel = 1;
    private float _currentCooltime;

    [SerializeField] private SkillData _skillData;
    public SkillData SkillData => _skillData;

    private bool _isInitialized = false;
    public int Level => _skillLevel;

    public void Setup(Unit owner)
    {
        _owner = owner;
        _isInitialized = true;
        _currentCooltime = _skillData.GetSkillLevelData(_skillLevel).skillCooltime;
    }

    public void SetSkillData(SkillData skillData)
    {
        _skillData = skillData;
    }

    private void OnEnable()
    {
        if (_skillData.SkillEventType == UnitEvents.None) return;
        GameEventSystem.Instance.Subscribe(_skillData.SkillEventType.ToString(), TrySpawnSkillEffect);
    }

    private void OnDisable()
    {
        _skillLevel = 1;
        _isInitialized = false;

        if (_skillData.SkillEventType == UnitEvents.None) return;
        GameEventSystem.Instance.Unsubscribe(_skillData.SkillEventType.ToString(), TrySpawnSkillEffect);
    }

    public void ResetCoolTime() => _currentCooltime = _skillData.GetSkillLevelData(_skillLevel).skillCooltime;

    public void LevelUp(int value = 1)
    {
        if (_skillLevel >= _skillData.SkillLevelDatas.Count - 1) return;
        _skillLevel += value;
    }

    public void TrySpawnSkillEffect(GameEvent gameEvent)
    {
        if (!_isInitialized) return;

        UnitEventArgs args = (UnitEventArgs)gameEvent.args;
        var skillLevelDetails = _skillData.GetSkillLevelData(_skillLevel);

        if (args == null) return;
        if (!args.publisher.GetInstanceID().Equals(_owner.GetInstanceID())) return;
        if (gameEvent.eventType != _skillData.SkillEventType.ToString()) return;
        if (args.publisher.Target == null) return;

        float random = Random.Range(0, 100f);
        if (random > skillLevelDetails.activationChance) return;

        SpawnSkillEffect(args.publisher, skillLevelDetails);
    }

    public async void SpawnSkillEffect(Unit user, SkillLevelData data)
    {
        GameObject skillFxPrefab = data.skillFxPrefab;
        var skillFxObject = ResourceManager.Instance.Spawn(skillFxPrefab);

        if (skillFxObject == null)
        {
            return;
        }

        var skillFx = skillFxObject.GetComponent<SkillEffect>();
        if (skillFx == null)
        {
            return;
        }

        while(user.Target == null)
        {
            if (!_owner.IsActive) return;
            await Awaitable.EndOfFrameAsync();
        }

        skillFxObject.transform.position = user.Target.transform.position;

        var publisherTarget = user.GetComponent<TargetDetector>().Target;
        skillFx.Initialized(this, user, _skillData, publisherTarget);
    }

    private void Update()
    {
        if (_skillData == null) return;
        if (_skillData.SkillEventType != UnitEvents.None) return;
        if (!_owner.IsActive) return;

        float skillCooltime = _skillData.GetSkillLevelData(_skillLevel).skillCooltime;

        if (_currentCooltime < skillCooltime)
        {
            _currentCooltime += Time.deltaTime;
        }
        else
        {
            _currentCooltime = 0;

            var skillLevelDetails = _skillData.GetSkillLevelData(_skillLevel);
            SpawnSkillEffect(_owner, skillLevelDetails);
        }
    }
}