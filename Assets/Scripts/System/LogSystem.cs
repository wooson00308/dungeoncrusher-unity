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
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;

        if (unitEventArgs != null)
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

            yield return StartCoroutine(DestroyLogUIWheenEnoughLogs());
        }
        else
        {
            _killLogEvents.Enqueue(args);

            yield return StartCoroutine(DestroyLogUIWheenEnoughLogs());
        }

        LogSpawn();
    }

    private IEnumerator DestroyLogUIWheenEnoughLogs()
    {
        if (_isRunningDestroyUI || _logImages.Count <= _logCount) yield break;
        _isRunningDestroyUI = true;

        while (_logImages.Count > _logCount)
        {
            ResourceManager.Instance.DestroyUI(_logImages.Dequeue());
            yield return null;
        }

        _isRunningDestroyUI = false;
    }

    private async void LogSpawn()
    {
        while (_killLogEvents.Count > 0)
        {
            var unitEventArgs = _killLogEvents.Dequeue();

            Unit unit = unitEventArgs.publisher;

            GameObject logImageView = ResourceManager.Instance.SpawnFromPath("UI/Pop/LogImage", _parent);
            _logImages.Enqueue(logImageView);
            logImageView.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
            logImageView.transform.SetAsFirstSibling();
            logImageView.GetComponent<LogImageView>().SetLog(unit.Icon, unit.Id);
            await Awaitable.WaitForSecondsAsync(0.1f);
        }

        while (_skillLogEvents.Count > 0)
        {
            var skillEventArgs = _skillLogEvents.Dequeue();

            SkillData skillData = skillEventArgs.data;

            GameObject logImageView = ResourceManager.Instance.SpawnFromPath("UI/Pop/LogImage", _parent);
            _logImages.Enqueue(logImageView);
            logImageView.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
            logImageView.transform.SetAsFirstSibling();
            logImageView.GetComponent<LogImageView>().SetLog(skillData.Icon, skillData.Name);
            await Awaitable.WaitForSecondsAsync(0.1f);
        }
    }
}