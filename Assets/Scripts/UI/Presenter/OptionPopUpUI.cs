using UnityEngine;

public class OptionPopUpUI : BasePresenter<OptionPopUpView, OptionPopUpModel>
{
    public void ChangeBGMSliderValue(float value)
    {
        _model.ChangeBGMSliderValue(value);
    }

    public void ChangeFxSliderValue(float value)
    {
        _model.ChangeFxSliderValue(value);
    }

    public void BGMMute(bool value)
    {
        _model.BGMMute(value);
    }

    public void FxMute(bool value)
    {
        _model.FxMute(value);
    }
}