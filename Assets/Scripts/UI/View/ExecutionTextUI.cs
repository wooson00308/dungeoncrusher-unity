using TMPro;
using UnityEngine;

public class ExecutionTextUI : BaseView
{
    private RectTransform _rectTransform;

    [SerializeField] private Vector2 _pivot;

    [SerializeField] private float randomValue;

    public enum Texts
    {
        Execution_Text
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

    public void Show(string name, Vector3 worldPos)
    {
        _rectTransform.SetParent(UIManager.Instance.Root.canvas.transform);
        
        var randomPos = Random.insideUnitCircle * randomValue;
        
        _rectTransform.anchoredPosition =
            (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas, worldPos) + _pivot +
            randomPos;

        var damageTxt = Get<TextMeshProUGUI>((int)Texts.Execution_Text);

        damageTxt.enableVertexGradient =true;

        damageTxt.SetText($"{name}");
    }
}