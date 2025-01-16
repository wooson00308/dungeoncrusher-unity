using System;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    private Unit _owner;
    private Animator _animator;
    private SortingGroup _sortingGroup;
    private Transform _body;
    public Transform Body => _body;

    private void Awake()
    {
        _owner = GetComponentInParent<Unit>();
        _animator = GetComponent<Animator>();
        _sortingGroup = GetComponentInChildren<SortingGroup>() ??
                        transform.GetChild(2).gameObject.AddComponent<SortingGroup>();
        _body = transform.Find("Body");
    }

    private void FixedUpdate()
    {
        OrderSprite();
    }

    public void ChargeAttackEvent(AnimationEvent e)
    {
        _owner.Target?.OnStun(e.floatParameter);
        AttackEvent(e);
    }

    private GameObject warningPrefab;
    private Vector2 targetPos;

    public void WarningEvent(AnimationEvent e)
    {
        warningPrefab = ResourceManager.Instance.Spawn(_owner.WarningPrefab.gameObject);

        if (warningPrefab == null) return;

        targetPos = _owner.Target.transform.position;
        warningPrefab.GetComponent<Warning>().Initialize(_owner, targetPos);
    }


    public void RangeAttackEvent(AnimationEvent e)
    {
        ResourceManager.Instance.Destroy(warningPrefab);
        var projectilePrefab = _owner.ProjectilePrefab;

        if (projectilePrefab == null) return;

        Projectile_old _spawnProjectilePrefab =
            ResourceManager.Instance.Spawn(projectilePrefab.gameObject).GetComponent<Projectile_old>();
        _spawnProjectilePrefab.transform.position = transform.position;

        var target = _owner.Target;
        _spawnProjectilePrefab.Initialize(target, targetPos, _owner.Attack.Value);
    }

    public void AttackEvent(AnimationEvent e)
    {
        var realDamage = _owner.Attack.Value;

        if (CriticalOperator.IsCritical(_owner.CriticalRate.Value))
        {
            realDamage = CriticalOperator.GetCriticalDamageIntValue(_owner.Attack.Value, _owner.CriticalPercent.Value);
            _owner.Target?.OnHit(realDamage, _owner, true);
        }
        else
        {
            _owner.Target?.OnHit(realDamage, _owner);
        }

        SoundSystem.Instance.PlayFx("AttackSound1"); //AnimationEvent string으로 사운드 받으면 될듯

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_OnAttack.ToString(), new UnitEventArgs
        {
            publisher = _owner
        });

        _owner.AddSkillMp(10); //AnimationEvent Int 파라미터로 받는게 좋을 듯
    }

    public void DeathEvent(AnimationEvent e)
    {
        UnitFactory.Instance.Destroy(_owner.Id, _owner);

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_OnDeath.ToString(), new UnitEventArgs { publisher = _owner });
    }

    public void SpecialDeathEvent(AnimationEvent e)
    {
        UnitFactory.Instance.Destroy(_owner.Id, _owner);

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_OnDeath_Special.ToString(), new UnitEventArgs { publisher = _owner });
    }

    private void OrderSprite()
    {
        _sortingGroup.sortingOrder = (int)(_owner.transform.position.y * -100);
    }
}