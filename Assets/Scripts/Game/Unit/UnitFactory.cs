using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 유닛 팩토리
/// </summary>
public class UnitFactory : SingletonMini<UnitFactory>
{
    private const string UNIT_SPAWN_PATH = "Unit/";

    private readonly List<Unit> _unitList = new();
    private readonly Dictionary<Team, HashSet<Unit>> _teamUnitDic = new();

    private readonly HashSet<Vector2> _spawnedPositionSet = new();
    private readonly List<Vector2> _spawnedPositions = new();

    public Unit GetPlayer()
    {
        return _unitList.Find(x => x.Team == Team.Friendly);
    }

    public HashSet<Unit> GetTeamUnits(Team team)
    {
        if (_teamUnitDic.TryGetValue(team, out HashSet<Unit> units))
        {
            return units.ToHashSet();
        }

        return null;
    }

    public HashSet<Unit> GetUnitsExcludingTeam(Team team)
    {
        var result = new HashSet<Unit>();

        foreach (var kvp in _teamUnitDic)
        {
            if (kvp.Key != team)
            {
                result.UnionWith(kvp.Value);
            }
        }

        return result;
    }

    [Header("설정")] [Tooltip("스폰 포인트 둘레")] public Transform _parent;
    [Tooltip("각 팀별 스폰 설정")] public List<TeamSpawnConfig> _teamSpawnPoints = new();

    /// <summary>
    /// 유닛 생성
    /// </summary>
    /// <param name="unitId"></param>
    /// <param name="team"></param>
    /// <param name="value"></param>
    public void Spawn(UnitData data, Team team, int value)
    {
        int spawnedCount = 0;
        var teamSpawnPoint = _teamSpawnPoints.Find(x => x.team == team);
        var spawnUnits = new List<Unit>();

        while (spawnedCount < value)
        {
            var spawnObj = ResourceManager.Instance.Spawn(data.Prefab, _parent);
            Unit spawnUnit = spawnObj.GetComponent<Unit>();
            spawnUnit.OnInitialized(data, team);

            GoToSpawnPoint(spawnUnit);
            spawnedCount++;

            _unitList.Add(spawnUnit);
            spawnUnits.Add(spawnUnit);

            if (_teamUnitDic.ContainsKey(team))
            {
                var hashSet = _teamUnitDic[team];
                hashSet.Add(spawnUnit);
            }
            else
            {
                _teamUnitDic.Add(team, new HashSet<Unit> { spawnUnit });
            }
        }
    }

    public void GoToSpawnPoint(Unit unit)
    {
        var teamSpawnPoint = _teamSpawnPoints.Find(x => x.team == unit.Team);

        Vector2 randomPosition = Vector2.zero;
        if (teamSpawnPoint.spawnShape == SpawnShape.Circle)
        {
            randomPosition = Util.GetRandomSpawnPositionCircle(
                teamSpawnPoint.spawnPoints[unit.Line],
                teamSpawnPoint.radius
            );
        }
        else if (teamSpawnPoint.spawnShape == SpawnShape.Box)
        {
            randomPosition = Util.GetRandomSpawnPositionBox(
                teamSpawnPoint.spawnPoints[unit.Line],
                teamSpawnPoint.boxSize
            );
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(teamSpawnPoint.spawnShape), "Invalid spawn shape");
        }

        unit.transform.position = randomPosition;
    }


    /// <summary>
    /// 유닛 제거
    /// </summary>
    /// <param name="id"></param>
    /// <param name="unit"></param>
    public void Destroy(string id, Unit unit)
    {
        ResourceManager.Instance.Destroy(unit.gameObject);
        _unitList.Remove(unit);
        _teamUnitDic[unit.Team].Remove(unit);
    }

    /// <summary>
    /// 모든 유닛을 제거
    /// </summary>
    public void KillAllUnits()
    {
        foreach (var team in Enum.GetValues(typeof(Team)))
        {
            KillTeamUnits((Team)team);
        }

        _unitList.Clear();
        _teamUnitDic.Clear();
        _spawnedPositionSet.Clear();
        _spawnedPositions.Clear();
    }

    /// <summary>
    /// 특정 팀 유닛들을 제거
    /// </summary>
    /// <param name="team"></param>
    public void KillTeamUnits(Team team)
    {
        var units = GetTeamUnits(team);

        foreach (var unit in units)
        {
            unit.OnHit(unit.Health.Value);
        }
    }

    public void DestroyTeamUnits(Team team)
    {
        var units = GetTeamUnits(team);

        foreach (var unit in units)
        {
            ResourceManager.Instance.Destroy(unit.gameObject);
        }
    }

    /// <summary>
    /// 디버그 기즈모
    /// </summary>
    void OnDrawGizmos()
    {
        foreach (var teamSpawnPoint in _teamSpawnPoints)
        {
            if (teamSpawnPoint.spawnShape == SpawnShape.Circle)
            {
                // Draw the spawn radius
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(teamSpawnPoint.frontlinePoint, teamSpawnPoint.radius);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(teamSpawnPoint.midlinePoint, teamSpawnPoint.radius);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(teamSpawnPoint.backlinePoint, teamSpawnPoint.radius);
            }
            else if (teamSpawnPoint.spawnShape == SpawnShape.Box)
            {
                // Draw the box
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(teamSpawnPoint.frontlinePoint, teamSpawnPoint.boxSize);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(teamSpawnPoint.midlinePoint, teamSpawnPoint.boxSize);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(teamSpawnPoint.backlinePoint, teamSpawnPoint.boxSize);
            }
        }
    }
}

public enum SpawnShape
{
    Circle,
    Box
}

[System.Serializable]
public class TeamSpawnConfig
{
    [Tooltip("팀 정보")] public Team team;
    [Tooltip("스폰 형태 (Circle/Box)")] public SpawnShape spawnShape = SpawnShape.Circle;

    [Space] [Tooltip("Circle 크기 (가로, 세로)")]
    public float radius = 2;

    [Tooltip("Box 크기 (가로, 세로)")] public Vector2 boxSize = new Vector2(5f, 5f);

    [Space] [Tooltip("앞 라인 스폰 포인트")] public Vector2 frontlinePoint;
    [Tooltip("센터 스폰 포인트")] public Vector2 midlinePoint;
    [Tooltip("뒷 라인 스폰 포인트")] public Vector2 backlinePoint;

    public class SpawnPoints
    {
        private TeamSpawnConfig _parent;

        public SpawnPoints(TeamSpawnConfig parent)
        {
            this._parent = parent;
        }

        public Vector2 this[Line line]
        {
            get
            {
                return line switch
                {
                    Line.Frontline => _parent.frontlinePoint,
                    Line.Midline => _parent.midlinePoint,
                    Line.Backline => _parent.backlinePoint,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(line), "Invalid line type"),
                };
            }
            set
            {
                switch (line)
                {
                    case Line.Frontline:
                        _parent.frontlinePoint = value;
                        break;
                    case Line.Midline:
                        _parent.midlinePoint = value;
                        break;
                    case Line.Backline:
                        _parent.backlinePoint = value;
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(line), "Invalid line type");
                }
            }
        }
    }

    public SpawnPoints spawnPoints;

    public TeamSpawnConfig()
    {
        spawnPoints = new SpawnPoints(this);
    }
}