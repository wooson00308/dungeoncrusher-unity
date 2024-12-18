using TMPro;
using UnityEngine;

public class MainView : BaseView
{
    public enum Texts
    {
        Txt_Stage_Value,
        Txt_GameSpeed
    }

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(ProcessEvents.Ready.ToString(), UpdateStageUI);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(ProcessEvents.Ready.ToString(), UpdateStageUI);
    }

    private void UpdateStageUI(GameEvent gameEvent)
    {
        Get<TextMeshProUGUI>((int)Texts.Txt_Stage_Value).SetText($"{StageManager.Instance.CurrentStage}");
    }

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void OnClickChangeGameSpeed()
    {
        GetComponent<MainUI>().ChangeGameSpeed();
        Get<TextMeshProUGUI>((int)Texts.Txt_GameSpeed)
            .SetText($"<size=45>x</size>{GetComponent<MainUI>().GetGameSpeed()}");
    }
}