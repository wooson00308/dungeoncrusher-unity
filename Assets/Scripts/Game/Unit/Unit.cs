using System;
using System.Collections.Generic;
using System.Linq;
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

    public const int MOVE_SPEED_FACTOR = 100;
    public Team Team { get; set; }

    private float _stunDuration;

    private Dictionary<PartType, Item> _equipments = new();
    public Dictionary<PartType, Item> Equipment => _equipments;
    private Dictionary<int, Skill> _skillDic = new();
    public Dictionary<int, Skill> SkillDic => _skillDic;

    private TargetDetector _targetDetector;
    private NavMeshAgent _agent;
    private Animator _animator;
    private FSM _fsm;
    private Rigidbody2D _rigidbody;

    private Sprite _icon;
    private GameObject _hitPrefab;
    private GameObject _projectilePrefab;
    private GameObject _warningPrefab;
    private int _dropExp;

    private Unit _attacker;
    public Unit Attacker => _attacker;

    private Unit _killer;
    public Unit Killer => _killer;

    public Sprite Icon => _icon;

    private bool _hasHitState;
    private bool _isAerial;
    public float StunDuration => _stunDuration;

    public Unit Target => _targetDetector.Target;

    public GameObject ProjectilePrefab => _projectilePrefab;
    public GameObject WarningPrefab => _warningPrefab;
    public int DropExp => _dropExp;

    public bool IsStun { get; set; }
    public bool IsAerial { get; set; }
    public bool IsHit { get; private set; }
    public bool IsDeath { get; private set; }
    public bool IsActive { get; set; }
    public bool IsSuperArmor { get; set; }

    private int _reviveCount = 0;
    public int ReviveCount => _reviveCount;
    public void AddRevive() => _reviveCount++;

    [Header("Config")] [SerializeField] private string _id;
    [SerializeField] private Line _line;
    [SerializeField] private UnitAnimator _model;
    public UnitAnimator Model => _model;
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
    public FloatStat AP { get; private set; }
    public FloatStat AD { get; private set; }
    public IntStat Defense { get; private set; }
    public IntStat Mp { get; private set; }
    public FloatStat MpPercent { get; private set; }
    public IntStat Exp { get; private set; }
    public FloatStat ExpPercent { get; private set; }
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
        Health.SetMaxValue(stats.Health.Value);
        Mp = new(0);
        Mp.SetMaxValue(stats.Mp.Value);
        MpPercent = new(1);
        Attack = new(stats.Attack.Value);
        AP = new(stats.AP.Value);
        AD = new(stats.AD.Value);
        Defense = new(stats.Defense.Value);
        Speed = new(stats.Speed.Value);
        AttackSpeed = new(stats.AttackSpeed.Value);
        Exp = new(stats.Exp.Value);
        Exp.SetMaxValue(100);
        ExpPercent = new(1);
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

    public void UpdateStats(string key, IStats stats, bool isStatTable = false)
    {
        Health.Update(key, stats.Health.Value);
        Attack.Update(key, stats.Attack.Value);
        AP.Update(key, stats.AP.Value);
        AD.Update(key, stats.AD.Value);
        Defense.Update(key, stats.Defense.Value);
        Mp.Update(key, stats.Mp.Value, StatValueType.Max);
        MpPercent.Update(key, stats.MpPercent.Value);
        Speed.Update(key, stats.Speed.Value);
        Exp.Update(key, stats.Exp.Value, StatValueType.Max);
        ExpPercent.Update(key, stats.ExpPercent.Value);
        Level.Update(key, stats.Level.Value);
        StageLevel.Update(key, stats.StageLevel.Value);
        AttackSpeed.Update(key, stats.AttackSpeed.Value);
        AttackRange.Update(key, stats.AttackRange.Value);
        CriticalRate.Update(key, stats.CriticalRate.Value);
        CriticalPercent.Update(key, stats.CriticalPercent.Value);
        AttackStunRate.Update(key, stats.AttackStunRate.Value);
        LifestealRate.Update(key, stats.LifestealRate.Value);
        LifestealPercent.Update(key, stats.LifestealPercent.Value);

        if (isStatTable)
        {
            if (stats.Health.Value <= 0) return;
            Health.SetMaxValue(Health.Max + stats.Health.Value);
        }
    }

    public void ResetStats(string key)
    {
        Health.Reset(key);
        Attack.Reset(key);
        AP.Reset(key);
        AD.Reset(key);
        Defense.Reset(key);
        Mp.Reset(key);
        Mp.Reset(key, StatValueType.Max);
        MpPercent.Reset(key);
        Speed.Reset(key);
        Exp.Reset(key);
        ExpPercent.Reset(key);
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

    public void UpdateCriticalRate(string key, float value)
    {
        CriticalRate.Update(key, value);
    }

    public void UpdateAttack(string key, int value)
    {
        Attack.Update(key, value);
    }

    public void UpdateMaxHealth(int value)
    {
        Health.SetMaxValue(value);
        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_Health_Regen);
    }

    public void UpdateMpPercent(string key, float value)
    {
        MpPercent.Update(key, value);
    }

    public void UpdateExpPercent(string key, int value)
    {
        ExpPercent.Update(key, value);
    }

    public void UpdateAP(string key, float value)
    {
        AP.Update(key, value);
        // Debug.Log(AP.Value);
    }

    #endregion

    public void SetVisialbe(bool value)
    {
        _model.SetVisialbe(value);
    }

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
    }

    private void OnDisable()
    {
        IsActive = false;
        if (Team == Team.Enemy) return;
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_SetActive, SetActiveEvent);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath, ExpUp);
        ResetItemNSkills();
    }

    public void OnInitialized(UnitData data, Team team)
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath, ExpUp);

        if (IsDeath)
        {
            ResetStats("Engage");
            UnitFactory.Instance.GoToSpawnPoint(this);
            _fsm.UnlockState();
        }

        _fsm?.StartState<IdleState>();
        _icon ??= data.Icon;
        _hitPrefab ??= data.HitPrefab;
        _projectilePrefab ??= data.ProjectilePrefab;
        _warningPrefab ??= data.WarningPrefab;
        _dropExp = data.dropExp;
        _fsm.enabled = false;
        Team = team;

        SetupStats(data);

        if (team == Team.Friendly)
        {
            GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_SetActive, SetActiveEvent);
        }
        else
        {
            SetActive(true);
        }

        ResetStats("Main");
    }

    private void ResetItemNSkills()
    {
        if (Team == Team.Enemy) return;

        foreach (var equipment in _equipments)
        {
            if (equipment.Value.Data.Health.Value != 0)
            {
                Health.SetMaxValue(Health.Max - equipment.Value.Data.Health.Value);
            }

            ResetStats(equipment.Value.Data.Id);
        }

        _equipments.Clear();
        _equipments = new();
        _skillDic.Clear();
        _skillDic = new();

        var skills = _skillStorage.GetComponentsInChildren<Transform>(true).ToList();
        skills.Remove(skills[0]);

        foreach (var skill in skills)
        {
            Destroy(skill.gameObject);
        }
    }

    private void SetActiveEvent(object gameEvent)
    {
        SetActive((bool)gameEvent);
    }

    private void SetActive(bool isActive)
    {
        IsActive = isActive;
        _agent.enabled = true;
        _fsm.enabled = IsActive;

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_SetActive, new SetActiveEventArgs()
        {
            publisher = this,
            isActive = IsActive
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
        _agent.speed = (Speed.Value * MOVE_SPEED_FACTOR) * GameTime.DeltaTime;
        _agent.SetDestination(target.position);
        Rotation(target.position - transform.position);
    }

    public void Warp(Vector3 pos)
    {
        _agent.Warp(pos);
    }

    public void DashToTarget_old(DashData data, Action callback = null)
    {
        if (!IsActive) return;
        if (TryGetComponent<DashState>(out var state))
        {
            var speed = (data.DashSpeed * MOVE_SPEED_FACTOR) * GameTime.DeltaTime;
            var distance = data.AdditionalDistance;
            state.OnDash(this, speed, distance, callback);
        }
    }

    public void DashToTarget(DashSkillFxEventData data, Action callback = null)
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

    public void OnHit(int damage, Unit attacker = null, bool isCritical = false)
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
        _attacker ??= attacker;

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

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnHit,
            new OnHitEventArgs { publisher = this, damageValue = damage, isCiritical = isCritical });

        // if (IsRevive)
        // {
        //     OnRevive();
        //     // Todo : 부활 로직
        //     return;
        // }

        if (Health.Value <= 0)
        {
            OnDeath(attacker, ActiveSpecialDeath);
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

    public void OnDeath(Unit killer = null, bool isSpecialDeath = false, bool isExecution = false)
    {
        if (IsDeath) return;
        IsDeath = true;

        IsActive = false;
        _agent.enabled = false;
        _killer = killer;

        if (_isBoss)
        {
            TimeManager.Instance.SlowMotion();
        }

        if (isExecution) // 처형
        {
            GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnDeath_Execution,
                new UnitEventOnAttackArgs() //처형이라면 처형 텍스트 띄우기위함.
                {
                    publisher = this
                });
        }

        if (!isSpecialDeath)
        {
            _fsm.TransitionTo<DeathState>();
        }
        else
        {
            if (GameTime.TimeScale != 1)
            {
                TimeManager.Instance.SlowMotion(false);
                CameraController.Instance.ZoomInOut(true);
            }

            AddForce(killer);
            TimeManager.Instance.FreezeTime(0.005f);
            CameraController.Instance.Shake(8, 0.05f);
            _fsm.TransitionTo<SpecialDeathState>();
        }
    }

    public void OnStun(float stunDuration = 1.0f)
    {
        if (!IsActive) return;
        if (IsStun) return;
        if (IsDeath) return;
        if (IsSuperArmor) return;

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_OnStun, new UnitEventArgs { publisher = this });

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
        if (!IsActive) return;
        if (IsStun) return;
        if (IsDeath) return;
        if (IsSuperArmor) return;

        _isAerial = true;

        if (TryGetComponent(out AerialState aerialState))
        {
            _fsm.TransitionTo<AerialState>();
        }
    }

    public void OnRevive()
    {
        IsDeath = false;
        IsActive = true;
        _agent.enabled = true;
        _fsm.UnlockState();
        _reviveCount--;
        _fsm.TransitionTo<IdleState>();
        var reviveHealthValue = (int)(Health.Max * 0.2f);
        Health.Update("Engage", reviveHealthValue);
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

    public void UpdateSkillMp(int mpValue)
    {
        if (mpValue >= 0) //mpValue가 0보다 낮으면 감소라
        {
            if (Mp.Max <= Mp.Value + (int)(mpValue * MpPercent.Value))
            {
                mpValue = Mp.Max;
            }
            else
            {
                mpValue = (int)(mpValue * MpPercent.Value);
            }
        }

        Mp.Update("mp", mpValue);

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_Mana_Regen,
            new UnitEventArgs() { publisher = this });
    }

    public void SetSuperArmor(float time)
    {
        if (IsSuperArmor)
            return;
        IsSuperArmor = true;
        Debug.Log("On superarmor");
        Invoke("InvokeSuperArmor", time);
    }

    public void InvokeSuperArmor()
    {
        IsSuperArmor = false;
        Debug.Log("Off superarmor");
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
            var skillObj = ResourceManager.Instance.Spawn(skillData.Prefab.gameObject);
            var skillComponent = skillObj.GetComponent<Skill>();
            skillComponent.Initialized(this, skillData);

            _skillDic.Add(skillData.Id, skillComponent);

            skillObj.transform.SetParent(_skillStorage);

            GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_RootSkill, skillData);
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
            ResetStats(equipment.Data.Id);

            if (equipment.Data.Health.Value != 0)
            {
                Health.SetMaxValue(Health.Max - equipment.Data.Health.Value);
            }

            ResourceManager.Instance.Destroy(equipment.gameObject);
            _equipments[item.Data.PartType] = item;
        }
        else
        {
            _equipments.Add(item.Data.PartType, item);
        }

        if (item.Data.Health.Value != 0)
        {
            Health.SetMaxValue(Health.Max + item.Data.Health.Value);
        }

        UpdateStats(item.Data.Id, item.Data);

        spawnItem.transform.SetParent(_inventory);
    }

    #endregion

    #region Level

    public void LevelUp(int value)
    {
        Level.Update("Main", value);
        StageLevel.Update("Ready", value);

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_Level, new UnitEventArgs { publisher = this });
    }

    public void ExpUp(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;
        Unit unit = unitEventArgs.publisher;

        if (unit.Team == Team) return;

        var expPercentValue = (int)(unit.DropExp * (ExpPercent.Value * 0.01f));
        Exp.Update("Main", unit._dropExp + expPercentValue); //Levelup 하고나면 0

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_Exp, new UnitEventArgs { publisher = this });

        if (Exp.Value < Exp.Max) return;

        int levelUp = (unit.DropExp + Exp.Value) / Exp.Max;

        if (levelUp > 0)
        {
            LevelUp(levelUp);
            Exp.SetMaxValue(Exp.Value * 2);
            Exp.Update("Main", -Exp.Value);
        }

        GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_Exp, new UnitEventArgs { publisher = this });
    }

    #endregion

    public (int damage, bool isCritical) LastDamage() //기존 UnitAnimator에서 Attack할때만 치명타 계산하면 스킬에서는 활용못해서 변경함. 
    {
        int realDamage;
        bool isCritical = false;
        if (CriticalOperator.IsCritical(CriticalRate.Value))
        {
            realDamage = CriticalOperator.GetCriticalDamageIntValue(Attack.Value, CriticalPercent.Value);
            isCritical = true;
        }
        else
        {
            realDamage = Attack.Value;
        }

        return (realDamage, isCritical);
    }
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