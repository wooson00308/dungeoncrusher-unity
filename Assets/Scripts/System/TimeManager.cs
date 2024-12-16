using System.Collections;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int _gameTimeScale = 1;
    private bool isBossDead = false;

    private void Awake()
    {
        _gameTimeScale = 1;
        Time.timeScale = _gameTimeScale;
    }

    public void ChangeTimeScale()
    {
        if (isBossDead) return;
        _gameTimeScale = _gameTimeScale == 1 ? 2 : 1;
        Time.timeScale = _gameTimeScale;
    }

    public async void SlowMotion()
    {
        isBossDead = true;
        Time.timeScale = 0.2f;
        await Awaitable.WaitForSecondsAsync(0.5f);
        Time.timeScale = _gameTimeScale;
        isBossDead = false;
    }

    public int GetGameSpeed()
    {
        return _gameTimeScale;
    }
}