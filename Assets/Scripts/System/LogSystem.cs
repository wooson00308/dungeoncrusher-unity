using System;
using System.Collections.Generic;
using UnityEngine;

public class LogSystem : MonoBehaviour
{
    [SerializeField] private int _logCount = 4;
    [SerializeField] private Transform _parent;
    private List<GameObject> _logImages = new();

    private int _currentLogCount = 0;

    private void Awake()
    {
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnDeath.ToString(), Log);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnDeath.ToString(), Log);

        _logImages.Clear();
    }

    public void Log(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;

        if (unitEventArgs != null)
        {
            Unit unit = unitEventArgs.publisher;
            Debug.Log(unit.name);

            GameObject logImageView = ResourceManager.Instance.SpawnFromPath("UI/Pop/LogImage", _parent);
            _logImages.Add(logImageView);
            logImageView.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
            logImageView.transform.SetAsFirstSibling();
            logImageView.GetComponent<LogImageView>().SetLog(null, unit.Id);
        }
    }
}