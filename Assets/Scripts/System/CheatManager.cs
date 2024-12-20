using UnityEditor;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            foreach (var enemyUnit in UnitFactory.Instance.GetTeamUnits(Team.Enemy))
            {
                enemyUnit.OnStun(3);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            foreach (var enemyUnit in UnitFactory.Instance.GetTeamUnits(Team.Enemy))
            {
                enemyUnit.OnAerial();
            }
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            foreach (var enemyUnit in UnitFactory.Instance.GetTeamUnits(Team.Friendly))
            {
                enemyUnit.OnStun(3);
            }
        }
    }
}