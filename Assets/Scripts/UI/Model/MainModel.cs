using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainModel : BaseModel
{
    private void Awake()
    {
    }

    public override void Initialize()
    {
    }

    public void ChangeGameSpeed()
    {
        TimeManager.Instance.ChangeTimeScale();
    }

    public int GetGameSpeed()
    {
        return TimeManager.Instance.GetGameSpeed();
    }
}