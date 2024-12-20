using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : BaseView
{
    public SkillData _data;

    private bool _isRootSkill;

    private Unit _player;

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
        GameEventSystem.Instance.Subscribe(UnitEvents.UnitEvent_RootSkill.ToString(), RootSkillEvent);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.UnitEvent_RootSkill.ToString(), RootSkillEvent);
    }

    private void RootSkillEvent(GameEvent e)
    {
        var data = e.args as SkillData;

        if (data == null) return;
        if (data.Id != _data.Id) return;

        Get<Image>((int)Images.Skill_Icon_Image).enabled = true;
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 0;

        var skill = _player.SkillDic[data.Id];

        _isRootSkill = true;
    }

    public override void BindUI()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void OnClick()
    {
        if(_player == null)
        {
            _player = UnitFactory.Instance.GetPlayer();
        }

        if (_player.SkillDic.TryGetValue(_data.Id, out Skill skill))
        {
            if (skill.IsCooldown) return;

            GameEventSystem.Instance.Publish(UnitEvents.UnitEvent_UseSkill_Publish_UI.ToString(),
            new GameEvent
            {
                eventType = UnitEvents.UnitEvent_UseSkill_Publish_UI.ToString(),
                args = new SkillEventArgs
                {
                    data = _data
                }
            });
        }
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = UnitFactory.Instance.GetPlayer();
        }

        if (!_isRootSkill) return;

        if (_player.SkillDic.TryGetValue(_data.Id, out Skill skill))
        {
            Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).enabled = skill.IsCooldown;

            if (!skill.IsCooldown)
            {
                ResetSkillUI();
                return;
            }

            UpdateSkillCooldown(skill);
        }
    }

    private void ResetSkillUI()
    {
        Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText(string.Empty);
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 0;
    }

    private void UpdateSkillCooldown(Skill skill)
    {
        float cooltime = Time.time - skill.TimeMarker;
        float maxCooltime = _data.GetSkillLevelData(skill.Level).coolTime;

        float remainCooltime = maxCooltime - cooltime;

        if (remainCooltime < 0)
        {
            ResetSkillUI();
            return;
        }

        // 텍스트 업데이트
        Get<TextMeshProUGUI>((int)Texts.Skill_Cooltime_Text).SetText($"{Mathf.Ceil(remainCooltime)}s");

        // 필 마운트 업데이트 (0 ~ 1)
        Get<Image>((int)Images.Skill_Cooltime_Image).fillAmount = 1 - (cooltime / maxCooltime);
    }

}
