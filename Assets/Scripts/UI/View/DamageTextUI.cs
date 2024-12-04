using TMPro;
using UnityEngine;

public class DamageTextUI : BaseView
{
    private RectTransform _rectTransform;

    [SerializeField] private Vector2 _pivot;

    public enum Texts
    {
        Damage_Text
    }

    public override void BindUI()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void Awake()
    {
        BindUI();

        _rectTransform = GetComponent<RectTransform>();
    }

    public void Show(int damage, Vector3 worldPos)
    {
        _rectTransform.SetParent(UIManager.Instance.Root.canvas.transform);
        _rectTransform.anchoredPosition = (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas, worldPos) + _pivot;
        
        Get<TextMeshProUGUI>((int)Texts.Damage_Text).SetText($"{damage}");
    }
}
