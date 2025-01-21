using System;
using System.Collections;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    private bool _isDestroy;

    private void OnEnable()
    {
        _isDestroy = false;
        DestroyTime();
    }

    private async void DestroyTime()
    {
        float time = 0;

        while(time < destroyTime)
        {
            if (_isDestroy) return;

            time += GameTime.DeltaTime; 
            await Awaitable.EndOfFrameAsync();
        }

        DestroyThis();
    }

    private void DestroyThis()
    {
        if (_isDestroy) return;
        _isDestroy = true;

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