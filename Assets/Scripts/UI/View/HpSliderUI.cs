using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HpSliderUI : BaseSlider
{
    private int _maxHealth = 0;

    public override void Show(Unit unit)
    {
        _unit = unit;

        _maxHealth = _unit.Health.Value;
        _rectTransform.SetParent(UIManager.Instance.Root.canvas.transform);
        var slider = Get<Slider>((int)Sliders.Hp_Slider);
        slider.value = 1;
    }

    protected override void UpdateSlider()
    {
        var fillAmount = (float)_unit.Health.Value / _maxHealth;

        Get<Slider>((int)Sliders.Hp_Slider).value = fillAmount;
    }
}