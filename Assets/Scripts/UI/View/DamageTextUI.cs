using TMPro;
using UnityEngine;

public class DamageTextUI : BaseView
{
    private RectTransform _rectTransform;

    [SerializeField] private Vector2 _pivot;

    [SerializeField] private float randomValue;

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

    public void Show(int damage, Vector3 worldPos, bool isCritical = false)
    {
        _rectTransform.SetParent(transform.parent);
        
        var randomPos = Random.insideUnitCircle * randomValue;
        _rectTransform.anchoredPosition =
            (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas, worldPos) + _pivot +
            randomPos;

        var damageTxt = Get<TextMeshProUGUI>((int)Texts.Damage_Text);

        if (isCritical)
        {
            damageTxt.enableVertexGradient = true;
            damageTxt.color = Color.yellow;
        }
        else
        {
            damageTxt.enableVertexGradient = false;
            damageTxt.color = Color.white; //애니메이션 쪽이라 안 바뀜
        }


        damageTxt.SetText($"{damage}");
    }
}