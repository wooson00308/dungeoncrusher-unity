using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �����ϰ� �ִ� ��ų ����
/// </summary>
public class Skill : MonoBehaviour
{
    private Unit _owner;
    private SkillData _data;
    private SkillLevelData _currentLevelData;
    private readonly Dictionary<ConditionData, Action<object>> _conditionActions = new();

    private int _level = 1;
    private bool _isInitialized;

    // === �߰�: ��Ÿ�� / �෹�̼� ���� �ʵ� ===
    private bool _isCoolingDown;
    private float _cooltimeRemain; // ���� ��Ÿ��
    private bool _isDurationActive;
    private float _durationRemain; // ���� ���ӽð�

    public Unit Owner => _owner;
    public SkillData Data => _data;
    public SkillLevelData CurrentLevelData => _currentLevelData;

    public int Level => _level;

    public float CooltimeRemain => _cooltimeRemain;

    public bool IsCoolingdown => _isCoolingDown;

    /// <summary>
    /// ��ų �ʱ�ȭ
    /// </summary>
    public void Initialized(Unit owner, SkillData data)
    {
        _owner = owner;
        _data = data;

        _level = 1;
        _currentLevelData = _data.GetSkillLevelData(_level);

        SubscribeConditionEvents();

        // ��ų�� Ȱ��ȭ�� �� ����Ǵ� ����Ʈ (���� ��)
        foreach (var fxEventData in _currentLevelData.ApplyFxDatas)
        {
            fxEventData.OnSkillEvent(_owner, this);
        }

        _isInitialized = true;
    }

    /// <summary>
    /// OnDisable �� �� (������Ʈ�� ��Ȱ��ȭ�� ��) ó��
    /// </summary>
    public void OnDisable()
    {
        // ��ų�� ��Ȱ��ȭ�� �� �����ؾ� �� ����Ʈ�� �ִٸ� ���⼭ ó��
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
            // �̸� delegate(����) ���� �� ����
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
    /// (���� ȣ���ϰų� Condition�� �������� ��) ��ų ���
    /// </summary>
    public void UseSkill()
    {
        // �̹� ��Ÿ�� ���̸� ��ų ��� �Ұ�
        if (_isCoolingDown)
        {
            // Debug.Log($"[{name}] ��ų�� ��Ÿ���Դϴ�. ���� ��Ÿ��: {_cooltimeRemain:F2}s");
            return;
        }

        _isCoolingDown = true;

        // ��ų ��� FX ó��
        foreach (var fxEventData in _currentLevelData.UseSkillFxDatas)
        {
            fxEventData.OnEvent(_owner, this);
        }

        // === �߰�: ��Ÿ�� ���� ===
        _cooltimeRemain = _currentLevelData.Cooltime;

        // === �߰�: �෹�̼� ����(����ȿ���� �ִ� ��ų�̶��) ===
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

        // Debug.Log($"[{name}] ��ų ���! (��Ÿ�� {_cooltimeRemain:F2}s, ���ӽð� {_durationRemain:F2}s)");
    }

    /// <summary>
    /// ��ų ���� ��
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

    // === �߰�: ��Ÿ�� / �෹�̼� ���� ���� ===
    public void Update()
    {
        if (GameTime.TimeScale == 0) return;

        // 1) ��Ÿ���� ���� �ִٸ�
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

        // 2) ���� ȿ���� Ȱ��ȭ�Ǿ� �ִٸ�
        if (_isDurationActive)
        {
            _durationRemain -= Time.deltaTime;
            if (_durationRemain <= 0f)
            {
                _durationRemain = 0f;
                _isDurationActive = false;

                // �̰����� ���� ȿ���� ������ ���� ����Ʈ�� ó�� ȣ��
                // Debug.Log($"[{name}] ��ų ȿ��(�෹�̼�)�� ����Ǿ����ϴ�.");

                // RemoveFxDatas ���� �� ���⼭ ȣ���� ���� ����
                // (���� "��ų ȿ���� ������ ����"���� �ߵ��ϴ� ������ �ʿ��ϴٸ�)

                foreach (var fxEventData in _currentLevelData.UseSkillFxDatas)
                {
                    fxEventData.OnEndEvent(_owner, this);
                }
            }
        }
    }
}