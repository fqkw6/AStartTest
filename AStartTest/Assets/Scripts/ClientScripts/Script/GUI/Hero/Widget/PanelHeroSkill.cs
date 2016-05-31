using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelHeroSkill : MonoBehaviour
{
    public Text _txtSkillNumber;
    public HeroListSkill _skill1;
    public HeroListSkill _skill2;
    public HeroListSkill _skill3;
    public HeroListSkill _skill4;
    public Button _btnBuy;

    private HeroInfo _currentInfo;

    // Use this for initialization
    void Start ()
    {
        EventDispatcher.AddEventListener(EventID.EVENT_UI_REFRESH_SKILL_POINT, UpdateSkillPoint);
        UserManager.Instance.RequestSkillPointInfo();

        InvokeRepeating("UpdateSkillPoint", 0, 1);
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_REFRESH_SKILL_POINT, UpdateSkillPoint);
    }

    public void UpdateSkillPoint()
    {
        int countdown = UserManager.Instance.GetSkillPointCountDownTime();

        int skillPoint = UserManager.Instance.GetCurrentSkillPoint();
        if (skillPoint >= GameConfig.MAX_SKILL_POINT) {
            // 技能点满的时候不显示倒计时
            _txtSkillNumber.text = Str.Get("UI_HERO_SKILL_POINT") + skillPoint.ToString();
        } else {
            // 显示倒计时
            _txtSkillNumber.text = Str.Get("UI_HERO_SKILL_POINT") + skillPoint.ToString() + string.Format("  ({0})", Utils.GetCountDownTime(countdown));
        }

        // 如果没有技能点，那么显示购买按钮
        _btnBuy.gameObject.SetActive(skillPoint <= 0);
    }

    public void SetHeroInfo(HeroInfo info)
    {
        if (info == null) return;

        _currentInfo = info;

        if (_currentInfo.Cfg == null) return;

        // 设置技能数据
        _skill1.SetInfo(_currentInfo, _currentInfo.Cfg.Skill1, 1);
        _skill2.SetInfo(_currentInfo, _currentInfo.Cfg.Skill2, 2);
        _skill3.SetInfo(_currentInfo, _currentInfo.Cfg.Skill3, 3);
        _skill4.SetInfo(_currentInfo, _currentInfo.Cfg.Skill4, 4);

        // 更新技能点
        UpdateSkillPoint();
    }

    public void OnClickAddSkillPoint()
    {
        
    }
}
