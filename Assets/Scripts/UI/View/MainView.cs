using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseView
{
    public enum Texts
    {
        Txt_Stage_Value,
        Txt_GameSpeed
    }
    public enum Images
    {
        SUPER_ARMOR_IMAGE
    }

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(ProcessEvents.Ready.ToString(), UpdateStageUI);
        GameEventSystem.Instance.Subscribe(UnitEvents.OnDeath_Special.ToString(), SpecialDeathEffect);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(ProcessEvents.Ready.ToString(), UpdateStageUI);
        GameEventSystem.Instance.Unsubscribe(UnitEvents.OnDeath_Special.ToString(), SpecialDeathEffect);
    }

    private void UpdateStageUI(GameEvent gameEvent)
    {
        Get<TextMeshProUGUI>((int)Texts.Txt_Stage_Value).SetText($"{StageManager.Instance.CurrentStage}");
    }

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }

    private void SpecialDeathEffect(GameEvent gameEvent)
    {
        UnitEventArgs unitEventArgs = (UnitEventArgs)gameEvent.args;

        Unit unit = unitEventArgs.publisher;

        var worldToViewportPoint = Camera.main.WorldToViewportPoint(unit.transform.position);
        worldToViewportPoint.x = Mathf.Clamp(worldToViewportPoint.x, 0, 1);
        worldToViewportPoint.y = Mathf.Clamp(worldToViewportPoint.y, 0, 1);

        var viewportToScreenPoint = Camera.main.ViewportToScreenPoint(worldToViewportPoint);

        var spawnPos = new Vector3(viewportToScreenPoint.x, viewportToScreenPoint.y, 0);
        ResourceManager.Instance.SpawnFromPath("UI/Efx_UI_SpecialDeath", transform).transform.position =
            spawnPos;
    }

    public void OnClickChangeGameSpeed()
    {
        GetComponent<MainUI>().ChangeGameSpeed();
        Get<TextMeshProUGUI>((int)Texts.Txt_GameSpeed)
            .SetText($"<size=45>x</size>{GetComponent<MainUI>().GetGameSpeed()}");
    }

    public void OnClickSuperArmor()
    {
        var image = Get<Image>((int)Images.SUPER_ARMOR_IMAGE);
        image.enabled = !image.enabled;
        var friendlys = UnitFactory.Instance.GetTeamUnits(Team.Friendly);
        if (friendlys == null) return;
        var player = friendlys.ToArray()[0];

        player.IsSuperArmor = !player.IsSuperArmor;
    }
}