using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : SingletonMini<GameEventSystem>
{
    private readonly Dictionary<int, Action<object>> _eventDictionary = new();

    public void Subscribe(int eventType, Action<object> listener)
    {
        if (!_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = null;
        }

        _eventDictionary[eventType] += listener;
    }

    public void Subscribe(int eventType, params Action<object>[] listeners)
    {
        if (!_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = null;
        }

        foreach (var listener in listeners)
        {
            _eventDictionary[eventType] += listener;
        }
    }

    public void Unsubscribe(int eventType, Action<object> listener)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] -= listener;
            if (_eventDictionary[eventType] == null) // 리스너가 모두 제거되면 삭제
            {
                _eventDictionary.Remove(eventType);
            }
        }
    }

    public void Unsubscribe(int eventType, params Action<object>[] listeners)
    {
        if (_eventDictionary.ContainsKey(eventType))
        {
            foreach (var listener in listeners)
            {
                _eventDictionary[eventType] -= listener;
            }

            if (_eventDictionary[eventType] == null) // 리스너가 모두 제거되면 삭제
            {
                _eventDictionary.Remove(eventType);
            }
        }
    }

    public void Publish(int eventType, object gameEvent = null)
    {
        if (_eventDictionary.TryGetValue(eventType, out var action))
        {
            action?.Invoke(gameEvent); // 즉시 실행
        }
    }
}