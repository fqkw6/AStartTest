using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 选择升级士兵的头像
public class SoldierTrainNotHaveWidget : ListItemWidget
{
    public Image _soldierIcon;
    public Text _uplevelLimit;

    private string _textLimit;
    private int _currentSoldierCfgID = 0;

    public void SetInfo(int soldierCfgID)
    {
        _currentSoldierCfgID = soldierCfgID;
        int level = CityManager.Instance.GetSoldierLevel(soldierCfgID);
        _soldierIcon.sprite = ResourceManager.Instance.GetSoldierIcon(soldierCfgID);

        _uplevelLimit.text = "";

        if (level == 0) {
            // 尚未获得该兵种
            SoldierConfig cfg = SoldierConfigLoader.GetConfig(soldierCfgID);
            if (cfg != null) {
                // 提示解锁 MSG_CITY_TRAIN_UNLOCK={0}级校场解锁
                _textLimit = string.Format(Str.Get("MSG_CITY_TRAIN_UNLOCK"), cfg.UnlockMilitaryDemand);
                _uplevelLimit.text = _textLimit;
            }
        } else {
            TrainBuildingInfo tpinfo = CityManager.Instance.GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
            SoldierLevelConfig cfg = SoldierLevelConfigLoader.GetConfig(soldierCfgID, level);
            if (cfg != null && tpinfo != null) {
                if (tpinfo.IsTrainingSoldier() && tpinfo.TrainSoldierCfgID == soldierCfgID) {
                    // 提示正在升级
                    _textLimit = Str.Get("UI_CITY_BUILDING_TRAIN_NOW");
                    _uplevelLimit.text = _textLimit;
                } else {
                    // 提示升级 MSG_CITY_TRAIN_LIMIT=需要{0}级校场
                    _textLimit = string.Format(Str.Get("MSG_CITY_TRAIN_LIMIT"), cfg.UpgradeMilitaryLevelDemand);
                    _uplevelLimit.text = _textLimit;
                }
            }
        }
    }

    public override void OnClick()
    {
        // 校场正在升级
        TrainBuildingInfo tbinfo = CityManager.Instance.GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
        if (tbinfo != null && tbinfo.IsInBuilding()) {
            UIUtil.ShowMsgFormat("MSG_CITY_TRAIN_IN_BUILDING");
            return;
        }

        // 点击弹出错误提示
        if (!string.IsNullOrEmpty(_textLimit)) {
            UIUtil.ShowMsg(_textLimit);
        }
    }

    public void OnClickInfo()
    {
        int level = CityManager.Instance.GetSoldierLevel(_currentSoldierCfgID);
        UIManager.Instance.OpenWindow<UICitySoldierInfoView>(_currentSoldierCfgID, Mathf.Max(level, 1));
    }
}
