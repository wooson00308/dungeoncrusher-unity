using UnityEngine;
using UnityEngine.UI;

public class MpSliderUI : BaseView
{
    private RectTransform _rectTransform;

    [SerializeField] private Vector2 _pivot;

    private int _maxMp = 0;
    [SerializeField] private Unit _unit;
    public Unit Unit => _unit;

    public enum Sliders
    {
        Mp_Slider
    }

    private void Awake()
    {
        BindUI();
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void BindUI()
    {
        Bind<Slider>(typeof(Sliders));
    }

    public void Show(Unit unit)
    {
        _unit = unit;

        if (_maxMp <= unit.MaxMp.Value)
        {
            _maxMp = _unit.MaxMp.Value;
        }

        _rectTransform.SetParent(UIManager.Instance.Root.canvas.transform);

        Get<Slider>((int)Sliders.Mp_Slider).value = 0;
    }

    private void FixedUpdate()
    {
        _rectTransform.anchoredPosition =
            (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas, _unit.transform.position) +
            _pivot;

        var fillAmout = (float)_unit.Mp.Value / _maxMp;

        if (_maxMp <= 0)
        {
            Get<Slider>((int)Sliders.Mp_Slider).value = 0;
        }
        else
        {
            Get<Slider>((int)Sliders.Mp_Slider).value = fillAmout;
        }

        if (_unit.IsDeath)
        {
            ResourceManager.Instance.Destroy(gameObject);
        }
    }
}