using System;
using UnityEngine;
using UnityEngine.Rendering;

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

    private void Update()
    {
        _animator.speed = GameTime.TimeScale;
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

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnAttack, new UnitEventOnAttackArgs
        {
            publisher = _owner,
            target = _owner.Target,
        });

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

        _owner.UpdateSkillMp(10); //AnimationEvent Int 파라미터로 받는게 좋을 듯
    }

    public void DeathEvent(AnimationEvent e)
    {
        ReviveWait();
    }

    private async void ReviveWait()
    {
        if (_owner.Team == Team.Enemy)
        {
            Death((int)UnitEvents.UnitEvent_OnDeath);
        }
        else
        {
            _owner.IsActive = true;
            GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnRevive,
                new UnitEventArgs { publisher = _owner });

            if (_owner.ReviveCount <= 0)
            {
                Death((int)UnitEvents.UnitEvent_OnDeath);
            }
            
            await Awaitable.WaitForSecondsAsync(2);

            if (_owner.IsRevive && _owner.ReviveCount > 0)
            {
                _owner.OnRevive();
            }
            else
            {
                Death((int)UnitEvents.UnitEvent_OnDeath);
            }
        }
    }

    public void SpecialDeathEvent(AnimationEvent e)
    {
        Death((int)UnitEvents.UnitEvent_OnDeath_Special);
    }

    private void Death(int eventId)
    {
        UnitFactory.Instance.Destroy(_owner.Id, _owner);

        GameEventSystem.Instance.Publish(eventId, new UnitEventArgs { publisher = _owner });
        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnKill,
            new UnitEventOnAttackArgs { publisher = _owner.Killer, target = _owner });
    }

    private void OrderSprite()
    {
        _sortingGroup.sortingOrder = (int)(_owner.transform.position.y * -100);
    }
}