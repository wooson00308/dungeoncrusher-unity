using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    [SerializeField] private bool isNonGameTime;
    private bool _isDestroy;

    private void OnEnable()
    {
        _isDestroy = false;
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_GameOver, DestroyThis);
        DestroyTime();
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_GameOver, DestroyThis);
    }

    private async void DestroyTime()
    {
        if (isNonGameTime)
        {
            await Awaitable.WaitForSecondsAsync(destroyTime);
            DestroyThis();
        }
        else
        {
            float time = 0;

            while (time < destroyTime)
            {
                if (_isDestroy) return;

                time += GameTime.DeltaTime;
                await Awaitable.EndOfFrameAsync();
            }

            DestroyThis();
        }
    }

    private void DestroyThis(object gameEvent = null)
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