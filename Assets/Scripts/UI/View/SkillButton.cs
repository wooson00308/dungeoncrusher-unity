using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : BaseView
{
    public SkillData_old _data;

    private bool _isRootSkill;

    private Unit _player;
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
        var data = e as SkillData_old;

        if (data == null) return;
        if (data.Id != _data.Id) return;

        Get<Image>((int)Images.Skill_Icon_Image).enabled = true;
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 0;

        if (_player == null)
        {
            _player = UnitFactory.Instance.GetPlayer();
        }

        var skill = _player.SkillDic[data.Id];

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
        if (IsNotEnoughUltiMana) return;

        if (_player.SkillDic.TryGetValue(_data.Id, out Skill_old skill))
        {
            if (skill.IsCooldown) return;

            GameEventSystem.Instance.Publish((int)UnitEvents.UnitEvent_UseSkill_Publish_UI, new SkillEventArgs
            {
                data = _data
            });
        }
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = UnitFactory.Instance.GetPlayer();
        }

        _button.enabled = !IsNotEnoughUltiMana;

        if (!_isRootSkill) return;

        if (_player.SkillDic.TryGetValue(_data.Id, out Skill_old skill))
        {
            if(!IsNotEnoughUltiMana)
            {
                Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).enabled = skill.IsCooldown;
            }
            else
            {
                Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).enabled = true;
            }

            UpdateSkillCooldown(skill);
        }
    }

    private void ResetSkillUI()
    {
        Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText(string.Empty);
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 0;
    }

    private void UpdateSkillCooldown(Skill_old skill)
    {
        if(IsNotEnoughUltiMana)
        {
            Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText("Not Enough MP");
            Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 1;
        }
        else
        {
            float cooltime = Time.time - skill.TimeMarker;
            float maxCooltime = _data.GetSkillLevelData(skill.Level).coolTime;

            float remainCooltime = maxCooltime - cooltime;

            if (remainCooltime < 0)
            {
                ResetSkillUI();
                return;
            }

            // �ؽ�Ʈ ������Ʈ
            Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText($"{Mathf.Ceil(remainCooltime)}s");
            // �� ����Ʈ ������Ʈ (0 ~ 1)
            Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 1 - (cooltime / maxCooltime);
        }
    }

    private bool IsNotEnoughUltiMana => _data.IsUltSkill && _player.Mp.Value < _player.Mp.Max;

}
