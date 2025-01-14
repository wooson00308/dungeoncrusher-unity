using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private Unit _owner;
    private int _skillLevel = 1;
    [SerializeField] private SkillData _skillData;
    private float _timeMarker;
    private bool _isInitialized = false;
    private bool _isCooldown;
    public float TimeMarker => _timeMarker;
    public bool IsCooldown => _isCooldown;
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
        if (_skillData.IsPassiveSkill) // 자가 발동 스킬이면 이벤트 등록 필요 없음
        {
            GameEventSystem.Instance.Subscribe(_skillData.SkillEventType.ToString(), TryUseEventSkill);
        }
        else
        {
            GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_UseSkill_Publish_UI.ToString(), TryUseSkillFromUI);
        }
    }

    private void OnDestroy() //재시작을 할때
    {
        _skillData.Level = 0;
    }

    private void OnApplicationQuit() //종료될 때
    {
        _skillData.Level = 0;
    }

    private void OnDisable()
    {
        _skillLevel = 1;
        _isInitialized = false;

        if (_skillData.IsUltSkill)
        {
            // 필살기로 지정될 이벤트 등록 필요
            GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_UseSkill_Publish_UI.ToString(),
                TryUseSkillFromUI);
        }

        if (_skillData.IsPassiveSkill)
        {
            GameEventSystem.Instance.Unsubscribe(_skillData.SkillEventType.ToString(), TryUseEventSkill);
        }
    }

    public void LevelUp(int value = 1)
    {
        if(_skillData.MaxLv > _skillLevel)
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

    public void TryUseSkillFromUI(GameEvent e)
    {
        var args = (SkillEventArgs)e.args;
        if (args.data.Id != _skillData.Id) return;
        if (_skillData.IsUltSkill && _owner.Mp.Value < _owner.Mp.Max) return;
        UseSkill(_owner);
    }

    public void TryUsePassiveSkillWheenCoolTime()
    {
        if (!_skillData.IsPassiveSkill) return;
        if (!_skillData.IsCooltimeSkill) return; // 스킬이 특정 조건이 아닌 자가발동 스킬인지 체크 
        UseSkill(_owner);
    }

    /// <summary>
    /// 스킬 사용 중복 코드 메서드화
    /// </summary>
    /// <param name="user"></param>
    private void UseSkill(Unit user)
    {
        if (!IsUseableSkill()) return;

        if (_skillData.IsUltSkill)
        {
            _owner.Mp.Update("Engage", -_owner.Mp.Value);
        }

        _timeMarker = Time.time; // 스킬 쿨타임 초기화

        var target = user.Target;
        if (target == null) return;

        #region 스킬 프리팹 무결성 검사

        var skillLevelDetails = _skillData.GetSkillLevelData(_skillLevel);

        GameObject skillFxPrefab = skillLevelDetails.skillFxPrefab;
        if (!Operator.IsRate(skillLevelDetails.activationChance)) return;

        if (skillFxPrefab == null) return;

        var skillFxObject = ResourceManager.Instance.Spawn(skillFxPrefab);
        if (skillFxObject == null)
        {
            ResourceManager.Instance.Destroy(skillFxPrefab);
            return;
        }

        if (!skillFxObject.TryGetComponent<SkillEffect>(out var skillFx)) return;

        #endregion


        if (_skillData.SkillFXSpawnPosType == SkillFXSpawnPosType.Self)
        {
            skillFxObject.transform.position = user.transform.position;
            Vector3 temp = skillFxObject.transform.localScale;
            temp.x = (user.transform.position.x - target.transform.position.x) >= 0 ? -1f : 1f;
            skillFxObject.transform.localScale = temp;
        }
        else
        {
            skillFxObject.transform.position = target.transform.position;
        }

        if (_skillData.IsRamdomDetected)
        {
            HashSet<Unit> enemies = UnitFactory.Instance.GetUnitsExcludingTeam(user.Team);
            // Debug.Log(enemies.Count);
            var enemy = enemies.OrderBy(x => Random.value).Take(1).ToList();
            if (_skillData.IsAreaAttack)
            {
                // 현재 타겟 주변의 일정 거리의 존재하는 유닛들을 범위 공격으로 피격 시킴.
                //List<Unit> targets = enemies
                //    .Where(x => Vector3.Distance(x.transform.position, enemy[0].transform.position) <=
                //                    _skillData.GetSkillLevelData(_skillLevel).range)
                //    .OrderBy(x => Random.value)
                //    .Take(_skillData.GetSkillLevelData(_skillLevel).targetNum)
                //    .ToList();
                List<Unit> targets = enemies
                    .Where(x => Vector3.Distance(x.transform.position, enemy[0].transform.position) <=
                                _skillData.GetSkillLevelData(_skillLevel).range)
                    .Where(x => x.enabled && x.IsActive)
                    .OrderBy(x => Vector3.Distance(x.transform.position, enemy[0].transform.position))
                    .Take(_skillData.GetSkillLevelData(_skillLevel).targetNum)
                    .ToList();
                Debug.Log("targets" + targets.Count);
                if (!targets.Contains(enemy[0]))
                {
                    targets.Insert(0, enemy[0]); // enemy를 targets의 맨 앞에 추가
                }
                else
                {
                    // enemy가 이미 포함되어 있다면 targets의 첫 번째로 이동
                    targets.Remove(enemy[0]);
                    targets.Insert(0, enemy[0]);
                }

                if (_skillData.SkillFXSpawnPosType == SkillFXSpawnPosType.Target)
                    skillFxObject.transform.position = targets[0].transform.position;
                skillFx.Initialized(this, user, _skillData, targets);
                return;
            }

            if (_skillData.SkillFXSpawnPosType == SkillFXSpawnPosType.Target)
                skillFxObject.transform.position = enemy[0].transform.position;
            skillFx.Initialized(this, user, _skillData, enemy);
            return;
        }

        if (_skillData.IsAreaAttack)
        {
            HashSet<Unit> enemies = UnitFactory.Instance.GetUnitsExcludingTeam(user.Team);

            // 현재 타겟 주변의 일정 거리의 존재하는 유닛들을 범위 공격으로 피격 시킴.
            List<Unit> targets = enemies
                .Where(enemy => Vector3.Distance(enemy.transform.position, target.transform.position) <=
                                _skillData.GetSkillLevelData(_skillLevel).range)
                .OrderBy(x => Random.value)
                .Take(_skillData.GetSkillLevelData(_skillLevel).targetNum)
                .ToList();

            skillFx.Initialized(this, user, _skillData, targets);
        }
        else
        {
            skillFx.Initialized(this, user, _skillData, new List<Unit> { target });
        }
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

        if (_owner.IsStun)
            return false;

        if (!_skillData.IsValidTarget(_owner))
            return false;

        if (_isCooldown) // 쿨타임 지났는지 체크 
            return false;

        return true;
    }

    private void Update()
    {
        _isCooldown = Time.time - _timeMarker <= _skillData.GetSkillLevelData(_skillLevel).coolTime;
        TryUsePassiveSkillWheenCoolTime();
    }

    public void ResetCooltime()
    {
        var cooltime = _skillData.GetSkillLevelData(_skillLevel).coolTime;
        if (Time.time - _timeMarker > cooltime) return;
        _timeMarker -= cooltime;
    }
}