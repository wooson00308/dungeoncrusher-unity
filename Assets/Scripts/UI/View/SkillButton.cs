using UnityEngine;
using UnityEngine.UI;

public class SkillButton : BaseView
{
    public SkillData _data;

    private bool _isRootSkill;
    private bool _isCooldown;

    private Unit _player;

    public enum Images
    {
        Skill_Icon_Image,
        Skill_Cooltime_Image
    }

    public enum Texts
    {

    }

    private void Awake()
    {
        BindUI();
    }

    private void OnEnable()
    {
        _isRootSkill = false;
        Get<Image>((int)Images.Skill_Icon_Image).enabled = false;
        Get<Image>((int)Images.Skill_Icon_Image).fillAmount = 1;
        GameEventSystem.Instance.Subscribe(UnitEvents.RootSkill.ToString(), RootSkillEvent);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(UnitEvents.RootSkill.ToString(), RootSkillEvent);
    }

    private void RootSkillEvent(GameEvent e)
    {
        var data = e.args as SkillData;

        if (data == null) return;
        if (data.Id != _data.Id) return;

        Get<Image>((int)Images.Skill_Icon_Image).enabled = true;

        var skill = _player.SkillDic[data.Id];
        _isCooldown = skill.IsCooldown;
    }

    public override void BindUI()
    {
        Bind<Image>(typeof(Images));
    }

    public void OnClick()
    {
        if (_isCooldown) return;

        GameEventSystem.Instance.Publish(UnitEvents.UseSkill_Publish_UI.ToString(), 
            new GameEvent {
                eventType = UnitEvents.UseSkill_Publish_UI.ToString(),
                args = new SkillEventArgs 
                { 
                    data = _data
                } 
            });

        if(_player == null)
        {
            _player = UnitFactory.Instance.GetPlayer();
        }

        if (_player.SkillDic.TryGetValue(_data.Id, out Skill skill))
        {
            _isCooldown = skill.IsCooldown;
        }
    }

    private void Update()
    {
        if(!_isRootSkill) return;
        if(!_isCooldown) return;

        if(_player.SkillDic.TryGetValue(_data.Id, out Skill skill))
        {
            _isCooldown = skill.IsCooldown;

            float cooltime = Time.time - skill.TimeMarker;
            float maxCooltime = _data.GetSkillLevelData(skill.Level).coolTime;

            Get<Image>((int)Images.Skill_Icon_Image).fillAmount = cooltime / maxCooltime;
        }
    }
}
