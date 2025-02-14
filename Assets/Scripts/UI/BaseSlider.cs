using UnityEngine;

public abstract class BaseSlider : BaseView
{
    protected Transform _parent;
    protected RectTransform _rectTransform;

    [SerializeField] protected Vector2 _pivot;

    [SerializeField] protected Unit _unit;

    protected virtual void Awake()
    {
        BindUI();
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void BindUI()
    {
    }

    public void OnInitialize(Transform parent)
    {
        _parent = parent;
    }

    public abstract void Show(Unit unit);

    protected virtual void FixedUpdate()
    {
        if (!_unit.IsActive)
        {
            ResourceManager.Instance.Destroy(gameObject);
            return;
        }

        MoveSlider();
        
        UpdateSlider();
    }

    protected abstract void UpdateSlider();

    private void MoveSlider()
    {
        Vector2 movePos;

        var worldToCanvasPoint =
            (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas,
                _unit.transform.position) + _pivot;

        if (_unit.Team == Team.Enemy)
        {
            if (_unit.IsBoss)
            {
                movePos = Vector2.zero + _pivot;
            }
            else
            {
                if (_unit.IsAerial)
                {
                    movePos = (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas,
                        _unit.Model.Body.transform.position) + _pivot * 0.5f;
                }
                else
                {
                    movePos = worldToCanvasPoint;
                }
            }
        }
        else
        {
            movePos = worldToCanvasPoint;
        }

        _rectTransform.anchoredPosition = movePos;
    }
}