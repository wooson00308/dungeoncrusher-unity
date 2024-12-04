using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

   protected readonly List<Unit> _units = new();

    protected bool _isInitialized = false;

    private Unit _user;
    private SkillData _skillData;
    private Unit _target;
    private int _level;

    public void Initialized(int level, Unit user, SkillData skillData, Unit target = null)
    {
        _level = level;
        _user = user;
        _skillData = skillData;

        if (!_skillData.IsAreaAttack)
        {
            _target = target;
        }

        _isInitialized = true;
    }

    public void OnAction(AnimationEvent e)
    {
        if (_skillData.IsAreaAttack)
        {
            _skillData.OnAction(_level, _user, _units);
        }
        else
        {
            
            var targets = new List<Unit>() { _target };
            _skillData.OnAction(_level, _user, targets);
        }
    }

    public void Destroy(AnimationEvent e)
    {
        _units.Clear();
        _isInitialized = false;
        ResourceManager.Instance.Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_skillData.IsAreaAttack) return;
        if (collision.TryGetComponent<Unit>(out var unit))
        {
            _units.Add(unit);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!_skillData.IsAreaAttack) return;
        if (collision.TryGetComponent<Unit>(out var unit))
        {
            _units.Remove(unit);
        }
    }
}
