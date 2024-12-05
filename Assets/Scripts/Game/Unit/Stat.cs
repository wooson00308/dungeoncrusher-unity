using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Stat<T>
{
    [SerializeField] protected T _value;

    protected Dictionary<string, T> _updateStats = new();

    public T Value => _value;

    public Stat(T value)
    {
        _value = value;
        _updateStats.Clear();
    }

    public virtual void Reset(string key)
    {
        if (!_updateStats.ContainsKey(key))
        {
            return;
        }

        _value = Subtract(_value, _updateStats[key]);
        _updateStats.Remove(key);
    }

    public virtual void Update(string key, T value)
    {
        if (_updateStats.ContainsKey(key))
        {
            _updateStats[key] = Add(_updateStats[key], value);
        }
        else
        {
            _updateStats.Add(key, value);
        }

        _value = Add(_value, value);
    }

    protected abstract T Add(T a, T b);
    protected abstract T Subtract(T a, T b);
}

[Serializable]
public class IntStat : Stat<int>
{
    public IntStat(int value) : base(value) { }

    protected override int Add(int a, int b) => a + b;
    protected override int Subtract(int a, int b) => a - b;
}

[Serializable]
public class FloatStat : Stat<float>
{
    public FloatStat(float value) : base(value) { }

    protected override float Add(float a, float b) => a + b;
    protected override float Subtract(float a, float b) => a - b;
}
