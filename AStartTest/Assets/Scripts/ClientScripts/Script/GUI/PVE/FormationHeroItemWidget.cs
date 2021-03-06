﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// 选择英雄widget
public class FormationHeroItemWidget : ListItemWidget
{
    public UIStarPanel _starPanel;
    public Image _imageHeroIconBg;
    public Image _imageHeroIcon;
    public Image _imageHeroAttribute;
    public Text _heroName;
    public Text _txtHeroLevel;
    public Image _imgExpPrg;
    public Text _txtExpPrg;
    public Text _txtFightScore;
    public Image _imgCheck;

    [NonSerialized] public bool IsSelect = false;

    private HeroInfo _currentInfo;

    public override void SetInfo(object data)
    {
        SetHeroInfo(data as HeroInfo);
    }

    public void SetHeroInfo(HeroInfo info)
    {
        _currentInfo = info;
        _starPanel.SetStar(info.StarLevel);
        _imageHeroIconBg.sprite = ResourceManager.Instance.GetHeroIconBgByStar(info.StarLevel);
        _imageHeroIcon.sprite = ResourceManager.Instance.GetHeroIcon(info.ConfigID);
        _heroName.text = info.GetName();
        _txtHeroLevel.text = "Lv" + info.Level;

        IsSelect = UserManager.Instance.IsHeroInPVEFormation(info.ConfigID);
        _imgCheck.gameObject.SetActive(IsSelect);

        _imageHeroAttribute.sprite = ResourceManager.Instance.GetHeroTypeIcon(info.ConfigID);

        _heroName.color = ResourceManager.Instance.GetColorByQuality(info.StarLevel);

        HeroLevelConfig expCfg = HeroLevelConfigLoader.GetConfig(_currentInfo.Level);
        if (expCfg != null) {
            if (expCfg.ExpRequire == 0) {
                _imgExpPrg.fillAmount = 1;
                _txtExpPrg.gameObject.SetActive(false);
            } else {
                _imgExpPrg.fillAmount = 1.0f * _currentInfo.Exp / expCfg.ExpRequire;
                _txtExpPrg.text = string.Format("{0}/{1}", _currentInfo.Exp, expCfg.ExpRequire);
                _txtExpPrg.gameObject.SetActive(true);
            }
        }

        _txtFightScore.text = _currentInfo.FightingScore.ToString();
    }

    public override void OnSelect()
    {
        IsSelect = true;
        _imgCheck.gameObject.SetActive(true);

        UserManager.Instance.PVEHeroList.Add(_currentInfo);
    }

    public override void OnUnselect()
    {
        UserManager.Instance.PVEHeroList.Remove(_currentInfo);
        IsSelect = false;
        _imgCheck.gameObject.SetActive(false);
    }
}
