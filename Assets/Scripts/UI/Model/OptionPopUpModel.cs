public class OptionPopUpModel : BaseModel
{
    public override void Initialize()
    {
    }

    public void ChangeBGMSliderValue(float value)
    {
        SoundSystem.Instance.BGMVolume(value);
    }

    public void ChangeFxSliderValue(float value)
    {
        SoundSystem.Instance.FXVolume(value);
    }

    public void BGMMute(bool value)
    {
        SoundSystem.Instance.BGMMute(value);
    }

    public void FxMute(bool value)
    {
        SoundSystem.Instance.FxMute(value);
    }
}