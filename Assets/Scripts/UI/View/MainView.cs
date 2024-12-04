using TMPro;

public class MainView : BaseView
{
    public enum Texts
    {
        Txt_Stage_Value
    }

    private void OnEnable()
    {
        BindUI();
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
}