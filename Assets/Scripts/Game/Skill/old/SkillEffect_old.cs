using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillEffect_old : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    protected readonly List<Unit> _units = new();

    protected bool _isInitialized = false;
    protected bool _isMultiTargeting = false;

    private Unit _user;
    private SkillData_old _skillData;
    private Unit _target;
    private List<Unit> _targets;
    private Skill_old _skill;

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnStun, OnStunEvent);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnStun, OnStunEvent);
    }

    private void OnStunEvent(object e)
    {
        if (!_isInitialized) return;

        var args = e as UnitEventArgs;
        if (!args.publisher.GetInstanceID().Equals(_user.GetInstanceID())) return;

        Destroy(null);
    }

    public void Initialized(Skill_old skill, Unit user, SkillData_old skillData, Unit target = null)
    {
        _skill = skill;
        _user = user;
        _skillData = skillData;

        if (!_skillData.IsAreaAttack)
        {
            _target = target;
        }

        _isInitialized = true;
    }
    public void Initialized(Skill_old skill, Unit user, SkillData_old skillData, List<Unit> targets = null)
    {
        _skill = skill;
        _user = user;
        _skillData = skillData;
        if (!_skillData.IsAreaAttack)
        {
            _target = targets[^1];
        }
        _targets = targets;
        _isMultiTargeting = true;
        _isInitialized = true;
    }

    public void OnAction(AnimationEvent e)
    {
        if (!_isInitialized) return;
        
        if (_isMultiTargeting) { 
            _skillData.OnAction(_skill, _user, _targets); 
            return;
        }
        if (!_skillData.IsAreaAttack)
        {
            _skillData.OnAction(_skill, _user, _units);
        }
        else
        {
            
            var targets = new List<Unit>() { _target };
            _skillData.OnAction(_skill, _user, targets);
        }
    }

    public void Destroy(AnimationEvent e)
    {
        if (!_isInitialized) return;
        _units.Clear();
        _isInitialized = false;
        ResourceManager.Instance.Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isInitialized) return;
        if (!_skillData.IsAreaAttack) return;
        if (collision.TryGetComponent<Unit>(out var unit))
        {
            _units.Add(unit);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isInitialized) return;
        if (!_skillData.IsAreaAttack) return;
        if (collision.TryGetComponent<Unit>(out var unit))
        {
            _units.Remove(unit);
        }
    }
}
