using System;
using System.Collections;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    private void OnEnable()
    {
        DestroyTime();
    }

    private async void DestroyTime()
    {
        await Awaitable.WaitForSecondsAsync(destroyTime);
        DestroyThis();
    }

    private void DestroyThis()
    {
        ResourceManager.Instance.DestroyUI(gameObject);
    }
}