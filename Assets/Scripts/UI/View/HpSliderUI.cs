using UnityEngine.UI;

public class HpSliderUI : BaseSlider
{
    private int _maxHealth = 0;

    public enum Sliders
    {
        Hp_Slider
    }

    public override void BindUI()
    {
        base.BindUI();
        Bind<Slider>(typeof(Sliders));
    }

    public override void Show(Unit unit)
    {
        _unit = unit;

        _maxHealth = _unit.Health.Value;
        _parent.SetParent(transform.parent);
        var slider = Get<Slider>((int)Sliders.Hp_Slider);
        slider.value = 1;
    }

    protected override void UpdateSlider()
    {
        var fillAmount = (float)_unit.Health.Value / _maxHealth;

        Get<Slider>((int)Sliders.Hp_Slider).value = fillAmount;
    }
}