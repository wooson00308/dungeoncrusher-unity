using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainModel : BaseModel
{
    private int _gameTimeScale = 1;

    public override void Initialize()
    {
    }

    public void ChangeGameSpeed()
    {
        _gameTimeScale = _gameTimeScale == 1 ? 2 : 1;
        Debug.Log(_gameTimeScale);
        Time.timeScale = _gameTimeScale;
    }

    public int GetGameSpeed()
    {
        return _gameTimeScale;
    }
}