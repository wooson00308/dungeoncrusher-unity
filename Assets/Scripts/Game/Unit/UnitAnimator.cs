using System;
using UnityEngine;
using UnityEngine.Rendering;

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

    public void AttackEvent(AnimationEvent e)
    {
        _owner.Target?.OnHit(_owner.Attack.Value, _owner);
        SoundSystem.Instance.PlayFx("AttackSound1"); //AnimationEvent string으로 사운드 받으면 될듯

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_Attack.ToString(), new GameEvent
        {
            args = new UnitEventArgs
            {
                publisher = _owner
            },
            eventType = UnitEvents.UnitEvent_Attack.ToString()
        });
        _owner.AddSkillMp(10); //AnimationEvent Int 파라미터로 받는게 좋을 듯
    }

    public void DeathEvent(AnimationEvent e)
    {
        UnitFactory.Instance.Destroy(_owner.Id, _owner);

        GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_OnDeath.ToString(), new GameEvent
        {
            eventType = UnitEvents.UnitEvent_OnDeath.ToString(),
            args = new UnitEventArgs { publisher = _owner }
        });
    }

    private void OrderSprite()
    {
        _sortingGroup.sortingOrder = (int)(_owner.transform.position.y * -100);
    }
}