using System;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Cinemachine;
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

    private float _stunDuration;

    private Dictionary<PartType, Item> _equipments = new();
    public Dictionary<PartType, Item> Equipment => _equipments;
    private Dictionary<string, Skill> _skillDic = new();
    public Dictionary<string, Skill> SkillDic => _skillDic;

    private TargetDetector _targetDetector;
    private NavMeshAgent _agent;
    private Animator _animator;
    private FSM _fsm;
    private Rigidbody2D _rigidbody;

    private GameObject _hitPrefab;
    private Projectile _projectilePrefab;
    private Warning _warningPrefab;
    private int _dropExp;

    private bool _hasHitState;
    private bool _hasAerialState;
    public float StunDuration => _stunDuration;

    public Unit Target => _targetDetector.Target;
    public bool IsStun { get; set; }
    public Projectile ProjectilePrefab => _projectilePrefab;
    public Warning WarningPrefab => _warningPrefab;
    public int DropExp => _dropExp;

    public bool IsHit { get; private set; }
    public bool IsDeath { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSuperArmor { get; set; }

    [Header("Config")] [SerializeField] private string _id;
    [SerializeField] private Line _line;
    [SerializeField] private UnitAnimator _model;
    [SerializeField] private Transform _skillStorage;
    [SerializeField] private Transform _inventory;

    [SerializeField] private bool _isBoss;
    public bool IsBoss => _isBoss;

    public bool ActiveSpecialDeath { get; set; }

    public string Id => _id;
    public Line Line => _line;

    #endregion

    #region Stats

    public IntStat Health { get; private set; }
    public IntStat Attack { get; private set; }
    public IntStat Defense { get; private set; }
    public IntStat Mp { get; private set; }
    public IntStat Exp { get; private set; }
    public IntStat Level { get; private set; }
    public IntStat StageLevel { get; private set; }
    public FloatStat Speed { get; private set; }
    public FloatStat AttackSpeed { get; private set; }
    public FloatStat AttackRange { get; private set; }
    public FloatStat CriticalRate { get; private set; }

    public FloatStat CriticalPercent { get; private set; }
    public FloatStat AttackStunRate { get; private set; }
    public FloatStat LifestealRate { get; private set; }
    public FloatStat LifestealPercent { get; private set; }

    public void SetupStats(IStats stats)
    {
        // 이거 굳이 Setup 메서드가 필요한가? new 해버려도 메모리 부하 없을거 같은데..
        Health = new(stats.Health.Value);
        Mp = new(0);
        Mp.SetMaxValue(stats.Mp.Value);
        Health.SetMaxValue(stats.Health.Value);
        Attack = new(stats.Attack.Value);
        Defense = new(stats.Defense.Value);
        Speed = new(stats.Speed.Value);
        AttackSpeed = new(stats.AttackSpeed.Value);
        Exp = new(stats.Exp.Value);
        Exp.SetMaxValue(100);
        Level = new(stats.Level.Value);
        StageLevel = new(0);
        AttackSpeed.OnValueChanged += (value) => { _animator.SetFloat("AttackSpeed", value); };
        _animator.SetFloat("AttackSpeed", AttackSpeed.Value);

        AttackRange = new(stats.AttackRange.Value);
        CriticalRate = new(stats.CriticalRate.Value);
        CriticalPercent = new(stats.CriticalPercent.Value);
        AttackStunRate = new(stats.AttackStunRate.Value);
        LifestealRate = new(stats.LifestealRate.Value);
        LifestealPercent = new(stats.LifestealPercent.Value);

        if (_agent != null)
        {
            _agent.speed = Speed.Value;
        }

        _stunDuration = 0;

        IsDeath = false;
    }

    public void UpdateStats(string key, IStats stats)
    {
        Health.Update(key, stats.Health.Value);
        Attack.Update(key, stats.Attack.Value);
        Defense.Update(key, stats.Defense.Value);
        Mp.Update(key, stats.Mp.Value, StatValueType.Max);
        Speed.Update(key, stats.Speed.Value);
        Exp.Update(key, stats.Exp.Value, StatValueType.Max);
        Level.Update(key, stats.Level.Value);
        StageLevel.Update(key, stats.StageLevel.Value);
        AttackSpeed.Update(key, stats.AttackSpeed.Value);
        AttackRange.Update(key, stats.AttackRange.Value);
        CriticalRate.Update(key, stats.CriticalRate.Value);
        CriticalPercent.Update(key, stats.CriticalPercent.Value);
        AttackStunRate.Update(key, stats.AttackStunRate.Value);
        LifestealRate.Update(key, stats.LifestealRate.Value);
        LifestealPercent.Update(key, stats.LifestealPercent.Value);
    }

    public void ResetStats(string key)
    {
        Health.Reset(key);
        Attack.Reset(key);
        Defense.Reset(key);
        Mp.Reset(key);
        Mp.Reset(key, StatValueType.Max);
        Speed.Reset(key);
        Exp.Reset(key);
        Level.Reset(key);
        StageLevel.Reset(key);
        AttackSpeed.Reset(key);
        AttackRange.Reset(key);
        CriticalRate.Reset(key);
        CriticalPercent.Reset(key);
        AttackStunRate.Reset(key);
        LifestealRate.Reset(key);
        LifestealPercent.Reset(key);
    }

    #endregion

    private void Awake()
    {
        _fsm = GetComponent<FSM>();
        _hasHitState = GetComponent<HitState>();
        _agent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _targetDetector = GetComponent<TargetDetector>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _animator = _model.GetComponent<Animator>();

        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnDeath.ToString(), ExpUp);
    }

    private void OnDisable()
    {
        if (Team == Team.Enemy) return;
        GameEventSystem.Instance.Unsubscribe(ProcessEvents.ProcessEvent_SetActive.ToString(), SetActiveEvent);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnDeath.ToString(), ExpUp);
    }

    public void OnInitialized(UnitData data, Team team)
    {
        if (IsDeath)
        {
            ResetStats("Engage");
            UnitFactory.Instance.GoToSpawnPoint(this);
            _fsm.UnlockState();
        }

        _fsm?.StartState<IdleState>();

        _hitPrefab ??= data.HitPrefab;
        _projectilePrefab ??= data.ProjectilePrefab;
        _warningPrefab ??= data.WarningPrefab;
        _dropExp = data.dropExp;
        _fsm.enabled = false;
        Team = team;

        SetupStats(data);

        if (team == Team.Friendly)
        {
            GameEventSystem.Instance.Subscribe(ProcessEvents.ProcessEvent_SetActive.ToString(), SetActiveEvent);
        }
        else
        {
            SetActive(true);
        }
    }

    private void SetActiveEvent(GameEvent gameEvent)
    {
        SetActive((bool)gameEvent.args);
    }

    private void SetActive(bool isActive)
    {
        IsActive = isActive;
        _agent.enabled = true;
        _fsm.enabled = IsActive;

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_SetActive.ToString(), new GameEvent
        {
            args = new SetActiveEventArgs()
            {
                publisher = this,
                isActive = IsActive
            },
            eventType = UnitEvents.UnitEvent_SetActive.ToString()
        });

        if (!IsActive)
        {
            ResetStats("Engage");
            UnitFactory.Instance.GoToSpawnPoint(this);
        }
        else
        {
            ResetStats("Ready");
            _fsm.UnlockState();
        }
    }

    #region Movement

    /// <summary>
    /// 타겟 방향으로 이동
    /// </summary>
    /// <param name="target"></param>
    public void MoveFromTarget(Transform target)
    {
        IsHit = false;
        _agent.isStopped = false;
        _agent.speed = Speed.Value;
        _agent.SetDestination(target.position);
        Rotation(target.position - transform.position);
    }

    public void Warp(Vector3 pos)
    {
        _agent.Warp(pos);
    }

    public void DashToTarget(DashData data, Action callback = null)
    {
        if (!IsActive) return;
        if (TryGetComponent<DashState>(out var state))
        {
            var speed = data.DashSpeed;
            var distance = data.AdditionalDistance;
            state.OnDash(this, speed, distance, callback);
        }
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

    public void AddForce(Unit killer)
    {
        var directionX = killer.transform.position.x - transform.position.x >= 0 ? -1 : 1;
        var directionY = UnityEngine.Random.Range(0, 2f);

        Vector2 forceVec = new Vector2(directionX, directionY).normalized * 20;

        _rigidbody.AddForce(forceVec, ForceMode2D.Impulse);
    }

    #endregion

    #region FSM

    public void OnHit(int damage, Unit attacker = null)
    {
        if (!IsActive) return;
        if (IsDeath) return;

        if (_hasHitState)
        {
            _fsm.TransitionTo<HitState>();
        }

        IsHit = true;

        var realDamage = damage - Defense.Value;

        damage = realDamage <= 0 ? 1 : realDamage;

        attacker?.TryLifeSteal(damage);

        Health.Update("Engage", -damage);

        if (_hitPrefab != null)
        {
            GameObject hitEffect = ResourceManager.Instance.Spawn(_hitPrefab);
            hitEffect.transform.position = transform.position;
        }
        else
        {
            //Debug.Log("hitPrefab이 없습니다");
        }

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_OnHit.ToString(), new GameEvent
        {
            eventType = UnitEvents.UnitEvent_OnHit.ToString(),
            args = new OnHitEventArgs { publisher = this, damageValue = damage }
        });

        if (Health.Value <= 0)
        {
            OnDeath(attacker, ActiveSpecialDeath);
            //OnDeath(attacker, true);
            //죽었다 하면 무조껀 스페셜 데스 하게 만드는 코드
        }
    }

    public void TryLifeSteal(int damage, Unit healer = null)
    {
        if (LifeStealOperator.IsLifeSteal(LifestealRate.Value))
        {
            var realHealValue = LifeStealOperator.LifeStealForDamage(damage, LifestealPercent.Value);
            OnHeal(realHealValue);
        }
    }

    public void OnHeal(int healValue, Unit healer = null)
    {
        if (healValue > Health.Max - Health.Value)
        {
            healValue = Health.Max - Health.Value;
        }

        Health.Update("Engage", healValue);
    }

    public void OnDeath(Unit killer = null, bool isSpecialDeath = false)
    {
        if (IsDeath) return;
        Health.Reset("Engage");
        IsDeath = true;
        IsActive = false;
        _agent.enabled = false;

        if (_isBoss)
        {
            TimeManager.Instance.SlowMotion();
        }

        if (!isSpecialDeath)
        {
            _fsm.TransitionTo<DeathState>();
        }
        else
        {
            AddForce(killer);
            TimeManager.Instance.FreezeTime(500);
            CameraController.Instance.Shake(8, 0.05f);
            _fsm.TransitionTo<SpecialDeathState>();
        }
    }

    public void OnStun(float stunDuration = 3)
    {
        if (!IsActive) return;
        if (IsStun) return;
        if (IsDeath) return;
        if (IsSuperArmor) return;

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_OnStun.ToString(), new GameEvent
        {
            eventType = UnitEvents.UnitEvent_OnStun.ToString(),
            args = new UnitEventArgs { publisher = this }
        });

        if (TryGetComponent<StunState>(out var state))
        {
            state.OnStun(stunDuration);
        }

        //_fsm.OnStun(stunDuration);
        //_fsm.LockState();
        //Invoke("Melt", stunDuration);
    }
    //public void Melt()
    //{
    //    _fsm.UnlockState();
    //    IsStun = false;
    //}

    public void OnAerial()
    {
        if (IsDeath) return;
        _hasAerialState = true;
        _fsm.TransitionTo<AerialState>();
    }

    #endregion

    #region Animator

    public void CrossFade(int state, float fadeTime)
    {
        if (IsStun) return;
        _animator.CrossFade(state, fadeTime);
    }

    public void CrossFade(string state, float fadeTime)
    {
        if (IsStun) return;
        _animator.CrossFade(state, fadeTime);
    }

    public bool IsUpdateState(string state)
    {
        return _animator.GetBool(state);
    }

    public int GetAttackState() => _animator.GetInteger("AttackState");

    #endregion

    #region Skill

    public void AddSkillMp(int mpValue)
    {
        if (Mp.Max < Mp.Value + mpValue) return;

        Mp.Update("Engage", mpValue);

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_Mana_Regen.ToString(),
            new GameEvent //따로 이벤트 나눈건 아군의 Mp를 추가 해 줄 수 있기 때문.
            {
                eventType = UnitEvents.UnitEvent_Mana_Regen.ToString(),
                args = new UnitEventArgs() { publisher = this }
            });
    }

    public void AddSkill(SkillData skillData)
    {
        if (_skillDic.TryGetValue(skillData.Id, out var skill))
        {
            // 해당 스킬을 가지고 있으면, 그 스킬의 레벨을 올려준다.
            skill.LevelUp();
        }
        else
        {
            var skillObj = ResourceManager.Instance.Spawn(skillData.Prefab);
            var skillComponent = skillObj.GetComponent<Skill>();
            skillComponent.Setup(this);

            _skillDic.Add(skillData.Id, skillComponent);

            skillObj.transform.SetParent(_skillStorage);

            GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_RootSkill.ToString(),
                new GameEvent
                {
                    eventType = UnitEvents.UnitEvent_RootSkill.ToString(),
                    args = skillData
                });
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
            ResetStats(item.Data.Id);
            ResourceManager.Instance.Destroy(equipment.gameObject);
            _equipments[item.Data.PartType] = item;
        }
        else
        {
            _equipments.Add(item.Data.PartType, item);
        }

        UpdateStats(item.Data.Id, item.Data);

        spawnItem.transform.SetParent(_inventory);
    }

    #endregion

    #region Level

    public void LevelUp(int value)
    {
        Exp.SetMaxValue(Exp.Value * 2);
        Exp.Update("Main", -Exp.Value);
        Level.Update("Main", value);
        StageLevel.Update("Ready", value);
    }

    public void ExpUp(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;
        Unit unit = unitEventArgs.publisher;

        Exp.Update("Main", unit.DropExp); //Levelup 하고나면 0

        if (Exp.Value < Exp.Max) return;
        
        int levelUp = unit.DropExp / Exp.Max;

        if (levelUp > 0)
        {
            LevelUp(levelUp);
        }
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