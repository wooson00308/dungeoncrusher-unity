using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private Unit _unit;

    public DetectDataBase _detectData;

    private Unit _currentTarget;

    public Unit Target
    {
        get
        {
            HashSet<Unit> enemies = UnitFactory.Instance.GetUnitsExcludingTeam(_unit.Team);
            _currentTarget = _detectData.Detect(_unit, enemies, _currentTarget);
            return _currentTarget;
        }
    }

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }
}

