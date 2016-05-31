using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroListSkill : MonoBehaviour {
    public Text _textName;
    public Image _skillIcon;
    public Text _textLevel;
    public Text _textMoney;
    public Text _textUnlock;
    public Button _btnUplev;
    public Image _flag;
    public Text _txtFullLevel;

    private SkillInfo _currentInfo;
    private HeroInfo _currentHeroInfo;
    private int _currentIndex;

    public void SetInfo(HeroInfo info, int skillID, int index)
    {
        _currentHeroInfo = info;
        _currentIndex = index;

        _skillIcon.sprite = ResourceManager.Instance.GetSkillIcon(skillID);

        SkillSettingsConfig cfg = SkillSettingsConfigLoader.GetConfig(skillID, false);
        if (cfg == null) {
            _flag.gameObject.SetActive(false);
            _textMoney.gameObject.SetActive(false);
            _btnUplev.gameObject.SetActive(false);
            _textLevel.gameObject.SetActive(false);
            _textUnlock.gameObject.SetActive(false);
            _skillIcon.gameObject.SetActive(false);
            _textName.gameObject.SetActive(false);
            _txtFullLevel.gameObject.SetActive(false);
            return;
        }

        _skillIcon.gameObject.SetActive(true);
        _textName.gameObject.SetActive(true);

        _textName.text = cfg.Name;

        bool lockSkill = false;
        if (index == 2 && info.Level < GameConfig.SKILL_UNLOCK2
            || index == 3 && info.Level < GameConfig.SKILL_UNLOCK3
            || index == 4 && info.Level < GameConfig.SKILL_UNLOCK4) {
                lockSkill = true;
        }

        if (!lockSkill) {
            SkillInfo skillInfo = info.GetSkillByID(skillID);
            int level = skillInfo != null ? skillInfo.Level : 1;

            // 已经解锁
            _flag.gameObject.SetActive(true);
            _textMoney.gameObject.SetActive(true);
            _btnUplev.gameObject.SetActive(true);
            _textLevel.gameObject.SetActive(true);
            _textUnlock.gameObject.SetActive(false);

            int upgradeCost = SkillInfo.GetUpgradeCost(index, level);
            if (upgradeCost > 0) {
                // 未满级
                _textMoney.text = upgradeCost.ToString();
                _textMoney.gameObject.SetActive(true);
                _txtFullLevel.gameObject.SetActive(false);
                _flag.gameObject.SetActive(true);
                _btnUplev.gameObject.SetActive(true);
            } else {
                // 已满级
                _textMoney.gameObject.SetActive(false);
                _txtFullLevel.gameObject.SetActive(true);
                _flag.gameObject.SetActive(false);
                _btnUplev.gameObject.SetActive(false);
            }
            _textLevel.text = "Lv." + level;
        } else {
            // 提升到固定品阶解锁
            _flag.gameObject.SetActive(false);
            _textMoney.gameObject.SetActive(false);
            _btnUplev.gameObject.SetActive(false);
            _textLevel.gameObject.SetActive(false);
            _textUnlock.gameObject.SetActive(true);
            _txtFullLevel.gameObject.SetActive(false);

            if (index == 2) {
                _textUnlock.text = Str.Format("UI_HERO_SKILL_UPGRADE_TIP", GameConfig.SKILL_UNLOCK2);
            } else if (index == 3) {
                _textUnlock.text = Str.Format("UI_HERO_SKILL_UPGRADE_TIP", GameConfig.SKILL_UNLOCK3);
            } else if (index == 4) {
                _textUnlock.text = Str.Format("UI_HERO_SKILL_UPGRADE_TIP", GameConfig.SKILL_UNLOCK4);
            }
        }
    }

    // 技能升级
    public void OnClick()
    {
        UserManager.Instance.RequestSkillUpgrade(_currentHeroInfo.EntityID, _currentHeroInfo.GetSkillConfigIDByIndex(_currentIndex), _currentIndex);
    }

    // 当点下去时，显示tip
    public void OnTouchDown()
    {

    }

    // 取消Tip
    public void OnTouchUp()
    {

    }
}
