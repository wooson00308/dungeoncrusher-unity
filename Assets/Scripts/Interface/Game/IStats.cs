using UnityEngine;

public interface IStats
{
    public int Health { get; }
    public int Attack { get; }
    public int Defense { get; }
    public float Speed { get; }
    public float AttackSpeed { get; }
    public float AttackRange { get; }

    public float CriticalRate { get; }
    public float AttackStunRate { get; }
    public float LifestealRate { get; }
}


public interface IStatSetable
{
    public void SetupStats(IStats stats);
}

public interface IStatUpdatable
{
    public void IncreaseStats(IStats stats);
    public void DecreaseStats(IStats stats);
}