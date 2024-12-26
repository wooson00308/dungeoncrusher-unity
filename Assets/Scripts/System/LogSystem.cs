using System;
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
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_OnDeath.ToString(), Log);
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_UseSkill_Publish_UI.ToString(), Log);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_OnDeath.ToString(), Log);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_UseSkill_Publish_UI.ToString(), Log);
    }

    public void Log(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;

        if (unitEventArgs != null)
        {
            if (gameEvent.args is SkillEventArgs skillEventArgs)
            {
                _skillLogEvents.Enqueue(skillEventArgs);

                if (_logImages.Count > _logCount)
                {
                    while (_logImages.Count > _logCount)
                    {
                        ResourceManager.Instance.DestroyUI(_logImages.Dequeue());
                    }
                }
            }
            else
            {
                _killLogEvents.Enqueue(unitEventArgs);

                if (_logImages.Count > _logCount)
                {
                    while (_logImages.Count > _logCount)
                    {
                        ResourceManager.Instance.DestroyUI(_logImages.Dequeue());
                    }
                    //Debug.Log("Log 최대 갯수보다 많습니다.");
                }
            }

            LogSpawn();
        }
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