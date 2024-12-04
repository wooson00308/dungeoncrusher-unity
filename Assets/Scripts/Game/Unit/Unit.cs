using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FSM))]
[RequireComponent(typeof(IdleState))]
[RequireComponent(typeof(StunState))]
[RequireComponent(typeof(DeathState))]
public class Unit : MonoBehaviour, IStats, IStatSetable, IStatUpdatable
{
    #region Fields

    public Team Team { get; set; }

    private bool _isInitialized = false;
    private float _stunDuration;

    private Dictionary<PartType, Item> _equipments = new();
    private Dictionary<string, Skill> _skillDic = new();

    private TargetDetector _targetDetector;
    private NavMeshAgent _agent;
    private Animator _animator;
    private FSM _fsm;

    public bool IsIniailized => _isInitialized;
    public float StunDuration => _stunDuration;
    public Unit Target => _targetDetector.Target;
    public bool IsDeath { get; private set; }

    [Header("Config")] [SerializeField] private string _id;
    [SerializeField] private Line _line;
    [SerializeField] private UnitAnimator _model;
    [SerializeField] private Transform _skillStorage;
    [SerializeField] private Transform _inventory;

    public string Id => _id;
    public Line Line => _line;

    #endregion

    #region Stats

    public int Health { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public float Speed { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackRange { get; private set; }
    public float CriticalRate { get; private set; }
    public float AttackStunRate { get; private set; }
    public float LifestealRate { get; private set; }

    public void SetupStats(IStats stats)
    {
        Health = stats.Health;
        Attack = stats.Attack;
        Defense = stats.Defense;
        Speed = stats.Speed;
        AttackSpeed = stats.AttackSpeed;
        AttackRange = stats.AttackRange;
        CriticalRate = stats.CriticalRate;
        AttackStunRate = stats.AttackStunRate;
        LifestealRate = stats.LifestealRate;

        if (_agent != null)
        {
            _agent.speed = Speed;
        }

        if (_fsm != null)
        {
            _fsm.StartState<IdleState>();
        }

        _stunDuration = 0;
        IsDeath = false;
    }

    public void IncreaseStats(IStats stats)
    {
        Health += stats.Health;
        Attack += stats.Attack;
        Defense += stats.Defense;
        Speed += stats.Speed;
        AttackSpeed += stats.AttackSpeed;
        AttackRange += stats.AttackRange;
        CriticalRate += stats.CriticalRate;
        AttackStunRate += stats.AttackStunRate;
        LifestealRate += stats.LifestealRate;
    }

    public void DecreaseStats(IStats stats)
    {
        Health -= stats.Health;
        Attack -= stats.Attack;
        Defense -= stats.Defense;
        Speed -= stats.Speed;
        AttackSpeed -= stats.AttackSpeed;
        AttackRange -= stats.AttackRange;
        CriticalRate -= stats.CriticalRate;
        AttackStunRate -= stats.AttackStunRate;
        LifestealRate -= stats.LifestealRate;
    }

    #endregion

    private void Awake()
    {
        _fsm = GetComponent<FSM>();
        _agent = GetComponent<NavMeshAgent>();
        _targetDetector = GetComponent<TargetDetector>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _animator = _model.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(ProcessEvents.SetActive.ToString(), SetActive);
    }

    private void OnDisable()
    {
        _isInitialized = false;
        GameEventSystem.Instance.Unsubscribe(ProcessEvents.SetActive.ToString(), SetActive);
    }

    public void OnInitialized(IStats data, Team team)
    {
        _fsm.enabled = false;

        Team = team;
        SetupStats(data);

        _isInitialized = true;
    }

    private void SetActive(GameEvent gameEvent)
    {
        bool isActive = (bool)gameEvent.args;
        _fsm.enabled = isActive;

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_SetActive.ToString(), new GameEvent
        {
            args = new UnitEventArgs()
            {
                publisher = this,
            },
            eventType = UnitEvents.UnitEvent_SetActive.ToString()
        });
    }

    #region Movement

    /// <summary>
    /// 타겟 방향으로 이동
    /// </summary>
    /// <param name="target"></param>
    public void MoveFromTarget(Transform target)
    {
        _agent.isStopped = false;
        _agent.speed = Speed;
        _agent.SetDestination(target.position);
        Rotation(target.position - transform.position);
    }

    /// <summary>
    /// 이동 일시중지
    /// </summary>
    public void Stop()
    {
        _agent.isStopped = true;
    }

    /// <summary>
    /// 타겟 초기화
    /// </summary>
    public void ResetMovePath()
    {
        _agent.ResetPath();
    }

    /// <summary>
    /// 해당 방향으로 회전
    /// </summary>
    /// <param name="rotDir"></param>
    public void Rotation(Vector3 rotDir)
    {
        if (rotDir.x != 0)
        {
            float rotY = rotDir.x > 0 ? 0 : 180;
            _model.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }
    }

    #endregion

    #region FSM

    public void OnHit(int damage, Unit attacker = null)
    {
        Health -= damage;

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_OnHit.ToString(), new GameEvent
        {
            eventType = UnitEvents.UnitEvent_OnHit.ToString(),
            args = new OnHitEventArgs { publisher = this, damageValue = damage }
        });

        if (Health <= 0)
        {
            OnDeath(attacker);
        }
    }

    public void OnDeath(Unit killer = null)
    {
        if (IsDeath) return;
        IsDeath = true;

        // Debug.Log($"{killer}이(가) {_id}을(를) 처치하였습니다.");

        _fsm.TransitionTo<DeathState>();
    }

    public void OnStun(int stunDuration)
    {
        if (IsDeath) return;
        _fsm.TransitionTo<StunState>();
    }

    #endregion

    #region Animator

    public void CrossFade(int state, float fadeTime)
    {
        _animator.CrossFade(state, fadeTime);
    }

    public void CrossFade(string state, float fadeTime)
    {
        _animator.CrossFade(state, fadeTime);
    }

    public bool IsUpdateState(string state)
    {
        return _animator.GetBool(state);
    }

    public int GetAttackState() => _animator.GetInteger("AttackState");

    #endregion

    #region Skill

    public void AddSkill(SkillData skillData)
    {
        if (_skillDic.TryGetValue(skillData.Id, out var skill))
        {
            // 해당 스킬을 가지고 있으면, 그 스킬의 레벨을 올려준다.
            skill.LevelUp();
        }
        else
        {
            var skillObj = ResourceManager.Instance.SpawnFromPath($"Skill/{skillData.PrefId}");
            var skillComponent = skillObj.GetComponent<Skill>();
            skillComponent.Setup(this);

            _skillDic.Add(skillData.Id, skillComponent);

            skillObj.transform.SetParent(_skillStorage);
        }
    }

    #endregion

    #region Inventory

    public void EquipItem(Item item)
    {
        Item spawnItem = ResourceManager.Instance.SpawnFromPath($"Item/{item.name}").GetComponent<Item>();
        // 아이템 교체
        if (_equipments.TryGetValue(item.Data.PartType, out var equipment))
        {
            DecreaseStats(equipment.Data);
            ResourceManager.Instance.Destroy(equipment.gameObject);
            _equipments[item.Data.PartType] = item;
        }
        else
        {
            _equipments.Add(item.Data.PartType, item);
        }

        IncreaseStats(item.Data);

        spawnItem.transform.SetParent(_inventory);
    }

    #endregion
}

public enum Team
{
    Friendly,
    Enemy
}

public enum Line
{
    Frontline,
    Midline,
    Backline
}