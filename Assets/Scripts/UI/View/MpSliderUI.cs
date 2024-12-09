using System;
using UnityEngine;
using UnityEngine.UI;

public class MpSliderUI : BaseSlider
{
    private readonly int _maxMp = 100;

    public override void Show(Unit unit)
    {
        _unit = unit;

        _maxMp = _unit.Mp.Max;
        _rectTransform.SetParent(UIManager.Instance.Root.canvas.transform);

        Get<Slider>((int)Sliders.Mp_Slider).value = 0;
    }

    protected override void UpdateSlider()
    {
        var fillAmount = (float)_unit.Mp.Value / _maxMp;

        Get<Slider>((int)Sliders.Mp_Slider).value = fillAmount;
    }
}