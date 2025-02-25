#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class UnitSpawnTest
{
    private ProcessSystem _system;
    private bool _isTestComplelte;

    [SetUp]
    public async void SetUp()
    {
        _isTestComplelte = false;

        SceneManager.LoadScene(0);

        await Awaitable.WaitForSecondsAsync(1f);

        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_AllStageClear, OnTestComplete);
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_GameOver, OnTestComplete);

        _system = GameObject.FindFirstObjectByType<ProcessSystem>();
    }

    private void OnTestComplete(object gameEvent)
    {
        _isTestComplelte = true;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator UnitSpawnTestWithEnumeratorPasses()
    {
        yield return new WaitForSeconds(3f);

        UIManager.Instance.ShowLayoutUI<MainUI>();
        _system.OnNextProcess<ReadyProcess>();

        yield return new WaitUntil(() => _isTestComplelte);
    }
}
#endif