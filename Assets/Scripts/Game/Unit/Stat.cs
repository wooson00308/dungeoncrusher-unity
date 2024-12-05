using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Stat<T>
{
    [SerializeField] protected T _value;

    protected Dictionary<string, T> _updateStats = new();

    public T Value => _value;

    public abstract void Update(string key, T value);
    public abstract void Reset(string key);
    public Stat(T value)
    {
        _value = value;
    }

    public abstract void Setup(T value);
}

[Serializable]
public class IntStat : Stat<int>
{
    public IntStat(int value) : base(value)
    {
    }

    public override void Setup(int value)
    {
        _value = value;

        _updateStats.Clear();
    }

    public override void Reset(string key)
    {
        if (!_updateStats.ContainsKey(key))
        {
            return;
        }

        _value -= _updateStats[key];
        _updateStats.Remove(key);
    }

    public override void Update(string key, int value)
    {
        if(_updateStats.ContainsKey(key))
        {
            _updateStats[key] += value;
        }
        else
        {
            _updateStats.Add(key, value);
        }

        _value += value;
    }
}

[Serializable]
public class FloatStat : Stat<float>
{
    public FloatStat(float value) : base(value)
    {
    }

    public override void Setup(float value)
    {
        _value = value;
    }

    public override void Reset(string key)
    {
        if (!_updateStats.ContainsKey(key))
        {
            return;
        }

        _value -= _updateStats[key];
        _updateStats.Remove(key);
    }

    public override void Update(string key, float value)
    {
        if (_updateStats.ContainsKey(key))
        {
            _updateStats[key] += value;
        }
        else
        {
            _updateStats.Add(key, value);
        }

        _value += value;
    }
}
