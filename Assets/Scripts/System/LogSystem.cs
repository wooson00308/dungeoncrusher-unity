using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSystem : MonoBehaviour
{
    [SerializeField] private int _logCount = 4;
    [SerializeField] private Transform _parent;

    private Queue<GameObject> _logImages = new();

    private Queue<UnitEventArgs> _killLogEvents = new();
    private Queue<SkillEventArgs> _skillLogEvents = new();

    private int _currentLogCount = 0;

    private void Awake()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath, Log);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_UseSkill_Publish_UI, Log);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_UseSkill_Publish_UI_Ulti, Log);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath, Log);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_UseSkill_Publish_UI, Log);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_UseSkill_Publish_UI_Ulti, Log);
    }

    public void Log(object gameEvent)
    {
        if (gameEvent is UnitEventArgs unitEventArgs)
        {
            StartCoroutine(LogSpawnProcess(unitEventArgs));
        }
    }

    bool _isRunningDestroyUI;

    private IEnumerator LogSpawnProcess(UnitEventArgs args)
    {
        if (args is SkillEventArgs skillEventArgs)
        {
            _skillLogEvents.Enqueue(skillEventArgs);
        }
        else if (args is UnitEventArgs unitEventArgs)
        {
            _killLogEvents.Enqueue(unitEventArgs);
        }

        LogSpawn();
        
        yield return StartCoroutine(DestroyLogUIWhenEnoughLogs());
    }

    private IEnumerator DestroyLogUIWhenEnoughLogs()
    {
        if (_isRunningDestroyUI || _logImages.Count <= _logCount) yield break;
        _isRunningDestroyUI = true;

        while (_logImages.Count > _logCount)
        {
            var uiObject = _logImages.Dequeue();
            ResourceManager.Instance.DestroyUI(uiObject);
            yield return null;
        }

        _isRunningDestroyUI = false;
    }

    private void LogSpawn()
    {
        while (_killLogEvents.Count > 0)
        {
            var unitEventArgs = _killLogEvents.Dequeue();
            SpawnLogUI(unitEventArgs);
        }

        while (_skillLogEvents.Count > 0)
        {
            var skillEventArgs = _skillLogEvents.Dequeue();
            SpawnLogUI(skillEventArgs);
        }
    }

    private async void SpawnLogUI(UnitEventArgs args)
    {
        GameObject logImageView = ResourceManager.Instance.SpawnFromPath("UI/Pop/LogImage", _parent);
        _logImages.Enqueue(logImageView);
        logImageView.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
        logImageView.transform.SetAsFirstSibling();

        var imageView = logImageView.GetComponent<LogImageView>();
        imageView.Initialize(this);

        if (args is SkillEventArgs skillEventArgs)
        {
            var skillData = skillEventArgs.data;
            imageView.SetLog(skillData.Icon, skillData.Name);
        }
        else if (args is UnitEventArgs unitEventArgs)
        {
            var unit = unitEventArgs.publisher;
            imageView.SetLog(unit.Icon, unit.Id);
        }

        await Awaitable.WaitForSecondsAsync(0.1f);
    }
}