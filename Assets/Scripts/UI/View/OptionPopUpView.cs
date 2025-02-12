using UnityEngine.UI;

public class OptionPopUpView : BaseView
{
    private OptionPopUpUI optionPopUpUI;

    private bool bgmIsMute = false;
    private bool fxIsMute = false;

    public enum Buttons
    {
        BGMButton,
        FxButton
    }

    public enum Sliders
    {
        BGMVolumeSlider,
        FxVolumeSlider
    }

    private void OnEnable()
    {
        BindUI();
        Get<Slider>((int)Sliders.BGMVolumeSlider).onValueChanged.AddListener(delegate { ChangeBGMSliderValue(); });
        Get<Slider>((int)Sliders.FxVolumeSlider).onValueChanged.AddListener(delegate { ChangeFxSliderValue(); });
        Get<Button>((int)Buttons.BGMButton).onClick.AddListener(OnClickBGMMute);
        Get<Button>((int)Buttons.FxButton).onClick.AddListener(OnClickFxMute);
    }

    private void Awake()
    {
        optionPopUpUI = GetComponent<OptionPopUpUI>();
    }

    public override void BindUI()
    {
        Bind<Slider>(typeof(Sliders));
        Bind<Button>(typeof(Buttons));
    }

    private void OnClickBGMMute()
    {
        bgmIsMute = !bgmIsMute;
        optionPopUpUI.BGMMute(bgmIsMute);
    }

    private void OnClickFxMute()
    {
        fxIsMute = !fxIsMute;
        optionPopUpUI.FxMute(fxIsMute);
    }

    private void ChangeBGMSliderValue()
    {
        var sliderValue = Get<Slider>((int)Sliders.BGMVolumeSlider).value;
        optionPopUpUI.ChangeBGMSliderValue(sliderValue);
    }

    private void ChangeFxSliderValue()
    {
        var sliderValue = Get<Slider>((int)Sliders.FxVolumeSlider).value;
        optionPopUpUI.ChangeFxSliderValue(sliderValue);
    }
}