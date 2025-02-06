using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int _timeScale = 1;
    private int _gameTimeScale = 1;
    private bool isBossDead = false;

    protected override void Awake()
    {
        base.Awake();
        _gameTimeScale = 1;
        GameTime.TimeScale = _gameTimeScale;
    }

    public void StopTime()
    {
        GameTime.TimeScale = 0;
    }

    public void PlayTime()
    {
        GameTime.TimeScale = _gameTimeScale;
    }

    public void ChangeTimeScale()
    {
        if (isBossDead) return;
        _timeScale = _timeScale == 1 ? 2 : 1;
        Time.timeScale = _timeScale;
        GameTime.TimeScale = _timeScale;
    }

    public async void SlowMotion()
    {
        isBossDead = true;
        Time.timeScale = 0.2f;
        GameTime.TimeScale = 0.2f;
        await Awaitable.WaitForSecondsAsync(0.5f);
        Time.timeScale = _gameTimeScale;
        GameTime.TimeScale = _gameTimeScale;
        isBossDead = false;
    }

    public void SlowMotion(bool value)
    {
        if (value)
        {
            GameTime.TimeScale = 0.02f;
        }
        else
        {
            GameTime.TimeScale = _gameTimeScale;
        }
    }

    public async void FreezeTime(float time)
    {
        Time.timeScale = 0.01f;
        await Awaitable.WaitForSecondsAsync(time);
        Time.timeScale = _timeScale;
    }

    public int GetGameSpeed()
    {
        return _gameTimeScale;
    }
}