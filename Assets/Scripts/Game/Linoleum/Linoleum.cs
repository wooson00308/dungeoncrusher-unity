using System.Linq;
using UnityEngine;

public class Linoleum : MonoBehaviour
{
    [SerializeField] protected LinoleumData _data;
    protected int _realDamage;
    protected Unit _target;
    private bool _isTick = false;

    public void Initialize(int damage)
    {
        _realDamage = damage * (int)_data.tickDamagePercent;
    }

    protected void FixedUpdate()
    {
        if (IsTargetInSight())
        {
            if (_isTick) return;
            Tick();
        }
    }

    protected async void Tick()
    {
        _isTick = true;
        await Awaitable.WaitForSecondsAsync(_data.tickInterval);
        _target?.OnHit(_realDamage);
        _isTick = false;
    }

    protected bool IsTargetInSight()
    {
        var players = UnitFactory.Instance.GetTeamUnits(Team.Friendly);
        var player = players?.FirstOrDefault();

        if (player == null)
        {
            _target = null;
            return false;
        }

        if (_target == null)
        {
            _target = player != null ? player : null;
        }

        return Vector3.Distance(transform.position, player.transform.position) <= _data.detectRange;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.detectRange);
    }
}