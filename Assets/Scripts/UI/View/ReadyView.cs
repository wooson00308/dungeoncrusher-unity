using TMPro;
using UnityEngine;

public class ReadyView : BaseView
{
    private void OnEnable()
    {
        BindUI();

        DelayEngage();
    }

    private async void DelayEngage()
    {
        await Awaitable.WaitForSecondsAsync(2f);

        GameEventSystem.Instance.Publish(ProcessEvents.Engage.ToString());
    }

    public override void BindUI()
    {
        
    }
}