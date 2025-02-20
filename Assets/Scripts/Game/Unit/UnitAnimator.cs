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
    public GameObject model;

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

    public void SetVisialbe(bool value)
    {
        model.SetActive(value);
    }

    public void ChargeAttackEvent(AnimationEvent e)
    {
        _owner.Target?.OnStun(e.floatParameter);
        AttackEvent(e);
    }

    private GameObject _warningPrefab;
    private Vector2 _targetPos;

    public void WarningEvent(AnimationEvent e)
    {
        _warningPrefab = ResourceManager.Instance.Spawn(_owner.WarningPrefab.gameObject);

        if (_warningPrefab == null) return;

        _targetPos = _owner.Target.transform.position;
        _warningPrefab.GetComponent<Warning>().Initialize(_owner, _targetPos);
    }


    public void RangeAttackEvent(AnimationEvent e)
    {
        ResourceManager.Instance.Destroy(_warningPrefab);
        var projectilePrefab = _owner.ProjectilePrefab;

        if (projectilePrefab == null) return;

        Projectile_old spawnProjectilePrefab =
            ResourceManager.Instance.Spawn(projectilePrefab.gameObject).GetComponent<Projectile_old>();
        spawnProjectilePrefab.transform.position = transform.position;

        var target = _owner.Target;
        spawnProjectilePrefab.Initialize(target, _targetPos, _owner.Attack.Value);
    }

    public void AttackEvent(AnimationEvent e)
    {
        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnAttack, new UnitEventOnAttackArgs
        {
            Publisher = _owner,
            Target = _owner.Target,
        });

        // if (CriticalOperator.IsCritical(_owner.CriticalRate.Value))  
        // {
        //     realDamage = CriticalOperator.GetCriticalDamageIntValue(_owner.Attack.Value, _owner.CriticalPercent.Value);
        //     _owner.Target?.OnHit(realDamage, _owner, true);
        // }
        // else
        // {
        //     _owner.Target?.OnHit(realDamage, _owner);
        // }
        (int damage, bool isCritical) = _owner.LastDamage();
        _owner.Target?.OnHit(damage, isCritical: isCritical);

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
            float time = 0;
            while (time <= 2f)
            {
                if (_owner.ReviveCount <= 0)
                {
                    Death((int)UnitEvents.UnitEvent_OnDeath);
                    return;
                }

                time += Time.deltaTime;
                await Awaitable.EndOfFrameAsync();
            }

            _owner.OnRevive();
        }
    }

    public void SpecialDeathEvent(AnimationEvent e)
    {
        Death((int)UnitEvents.UnitEvent_OnDeath_Special);
    }

    private void Death(int eventId)
    {
        UnitFactory.Instance.Destroy(_owner.Id, _owner);

        GameEventSystem.Instance.Publish(eventId, new UnitEventArgs { Publisher = _owner });
        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnKill,
            new UnitEventOnAttackArgs { Publisher = _owner.Killer, Target = _owner });
    }

    private void OrderSprite()
    {
        _sortingGroup.sortingOrder = (int)(_owner.transform.position.y * -100);
    }
}