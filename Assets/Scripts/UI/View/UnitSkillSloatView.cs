using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitSkillSloatView : BaseView
{
    private Image _image;
    public Image Image => _image;
    private Skill _skill;
    private Unit _player;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _player = UnitFactory.Instance.GetPlayer();
    }

    public void Initialize(Skill skill)
    {
        _skill = skill;
        _image.fillAmount = 1;
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
            _image.fillAmount = 1;
            return;
        }

        float remainCooltime = skill.CooltimeRemain;

        if (remainCooltime <= 0)
        {
            ResetSkillUI();
            return;
        }

        _image.fillAmount = remainCooltime / maxCooltime;
    }

    private void ResetSkillUI()
    {
        _image.fillAmount = 0;
    }
}