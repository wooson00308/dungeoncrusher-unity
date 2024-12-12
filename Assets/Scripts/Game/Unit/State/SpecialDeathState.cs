using System.Linq;
using UnityEngine;

public class SpecialDeathState : StateBase, IState
{
    [SerializeField] private float forceSpeed = -1;

    public void OnEnter(Unit unit)
    {
        unit.CrossFade("SpecialDeath", 0f);
        Unit playerUnit = UnitFactory.Instance.GetTeamUnits(Team.Friendly).FirstOrDefault();

        float randomYForce = Random.Range(0f, 1f);
        var positionX = playerUnit.transform.position.x - transform.position.x >= 0 ? 1 : -1;
        Vector2 forceVec = new Vector2(positionX * forceSpeed, randomYForce);
        Debug.Log(forceVec);
        unit.Rigidbody.AddForce(forceVec, ForceMode2D.Impulse);
    }

    public void OnUpdate(Unit unit)
    {
    }

    public void OnExit(Unit unit)
    {
    }
}