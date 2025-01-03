using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSlider : BaseView
{
    protected RectTransform _rectTransform;

    [SerializeField] protected Vector2 _pivot;

    [SerializeField] protected Unit _unit;
    public Unit Unit => _unit;

    protected virtual void Awake()
    {
        BindUI();
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void BindUI()
    {
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
        if (_unit.IsBoss)
        {
            _rectTransform.anchoredPosition = Vector2.zero + _pivot;
            return;
        }

        _rectTransform.anchoredPosition =
            (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas, _unit.transform.position) +
            _pivot;
    }
}