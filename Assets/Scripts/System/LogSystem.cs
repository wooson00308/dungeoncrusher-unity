using System;
using System.Collections.Generic;
using UnityEngine;

public class LogSystem : MonoBehaviour
{
    [SerializeField] private int _logCount = 4;
    [SerializeField] private Transform _parent;

    private Queue<GameObject> _logImages = new();
    private Queue<UnitEventArgs> _logEvents = new();

    private int _currentLogCount = 0;

    private void Awake()
    {
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnDeath.ToString(), Log);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnDeath.ToString(), Log);
    }

    public void Log(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;

        if (unitEventArgs != null)
        {
            _logEvents.Enqueue(unitEventArgs);
            
            if (_logImages.Count > _logCount)
            {
                while (_logImages.Count > _logCount)
                {
                    ResourceManager.Instance.DestroyUI(_logImages.Dequeue());
                }

                Debug.Log("Log 최대 갯수보다 많습니다.");
            }
            
            LogSpawn();
        }
    }

    private async void LogSpawn()
    {
        while (_logEvents.Count > 0)
        {
            var unitEventArgs = _logEvents.Dequeue();

            Unit unit = unitEventArgs.publisher;

            GameObject logImageView = ResourceManager.Instance.SpawnFromPath("UI/Pop/LogImage", _parent);
            _logImages.Enqueue(logImageView);
            logImageView.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
            logImageView.transform.SetAsFirstSibling();
            logImageView.GetComponent<LogImageView>().SetLog(null, unit.Id);
            await Awaitable.WaitForSecondsAsync(0.1f);
        }
    }
}