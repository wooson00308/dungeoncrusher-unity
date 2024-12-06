using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSlider : BaseView
{
    protected RectTransform _rectTransform;

    [SerializeField] protected Vector2 _pivot;

    [SerializeField] protected Unit _unit;
    public Unit Unit => _unit;

    protected enum Sliders
    {
        Hp_Slider,
        Mp_Slider
    }

    protected virtual void Awake()
    {
        BindUI();
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void BindUI()
    {
        Bind<Slider>(typeof(Sliders));
    }

    public abstract void Show(Unit unit);

    protected virtual void FixedUpdate()
    {
        MoveSlider();

        if (!_unit.IsActive)
        {
            ResourceManager.Instance.Destroy(gameObject);
            return;
        }

        UpdateSlider();
    }

    protected abstract void UpdateSlider();

    private void MoveSlider()
    {
        _rectTransform.anchoredPosition =
            (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas, _unit.transform.position) +
            _pivot;
    }
}