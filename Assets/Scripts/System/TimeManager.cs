using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int _gameTimeScale = 1;
    private bool isBossDead = false;

    protected override void Awake()
    {
        base.Awake();
        _gameTimeScale = 1;
        Time.timeScale = _gameTimeScale;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void PlayTime()
    {
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

    public void SlowMotion(bool value)
    {
        if(value)
        {
            Time.timeScale = 0.02f;
        }
        else
        {
            Time.timeScale = _gameTimeScale;
        }
    }

    public async Task FreezeTime(int time)
    {
        Time.timeScale = 0.01f;
        await Task.Delay(time);
        Time.timeScale = _gameTimeScale;
    }

    public int GetGameSpeed()
    {
        return _gameTimeScale;
    }
}