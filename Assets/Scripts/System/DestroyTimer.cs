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
        if (TryGetComponent(out RectTransform rectTransform))
        {
            ResourceManager.Instance.DestroyUI(gameObject);
        }
        else
        {
            ResourceManager.Instance.Destroy(gameObject);
        }
    }
}