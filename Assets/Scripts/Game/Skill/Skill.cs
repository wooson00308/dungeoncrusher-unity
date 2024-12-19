using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private Unit _owner;
    private int _skillLevel = 1;
    [SerializeField] private SkillData _skillData;
    private float _timeMarker;
    private bool _isInitialized = false;
    public SkillData SkillData => _skillData;
    public int Level => _skillLevel;

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
        if (_skillData.IsUltSkill)
        {
            // 필살기로 지정될 이벤트 등록 필요
        }

        if (!_skillData.IsSelfSkill) // 자가 발동 스킬이면 이벤트 등록 필요 없음
        {
            GameEventSystem.Instance.Subscribe(_skillData.SkillEventType.ToString(), TryUseEventSkill);
        }
    }

    private void OnDisable()
    {
        _skillLevel = 1;
        _isInitialized = false;

        if (!_skillData.IsSelfSkill)
        {
            GameEventSystem.Instance.Unsubscribe(_skillData.SkillEventType.ToString(), TryUseEventSkill);
        }
    }

    public void LevelUp(int value = 1)
    {
        _skillLevel += value;
    }

    public void TryUseEventSkill(GameEvent gameEvent)
    {
        UnitEventArgs args = (UnitEventArgs)gameEvent.args;

        if (args == null) return;
        if (!args.publisher.GetInstanceID().Equals(_owner.GetInstanceID())) return;
        if (gameEvent.eventType != _skillData.SkillEventType.ToString()) return;
        if (args.publisher.Target == null) return;

        UseSkill(args.publisher);
    }

    public void TryUseSkillWheenCoolTimeReady()
    {
        if (!_skillData.IsSelfSkill) return; // 스킬이 특정 조건이 아닌 자가발동 스킬인지 체크 
        UseSkill(_owner);
    }

    /// <summary>
    /// 스킬 사용 중복 코드 메서드화
    /// </summary>
    /// <param name="user"></param>
    private void UseSkill(Unit user)
    {
        if (!IsUseableSkill()) return;

        _timeMarker = Time.time; // 스킬 쿨타임 초기화

        if (user.Target == null) return;

        #region ��ų ������ ���Ἲ �˻�

        var skillLevelDetails = _skillData.GetSkillLevelData(_skillLevel);

        GameObject skillFxPrefab = skillLevelDetails.skillFxPrefab;
        if (skillFxPrefab == null) return;

        var skillFxObject = ResourceManager.Instance.Spawn(skillFxPrefab);
        if (skillFxObject == null)
        {
            ResourceManager.Instance.Destroy(skillFxPrefab);
            return;
        }

        if (!skillFxObject.TryGetComponent<SkillEffect>(out var skillFx)) return;

        #endregion

        if (!Operator.IsRate(skillLevelDetails.activationChance)) return;

        skillFxObject.transform.position = user.Target.transform.position;
        if (_skillData.IsAreaAttack)
        {
            HashSet<Unit> enemies = UnitFactory.Instance.GetUnitsExcludingTeam(user.Team);

            // TODO : 수정 필요 -> 현재 타겟 주변의 일정 거리의 존재하는 유닛들을 범위 공격으로 피격 시켜야 함.
            // 1. 타겟을 찾고
            // 2. 적들 중 타겟과 거리가 일정거리 가까운 애들을 선별하여
            // 3. 범위 공격 피격

            // BEFORE
            //List<Unit> targets = enemies.OrderBy(x => Random.value).Take(_skillData.GetSkillLevelData(_skillLevel).targetNum).ToList();

            //AFTER
            // ...
            List<Unit> targets = enemies
                .Where(enemy => Vector3.Distance(enemy.transform.position, user.Target.transform.position) <=
                                _skillData.GetSkillLevelData(_skillLevel).range)
                .OrderBy(x => Random.value)
                .Take(_skillData.GetSkillLevelData(_skillLevel).targetNum)
                .ToList();
            skillFx.Initialized(this, user, _skillData, targets);
        }
        else
        {
            skillFx.Initialized(this, user, _skillData, new List<Unit> { user.Target });
        }
    }

    private void Update()
    {
        TryUseSkillWheenCoolTimeReady();
    }

    /// <summary>
    /// 스킬 사용 여부 판단
    /// </summary>
    /// <returns></returns>
    private bool IsUseableSkill()
    {
        if (!_isInitialized) // 초기화 되었는지 체크
            return false;

        if (!_owner.IsActive) // 스킬을 사용하는 유닛이 비활성화 되었는지 확인. 비활성화 조건 -> 게임이 준비 단계로 넘어가면 비활성화 됨.
        {
            ResetCooltime();
            return false;
        }

        if (!_skillData.IsValidTarget(_owner))
            return false;

        if (Time.time - _timeMarker <= _skillData.GetSkillLevelData(_skillLevel).coolTime) // 쿨타임 지났는지 체크 
            return false;

        return true;
    }

    public void ResetCooltime()
    {
        var cooltime = _skillData.GetSkillLevelData(_skillLevel).coolTime;

        if (Time.time - _timeMarker > cooltime) return;
        _timeMarker -= cooltime;
    }

    // 기존 기능 중에 전투 중인지 전투중이 아닌지 체크하는 기능이 존재하여 해당 기능을 비활성화하였습니다.
    //private void Off(GameEvent e)
    //{
    //    OnOff = false;
    //}
    //private void On(GameEvent e)
    //{
    //    _timeMarker = Time.time;
    //    OnOff = true;
    //}
}