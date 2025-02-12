using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : BaseView
{
    public SkillData _data;

    private bool _isRootSkill;

    private Unit _player;
    private Skill _skill;
    private Button _button;

    public enum Images
    {
        Skill_Icon_Image,
        Skill_Cooltime_Image
    }

    public enum Texts
    {
        Skill_Cooltime_Text
    }

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        _isRootSkill = false;
        Get<Image>((int)Images.Skill_Icon_Image).enabled = false;
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 1;
        Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText($"LOCKED");
        GameEventSystem.Instance.Subscribe((int)UnitEvents.UnitEvent_RootSkill, RootSkillEvent);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe((int)UnitEvents.UnitEvent_RootSkill, RootSkillEvent);
    }

    private void RootSkillEvent(object e)
    {
        var data = e as SkillData;

        if (data == null) return;
        if (data.Id != _data.Id) return;

        Get<Image>((int)Images.Skill_Icon_Image).enabled = true;
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 0;

        if (_player == null)
        {
            _player = UnitFactory.Instance.GetPlayer();
            if (_player.SkillDic.TryGetValue(data.Id, out Skill skill))
            {
                _skill = skill;
            }
        }

        _isRootSkill = true;
    }

    public override void BindUI()
    {
        _button = GetComponent<Button>();
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void OnClick()
    {
        if (GameTime.TimeScale == 0) return;
        if (IsNotEnoughUltiMana) return;

        if (_player.SkillDic.TryGetValue(_data.Id, out Skill skill))
        {
            if (skill.IsCoolingdown) return;

            if (skill.Data.Id == 690) //궁극기 임시 서포트샷
            {
                GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_UseSkill_Publish_UI_Ulti, new SkillEventArgs
                {
                    Data = _data
                });
            }
            else
            {
                GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_UseSkill_Publish_UI, new SkillEventArgs
                {
                    Data = _data
                });
            }
        }
    }

    private void Update()
    {
        if (_player == null)
        {
            _player ??= UnitFactory.Instance.GetPlayer();
        }

        _button.enabled = !IsNotEnoughUltiMana;

        if (!_isRootSkill) return;
        
        if (_player != null)
        {
            if (_player.SkillDic.TryGetValue(_data.Id, out Skill skill))
            {
                if (!IsNotEnoughUltiMana)
                {
                    Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).enabled = skill.IsCoolingdown;
                }
                else
                {
                    Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).enabled = true;
                }

                UpdateSkillCooldown(skill);
            }
        }
    }

    private void ResetSkillUI()
    {
        Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText(string.Empty);
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 0;
    }

    private void UpdateSkillCooldown(Skill skill)
    {
        if (IsNotEnoughUltiMana)
        {
            Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText("Not Enough MP");
            Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 1;
        }
        else
        {
            float remainCooltime = skill.CooltimeRemain;
            float maxCooltime = skill.CurrentLevelData.Cooltime;

            if (remainCooltime <= 0)
            {
                ResetSkillUI();
                return;
            }

            Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText($"{Mathf.Ceil(remainCooltime)}s");
            Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = remainCooltime / maxCooltime;
        }
    }

    private bool IsNotEnoughUltiMana
    {
        get
        {
            if (_player == null) return false;
            if (_skill == null) return false;

            return _player.Mp.Value < _skill.CurrentLevelData.NeedMp;
        }
    }
}