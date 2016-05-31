using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PanelHeroDetail : MonoBehaviour
{
    public Text _txtStr;
    public Text _txtLeadership;
    public Text _txtInt;
    public Text _txtAttack;
    public Text _txtMagicAttack;
    public Text _txtHp;
    public Text _txtCritical;
    public Text _txtHpSorb;
    public Text _txtDef;
    public Text _txtStum;
    public Text _txtAttackSpeed;
    public Text _txtCooldown;
    public Text _txtTalentSkillInfo;
    public Text _txtTalentSkillName;
    public Text[] _txtTag;
    public UIPropWidget _heroProp;

    private HeroInfo _currentInfo = null;

    public void SetHeroInfo(HeroInfo info)
    {
        if (info == null) return;

        bool anim = _currentInfo == null || _currentInfo.ConfigID != info.ConfigID;
        _currentInfo = info;

        List<float> propList = new List<float> { info.Cfg.ViabilityPower, info.Cfg.AuxiliaryPower, info.Cfg.PhysicalDamage, info.Cfg.ComprehensiveAbility, info.Cfg.AbilityDamage };
        _heroProp.SetPropList(propList, anim);

        // 英雄属性
        _txtStr.text = info.Property.Strength.ToString();
        _txtLeadership.text = info.Property.Leadership.ToString();
        _txtInt.text = info.Property.Intelligence.ToString();
        _txtAttack.text = info.Property.Attack.ToString();
        _txtMagicAttack.text = info.Property.MagicAttack.ToString();
        _txtHp.text = info.Property.Hp.ToString();
        _txtCritical.text = info.Property.Critical + "%";
        _txtHpSorb.text = info.Property.HpSorb + "%";
        _txtDef.text = info.Property.Def + "%";
        _txtStum.text = info.Property.Stum + "%";
        _txtCooldown.text = info.Property.Cooldown + "%";
        _txtAttackSpeed.text = info.Property.AttackSpeed + "%";

        for (int i = 0; i < _txtTag.Length; ++i) {
            List<int> label = info.Cfg.HeroLabel;
            if (i < label.Count) {
                _txtTag[i].gameObject.SetActive(true);
                _txtTag[i].text = Str.Get("UI_HERO_LABEL_" + label[i]);
            } else {
                _txtTag[i].gameObject.SetActive(false);
            }
        }

        if (_currentInfo.StarLevel > 1) {
            _txtTalentSkillName.gameObject.SetActive(true);
            _txtTalentSkillInfo.gameObject.SetActive(true);
            _txtTalentSkillName.text = _currentInfo.GetFixedSkillName(_currentInfo.Cfg.PassiveSkill, _currentInfo.StarLevel - 1);
            _txtTalentSkillInfo.text = _currentInfo.GetTalentSkillInfo();
        } else {
            _txtTalentSkillName.gameObject.SetActive(false);
            _txtTalentSkillInfo.gameObject.SetActive(false);
        }
    }
}
