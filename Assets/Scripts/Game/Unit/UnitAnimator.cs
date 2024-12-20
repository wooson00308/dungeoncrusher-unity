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

    private void Awake()
    {
        _owner = GetComponentInParent<Unit>();
        _animator = GetComponent<Animator>();
        _sortingGroup = GetComponentInChildren<SortingGroup>() ??
                        transform.GetChild(2).gameObject.AddComponent<SortingGroup>();
    }

    private void FixedUpdate()
    {
        OrderSprite();
    }

    public void ChargeAttackEvent(AnimationEvent e)
    {
        _owner.Target?.OnStun();
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

    private Projectile _projectilePrefab;

    public void RangeAttackEvent(AnimationEvent e)
    {
        ResourceManager.Instance.Destroy(warningPrefab);
        var projectilePrefab = _owner.ProjectilePrefab;

        if (projectilePrefab == null) return;

        _projectilePrefab =
            ResourceManager.Instance.Spawn(projectilePrefab.gameObject).GetComponent<Projectile>();
        _projectilePrefab.transform.position = transform.position;

        var target = _owner.Target;
        _projectilePrefab.Initialize(target, targetPos, _owner.Attack.Value);

        // WarningEvent(e);
    }

    public void AttackEvent(AnimationEvent e)
    {
        var realDamage = _owner.Attack.Value;

        if (CriticalOperator.IsCritical(_owner.CriticalRate.Value))
        {
            realDamage = CriticalOperator.GetCriticalDamageIntValue(_owner.Attack.Value, _owner.CriticalPercent.Value);
        }

        _owner.Target?.OnHit(realDamage, _owner);

        SoundSystem.Instance.PlayFx("AttackSound1"); //AnimationEvent string으로 사운드 받으면 될듯

        GameEventSystem.Instance.Publish(UnitEvents.OnAttack.ToString(), new GameEvent
        {
            args = new UnitEventArgs
            {
                publisher = _owner
            },
            eventType = UnitEvents.OnAttack.ToString()
        });

        _owner.AddSkillMp(10); //AnimationEvent Int 파라미터로 받는게 좋을 듯
    }

    public void DeathEvent(AnimationEvent e)
    {
        UnitFactory.Instance.Destroy(_owner.Id, _owner);

        GameEventSystem.Instance.Publish(UnitEvents.OnDeath.ToString(), new GameEvent
        {
            eventType = UnitEvents.OnDeath.ToString(),
            args = new UnitEventArgs { publisher = _owner }
        });
    }

    public void SpecialDeathEvent(AnimationEvent e)
    {
        UnitFactory.Instance.Destroy(_owner.Id, _owner);

        GameEventSystem.Instance.Publish(UnitEvents.OnDeath_Special.ToString(),
            new GameEvent
            {
                args = new UnitEventArgs { publisher = _owner } //적 일러스트를 넣는다면 체크하는 용도로 유닛을 넘겨줌.
            });
    }

    private void OrderSprite()
    {
        _sortingGroup.sortingOrder = (int)(_owner.transform.position.y * -100);
    }
}