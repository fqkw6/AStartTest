using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

// 升星成功界面
public class UIHeroStarRisingSuccView : UIWindow
{
    public const string Name = "Hero/UIHeroStarRisingSuccView";
    //升星成功
    public Image _imageUpgradeSuccess;

    public Image _imgHeroBg;
    public Image _imgHeroIcon;
    public UIStarPanel _starPanel;
    public Text _txtName;

    public Image _imgNewHeroBg;
    public Image _imgNewHeroIcon;
    public UIStarPanel _newStarPanel;
    public Text _txtNewName;

    //武将当前属性
    public Text _texHeroFirstForce;
    public Text _texHeroFirstLeader;
    public Text _texHeroFirstChi;
    
    //武将升星后属性
    public Text _texHeroEndForce;
    public Text _texHeroEndLeader;
    public Text _texHeroEndchi;

    //武将技
    public Text _textHeroSkillTag;
    public Image _imgHeroSkill;
    public Text _textHeroSkillName;
    public Text _texHeroSkillInfo;

    private HeroInfo _oriInfo;
    private HeroInfo _info;

    public override void OnBindData(params object[] param)
    {
        _oriInfo = (HeroInfo) param[0];
        _info = (HeroInfo) param[1];
    }

    public override void OnRefreshWindow()
    {
        // 原来的属性
        _imgHeroIcon.sprite = ResourceManager.Instance.GetHeroIcon(_oriInfo.ConfigID);
        _imgHeroBg.sprite = ResourceManager.Instance.GetIconBgByQuality(_oriInfo.StarLevel);
        _starPanel.SetStar(_oriInfo.StarLevel);
        _texHeroFirstForce.text = _oriInfo.Property.Strength.ToString();
        _texHeroFirstLeader.text = _oriInfo.Property.Leadership.ToString();
        _texHeroFirstChi.text = _oriInfo.Property.Intelligence.ToString();
        _txtName.text = string.Format("{0} Lv.{1}", _oriInfo.Cfg.HeroName, _oriInfo.Level);
        _txtName.color = ResourceManager.Instance.GetColorByQuality(_oriInfo.StarLevel);

        // 新的属性
        _newStarPanel.SetStar(_info.StarLevel);
        _imgNewHeroIcon.sprite = _imgHeroIcon.sprite;
        _imgNewHeroBg.sprite = _imgHeroBg.sprite;
        _texHeroEndForce.text = _info.Property.Strength.ToString();
        _texHeroEndLeader.text = _info.Property.Leadership.ToString();
        _texHeroEndchi.text = _info.Property.Intelligence.ToString();
        _txtNewName.text = string.Format("{0} Lv.{1}", _info.Cfg.HeroName, _info.Level);
        _txtNewName.color = ResourceManager.Instance.GetColorByQuality(_info.StarLevel);

        if (_info.StarLevel == 2) {
            _textHeroSkillTag.text = Str.Get("UI_HERO_TALENTSKILL_OPEN");
        } else {
            _textHeroSkillTag.text = Str.Get("UI_HERO_TALENTSKILL_CLOSE");
        }
        _texHeroSkillInfo.text = _info.GetTalentSkillInfo();
        _textHeroSkillName.text = _info.GetFixedSkillName(_info.Cfg.PassiveSkill, _info.StarLevel - 1);
    }
}
