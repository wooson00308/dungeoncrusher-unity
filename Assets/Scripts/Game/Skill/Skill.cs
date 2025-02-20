using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 사용과 관련된 클래스
/// </summary>
public class Skill : MonoBehaviour
{
    private Unit _owner;
    private SkillData _data;
    private SkillLevelData _currentLevelData;
    private readonly Dictionary<ConditionData, Action<object>> _conditionActions = new();

    private int _level = 1;
    private bool _isInitialized;

    // === 추가: 쿨타임 / 지속시간 관련 변수 ===
    private bool _isCoolingDown;
    private float _cooltimeRemain; // 쿨타임 남은 시간
    private bool _isDurationActive;
    private float _durationRemain; // 지속시간 남은 시간

    public Unit Owner => _owner;
    public SkillData Data => _data;
    public SkillLevelData CurrentLevelData => _currentLevelData;

    public int Level => _level;

    public float CooltimeRemain => _cooltimeRemain;

    public bool IsCoolingdown => _isCoolingDown;

    /// <summary>
    /// 스킬 초기화
    /// </summary>
    public void Initialized(Unit owner, SkillData data)
    {
        _owner = owner;
        _data = data;

        _level = 1;
        _currentLevelData = _data.GetSkillLevelData(_level);

        SubscribeConditionEvents();

        // 스킬 적용 효과들을 초기화한다 (적용 시)
        foreach (var fxEventData in _currentLevelData.ApplyFxDatas)
        {
            fxEventData.OnSkillEvent(_owner, this);
        }

        _isInitialized = true;
    }

    /// <summary>
    /// OnDisable 시 (해제 효과를 실행)
    /// </summary>
    public void OnDisable()
    {
        // 스킬 비활성화 시 적용된 효과들을 제거하기 위해 호출
        foreach (var fxEventData in _currentLevelData.RemoveFxDatas)
        {
            fxEventData.OnSkillEvent(_owner, this);
        }

        _isInitialized = false;
        UnsubscribeConditionEvents();
    }

    private void SubscribeConditionEvents()
    {
        foreach (var condition in _currentLevelData.Conditions)
        {
            if (!_conditionActions.ContainsKey(condition))
            {
                Action<object> action = OnEvent(condition);
                _conditionActions.Add(condition, action);
            }

            GameEventSystem.Instance.Subscribe(condition.EventId, _conditionActions[condition]);
        }

        Action<object> OnEvent(ConditionData condition)
        {
            // 익명 delegate(람다)를 사용한 이유
            return (gameEvent) => { condition.TryEvent(this, gameEvent); };
        }
    }

    private void UnsubscribeConditionEvents()
    {
        foreach (var condition in _currentLevelData.Conditions)
        {
            if (_conditionActions.TryGetValue(condition, out var action))
            {
                GameEventSystem.Instance.Unsubscribe(condition.EventId, action);
                _conditionActions.Remove(condition);
            }
        }
    }

    /// <summary>
    /// 스킬 사용 (조건에 의해 호출됨)
    /// </summary>
    public void UseSkill()
    {
        // 이미 쿨타임 중이면 스킬 사용하지 않음
        if (_isCoolingDown)
        {
            // Debug.Log($"[{name}] 스킬이 쿨타임 중입니다. 남은 쿨타임: {_cooltimeRemain:F2}s");
            return;
        }
        _isCoolingDown = true;

        // 스킬 사용 FX 실행
        foreach (var fxEventData in _currentLevelData.UseSkillFxDatas)
        {
            fxEventData.OnEvent(_owner, this);
        }

        // === 추가: 쿨타임 적용 ===
        _cooltimeRemain = _currentLevelData.Cooltime;

        // === 추가: 지속시간 처리 (지속시간이 있는 스킬인 경우) ===
        if (_currentLevelData.Duration > 0f)
        {
            _isDurationActive = true;
            _durationRemain = _currentLevelData.Duration;
        }
        else
        {
            _isDurationActive = false;
        }

        if (_owner.Mp.Value >= _currentLevelData.NeedMp && _currentLevelData.NeedMp != 0)
        {
            _owner.UpdateSkillMp(-_currentLevelData.NeedMp);
            GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_UseSkill_Ulti);
        }

        // Debug.Log($"[{name}] 스킬 사용! (쿨타임: {_cooltimeRemain:F2}s, 지속시간: {_durationRemain:F2}s)");
    }

    /// <summary>
    /// 스킬 레벨 업
    /// </summary>
    public void LevelUp(int amount = 1)
    {
        if (!_isInitialized) return;
        if (_level >= _data.MaxLevel) return;

        int changeLevel = Math.Min(_level + amount, _data.MaxLevel);

        if (_level == changeLevel) return;
        _level = changeLevel;

        UnsubscribeConditionEvents();

        _currentLevelData = _data.GetSkillLevelData(_level);
        SubscribeConditionEvents();

        foreach (var fxEventData in _currentLevelData.ApplyFxDatas)
        {
            fxEventData.OnSkillEvent(_owner, this);
        }
    }

    public void ResetCooltime()
    {
        _cooltimeRemain = 0;
    }

    // === 추가: 쿨타임 및 지속시간 업데이트 ===
    public void Update()
    {
        if (GameTime.TimeScale == 0) return;

        // 1) 쿨타임 진행 중일 때
        if (_isCoolingDown)
        {
            _cooltimeRemain -= Time.deltaTime;

            if (_cooltimeRemain <= 0f)
            {
                _cooltimeRemain = 0f;
                _isCoolingDown = false;
            }
        }
        else
        {
            if (CurrentLevelData.Conditions.Count <= 0 && _owner.Mp.Value >= _currentLevelData.NeedMp)
            {
                UseSkill();
            }
        }

        // 2) 지속시간 효과가 활성화 중일 때
        if (_isDurationActive)
        {
            _durationRemain -= Time.deltaTime;
            if (_durationRemain <= 0f)
            {
                _durationRemain = 0f;
                _isDurationActive = false;

                // 지속시간 종료 시 스킬 효과 종료 이벤트 호출
                // Debug.Log($"[{name}] 스킬 지속 효과가 종료되었습니다.");

                // 스킬 종료 FX 이벤트 호출 (별도의 '스킬 지속 효과 종료'가 구현되어 있지 않은 경우)
                foreach (var fxEventData in _currentLevelData.UseSkillFxDatas)
                {
                    fxEventData.OnEndEvent(_owner, this);
                }
            }
        }
    }
}
