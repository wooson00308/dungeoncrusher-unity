using System;
using UnityEngine;

public class BuffManager : SingletonMini<BuffManager>
{
    [SerializeField] private InertiaSkillFxEventData buffFxEventData1;
    [SerializeField] private ChainSpearSkillFxEventData buffFxEventData2;

    private void Start()
    {
        buffFxEventData1.Initialize();
        buffFxEventData2.Initialize();
    }

    private void OnDisable()
    {
        buffFxEventData1.DisEvent();
        buffFxEventData2.DisEvent();
    }
}