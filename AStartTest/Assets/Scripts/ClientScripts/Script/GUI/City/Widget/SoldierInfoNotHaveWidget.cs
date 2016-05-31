using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 选择士兵生产的头像
public class SoldierInfoNotHaveWidget : ListItemWidget
{
    public Image _soldierIcon;
    public Text _uplevelLimit;

    private string _textLimit;
    private int _currentSoldierCfgID = 0;

    public void SetInfo(int soldierCfgID, bool isSameSoldier)
    {
        _currentSoldierCfgID = soldierCfgID;
        _soldierIcon.sprite = ResourceManager.Instance.GetSoldierIcon(soldierCfgID);

        if (isSameSoldier) {
            // 当前兵种
            _textLimit = Str.Get("UI_CITY_TROOP_SAME_SOLDIER");
            _uplevelLimit.text = _textLimit;
        } else {
            // 尚未获得该兵种
            SoldierConfig cfg = SoldierConfigLoader.GetConfig(soldierCfgID);
            if (cfg != null) {
                // 提示解锁 MSG_CITY_TRAIN_UNLOCK={0}级校场解锁
                _textLimit = string.Format(Str.Get("MSG_CITY_TRAIN_UNLOCK"), cfg.UnlockMilitaryDemand);
                _uplevelLimit.text = _textLimit;
            } 
        }
    }

    public override void OnClick()
    {
        // 点击弹出错误提示
        if (!string.IsNullOrEmpty(_textLimit)) {
            UIUtil.ShowMsg(_textLimit);
        }
    }

    public void OnClickInfo()
    {
        int level = CityManager.Instance.GetSoldierLevel(_currentSoldierCfgID);
        UIManager.Instance.OpenWindow<UICitySoldierInfoView>(_currentSoldierCfgID, 1);
    }
}
