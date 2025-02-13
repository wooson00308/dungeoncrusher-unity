using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseView
{
    private MainUI _presenter;

    public enum Texts
    {
        Txt_Stage_Value,
        Txt_Timer,
        Txt_GameSpeed
    }

    public enum Images
    {
        SUPER_ARMOR_IMAGE
    }

    public enum RectTransforms
    {
        BossCut
    }

    private void Awake()
    {
        _presenter = GetComponent<MainUI>();
        BindUI();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_SetActive, BossWarning);
        GameEventSystem.Instance.Subscribe((int)ProcessEvents.ProcessEvent_Ready, UpdateStageUI);
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_OnDeath_Special, SpecialDeathEffect);
        Get<TextMeshProUGUI>((int)Texts.Txt_GameSpeed)
            .SetText($"<size=45>x</size>{1}");
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_SetActive, BossWarning);
        GameEventSystem.Instance.Unsubscribe((int)ProcessEvents.ProcessEvent_Ready, UpdateStageUI);
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_OnDeath_Special, SpecialDeathEffect);
    }

    private void UpdateStageUI(object gameEvent)
    {
        Get<TextMeshProUGUI>((int)Texts.Txt_Stage_Value).SetText($"{StageManager.Instance.CurrentStage}");
    }

    private void Update()
    {
        Get<TextMeshProUGUI>((int)Texts.Txt_Timer).SetText($"{(int)StageManager.Instance.EnageTime}s");
    }

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<RectTransform>(typeof(RectTransforms));
        Get<RectTransform>((int)RectTransforms.BossCut).gameObject.SetActive(false);
    }

    private void SpecialDeathEffect(object gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent;

        Unit unit = unitEventArgs.Publisher;

        var worldToViewportPoint = Camera.main.WorldToViewportPoint(unit.transform.position);
        worldToViewportPoint.x = Mathf.Clamp(worldToViewportPoint.x, 0, 1);
        worldToViewportPoint.y = Mathf.Clamp(worldToViewportPoint.y, 0, 1);

        var viewportToScreenPoint = Camera.main.ViewportToScreenPoint(worldToViewportPoint);

        var spawnPos = new Vector3(viewportToScreenPoint.x, viewportToScreenPoint.y, 0);
        ResourceManager.Instance.SpawnFromPath("UI/Efx_UI_SpecialDeath", transform).transform.position =
            spawnPos;
    }

    private async void BossWarning(object gameEvent)
    {
        if (gameEvent is UnitEventArgs unitEventArgs)
        {
            var publisher = unitEventArgs.Publisher;
            if (publisher == null) return;
            if (!publisher.IsBoss) return;

            Get<RectTransform>((int)RectTransforms.BossCut).gameObject.SetActive(true);
            TimeManager.Instance.StopTime();
            await Awaitable.WaitForSecondsAsync(2);
            Get<RectTransform>((int)RectTransforms.BossCut).gameObject.SetActive(false);

            GameEventSystem.Instance.Publish((int)ProcessEvents.ProcessEvent_SetActive, true);
            TimeManager.Instance.PlayTime();
        }
    }

    public void OnClickChangeGameSpeed()
    {
        _presenter.ChangeGameSpeed();
        Get<TextMeshProUGUI>((int)Texts.Txt_GameSpeed).SetText($"<size=45>x</size>{Time.timeScale}");
    }

    public void OnClickSuperArmor()
    {
        var image = Get<Image>((int)Images.SUPER_ARMOR_IMAGE);
        image.enabled = !image.enabled;

        var player = UnitFactory.Instance.GetPlayer();
        if (player == null) return;

        player.IsSuperArmor = !player.IsSuperArmor;
    }
}