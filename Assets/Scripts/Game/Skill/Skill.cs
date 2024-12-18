using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private Unit _owner;
    private int _skillLevel = 1;
    [SerializeField] private SkillData _skillData;
    private float _timeMarker;
    private bool _isInitialized = false;
    public SkillData SkillData => _skillData;
    public int Level => _skillLevel;

    public void Setup(Unit owner)
    {
        _owner = owner;
        _isInitialized = true;
    }

    public void SetSkillData(SkillData skillData)
    {
        _skillData = skillData;
    }

    private void OnEnable()
    {
        if (_skillData.IsUltSkill)
        {
            //�ʻ�� ��ų�̸� �̺�Ʈ�� ����ϱ�
        }

        if (!_skillData.IsSelfSkill) // �ڰ� �ߵ� ��ų�̸� �̺�Ʈ ��� �ʿ� ����
        {
            GameEventSystem.Instance.Subscribe(_skillData.SkillEventType.ToString(), TryUseEventSkill);
        }
    }

    private void OnDisable()
    {
        _skillLevel = 1;
        _isInitialized = false;

        if (!_skillData.IsSelfSkill)
        {
            GameEventSystem.Instance.Unsubscribe(_skillData.SkillEventType.ToString(), TryUseEventSkill);
        }
    }

    public void LevelUp(int value = 1)
    {
        _skillLevel += value;
    }

    public void TryUseEventSkill(GameEvent gameEvent)
    {
        UnitEventArgs args = (UnitEventArgs)gameEvent.args;

        if (args == null) return;
        if (!args.publisher.GetInstanceID().Equals(_owner.GetInstanceID())) return;
        if (gameEvent.eventType != _skillData.SkillEventType.ToString()) return;
        if (args.publisher.Target == null) return;

        UseSkill(args.publisher);
    }

    public void TryUseSkillWheenCoolTimeReady()
    {
        if (!_skillData.IsSelfSkill) return; // ��ų�� Ư�� ������ �ƴ� �ڰ��ߵ� ��ų���� üũ 
        UseSkill(_owner);
    }

    /// <summary>
    /// ��ų ��� �ߺ� �ڵ� �޼���ȭ
    /// </summary>
    /// <param name="user"></param>
    private void UseSkill(Unit user)
    {
        if (!IsUseableSkill()) return;

        _timeMarker = Time.time; // ��ų ��Ÿ�� �ʱ�ȭ

        if (user.Target == null) return;

        #region ��ų ������ ���Ἲ �˻�

        var skillLevelDetails = _skillData.GetSkillLevelData(_skillLevel);

        GameObject skillFxPrefab = skillLevelDetails.skillFxPrefab;
        if (skillFxPrefab == null) return;

        var skillFxObject = ResourceManager.Instance.Spawn(skillFxPrefab);
        if (skillFxObject == null)
        {
            ResourceManager.Instance.Destroy(skillFxPrefab);
            return;
        }

        if (!skillFxObject.TryGetComponent<SkillEffect>(out var skillFx)) return;

        #endregion

        if (!Operator.IsRate(skillLevelDetails.activationChance)) return;

        skillFxObject.transform.position = user.Target.transform.position;
        if (_skillData.IsAreaAttack)
        {
            HashSet<Unit> enemies = UnitFactory.Instance.GetUnitsExcludingTeam(user.Team);

            // TODO : ���� �ʿ� -> ���� Ÿ�� �ֺ��� ���� �Ÿ��� �����ϴ� ���ֵ��� ���� �������� �ǰ� ���Ѿ� ��.
            // 1. Ÿ���� ã��
            // 2. ���� �� Ÿ�ٰ� �Ÿ��� �����Ÿ� ����� �ֵ��� �����Ͽ�
            // 3. ���� ���� �ǰ�

            // BEFORE
            //List<Unit> targets = enemies.OrderBy(x => Random.value).Take(_skillData.GetSkillLevelData(_skillLevel).targetNum).ToList();

            //AFTER
            // ...
            List<Unit> targets = enemies
                .Where(enemy => Vector3.Distance(enemy.transform.position, user.Target.transform.position) <=
                                _skillData.GetSkillLevelData(_skillLevel).range)
                .OrderBy(x => Random.value)
                .Take(_skillData.GetSkillLevelData(_skillLevel).targetNum)
                .ToList();
            skillFx.Initialized(this, user, _skillData, targets);
        }
        else
        {
            skillFx.Initialized(this, user, _skillData, new List<Unit> { user.Target });
        }
    }

    private void Update()
    {
        TryUseSkillWheenCoolTimeReady();
    }

    /// <summary>
    /// ��ų ��� ���� �Ǵ�
    /// </summary>
    /// <returns></returns>
    private bool IsUseableSkill()
    {
        if (!_isInitialized) // �ʱ�ȭ �Ǿ����� üũ
            return false;

        if (!_owner.IsActive) // ��ų�� ����ϴ� ������ ��Ȱ��ȭ �Ǿ����� Ȯ��. ��Ȱ��ȭ ���� -> ������ �غ� �ܰ�� �Ѿ�� ��Ȱ��ȭ ��.
        {
            ResetCooltime();
            return false;
        }

        if (!_skillData.IsValidTarget(_owner))
            return false;

        if (Time.time - _timeMarker <= _skillData.GetSkillLevelData(_skillLevel).coolTime) // ��Ÿ�� �������� üũ 
            return false;

        return true;
    }

    public void ResetCooltime()
    {
        var cooltime = _skillData.GetSkillLevelData(_skillLevel).coolTime;

        if (Time.time - _timeMarker > cooltime) return;
        _timeMarker -= cooltime;
    }

    // ���� ��� �߿� ���� ������ �������� �ƴ��� üũ�ϴ� ����� �����Ͽ� �ش� ����� ��Ȱ��ȭ�Ͽ����ϴ�.
    //private void Off(GameEvent e)
    //{
    //    OnOff = false;
    //}
    //private void On(GameEvent e)
    //{
    //    _timeMarker = Time.time;
    //    OnOff = true;
    //}
}