using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HpSliderUI : BaseView
{
    private RectTransform _rectTransform;

    [SerializeField] private Vector2 _pivot;

    private int _maxHealth;
    [SerializeField] private Unit _unit;
    public Unit Unit => _unit;

    public enum Sliders
    {
        Hp_Slider
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

        _maxHealth = _unit.Health.Value;
        _rectTransform.SetParent(UIManager.Instance.Root.canvas.transform);

        Get<Slider>((int)Sliders.Hp_Slider).value = 1;
    }

    private void FixedUpdate()
    {
        _rectTransform.anchoredPosition =
            (Vector2)Util.WorldToCanvasPoint(Camera.main, UIManager.Instance.Root.canvas, _unit.transform.position) +
            _pivot;

        Get<Slider>((int)Sliders.Hp_Slider).value = (float)_unit.Health.Value / _maxHealth;

        if (_unit.IsDeath)
        {
            ResourceManager.Instance.Destroy(gameObject);
        }
    }
}