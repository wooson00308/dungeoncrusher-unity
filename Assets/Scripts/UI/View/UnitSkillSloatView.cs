using UnityEngine.UI;

public class UnitSkillSloatView : BaseView
{
    private Image _image;
    public Image Image => _image;
    private Image _coolTimeImage;
    private Skill _skill;
    private Unit _player;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _coolTimeImage = transform.Find("SkillCoolImage").GetComponent<Image>();
        _player = UnitFactory.Instance.GetPlayer();
        _coolTimeImage.fillAmount = 0;
    }

    public void Initialize(Skill skill)
    {
        _skill = skill;
        _coolTimeImage.fillAmount = 0;
    }

    public override void BindUI()
    {
    }

    private void Update()
    {
        if (_skill == null) return;
        UpdateSkillCooldown(_skill);
    }

    private void UpdateSkillCooldown(Skill skill)
    {
        float maxCooltime = skill.CurrentLevelData.Cooltime;
        if (maxCooltime == 0)
        {
            _coolTimeImage.fillAmount = 0;
            return;
        }

        float remainCooltime = skill.CooltimeRemain;

        if (remainCooltime <= 0)
        {
            ResetSkillUI();
            return;
        }

        _coolTimeImage.fillAmount = remainCooltime / maxCooltime;
    }

    private void ResetSkillUI()
    {
        _coolTimeImage.fillAmount = 0;
    }
}