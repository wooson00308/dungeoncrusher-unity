using UnityEngine;

public interface IStats
{
    public IntStat Health { get; }
    public IntStat Attack { get; }
    public IntStat Defense { get; }
    public FloatStat Speed { get; }
    public FloatStat AttackSpeed { get; }
    public FloatStat AttackRange { get; }

    public FloatStat CriticalRate { get; }
    public FloatStat AttackStunRate { get; }
    public FloatStat LifestealRate { get; }
}


public interface IStatSetable
{
    public void SetupStats(IStats stats);
}

public interface IStatUpdatable
{
    public void UpdateStats(string key, IStats stats);
    public void ResetStats(string key);
}