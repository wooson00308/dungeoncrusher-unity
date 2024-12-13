using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Skill : MonoBehaviour
{
    private Unit _owner;
    private int _skillLevel = 1;
    [SerializeField] private SkillData _skillData;
    public SkillData SkillData => _skillData;
    private float _timeMarker;
    private bool _isInitialized = false;
    private bool OnOff = false;

    public void Setup(Unit owner)
    {
        _owner = owner;
        _isInitialized = true;
    }

    public void SetSkillData(SkillData skillData)
    {
        _skillData = skillData;
    }

    private void OnEnable()
    {
        if (_skillData.IsSelfSkill)
        {
            GameEventSystem.Instance.Subscribe(ProcessEvents.Engage.ToString(), On);
            GameEventSystem.Instance.Subscribe(ProcessEvents.Ready.ToString(), Off);
            return;
        }
        GameEventSystem.Instance.Subscribe(_skillData.SkillEventType.ToString(), TrySpawnSkillEffect);
    }

    private void OnDisable()
    {
        _skillLevel = 1;
        _isInitialized = false;
        if (_skillData.IsSelfSkill)
        {
            GameEventSystem.Instance.Unsubscribe(ProcessEvents.Engage.ToString(), On);
            GameEventSystem.Instance.Unsubscribe(ProcessEvents.Ready.ToString(), Off);
            return;
        }
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
        if (!args.publisher.GetInstanceID().Equals(_owner.GetInstanceID())) return;
        if (gameEvent.eventType != _skillData.SkillEventType.ToString()) return;
        if (args.publisher.Target == null) return;

        float random = Random.Range(0, 100f);

        if (random < skillLevelDetails.activationChance) return;

        GameObject skillFxPrefab = skillLevelDetails.skillFxPrefab;
        if (skillFxPrefab == null)
            return;
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

        skillFxObject.transform.position = args.publisher.Target.transform.position;

        var publisherTarget = args.publisher.GetComponent<TargetDetector>().Target;
        skillFx.Initialized(_skillLevel, args.publisher, _skillData, publisherTarget);
    }
    public void TrySpawnSkillEffect()
    {
        var skillLevelDetails = _skillData.GetSkillLevelData(_skillLevel);

        GameObject skillFxPrefab = skillLevelDetails.skillFxPrefab;
        if (skillFxPrefab == null)
            return;
        var skillFxObject = ResourceManager.Instance.Spawn(skillFxPrefab);
        if (skillFxObject == null)
            return;
        float random = Random.Range(0, 100f);
        if (random < skillLevelDetails.activationChance) return;
        

        var skillFx = skillFxObject.GetComponent<SkillEffect>();
        if (skillFx == null)
        {
            return;
        }
        skillFx.transform.position = _owner.transform.position;  
        if (_skillData.IsAreaAttack)
        {
            HashSet<Unit> enemies = UnitFactory.Instance.GetUnitsExcludingTeam(_owner.Team);
            List<Unit> targets = enemies.OrderBy(x => Random.value).Take(_skillData.GetSkillLevelData(_skillLevel).targetNum).ToList(); // 랜덤으로 지정 수만큼 타겟을 지정
            //enemies
            //.OrderBy(enemy => Vector3.Distance(enemy.transform.position, _owner.transform.position)) // 거리 기준으로 정렬
            //.Take(_skillData.GetSkillLevelData(_skillLevel).targetNum) // 가까운 적을 필요한 수만큼 가져오기
            //.ToList();
            skillFx.Initialized(_skillLevel, _owner, _skillData, targets);
        }
        else
        {
            skillFx.Initialized(_skillLevel, _owner, _skillData, _owner.GetComponent<TargetDetector>().Target);
        }
        _timeMarker = Time.time;
    }

    private void Update()
    {
        if (!OnOff)
            return;
        if (!_skillData.IsSelfSkill)
            return;
        if (Time.time - _timeMarker <= _skillData.GetSkillLevelData(_skillLevel).coolTIme)
            return;
        TrySpawnSkillEffect();
    }
    private void Off(GameEvent e)
    {
        OnOff = false;
    }
    private void On(GameEvent e)
    {
        _timeMarker = Time.time;
        OnOff = true;
    }
}