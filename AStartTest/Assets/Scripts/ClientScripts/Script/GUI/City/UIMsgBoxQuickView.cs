using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 快速完成xxx的确认界面
public class UIMsgBoxQuickView : UIWindow
{
    public const string Name = "City/UIMsgBoxQuickView";
    public Text _title;
    public Text _detail;
    public Text _cost;
    public Image _imgFlag;

    private BuildingInfo _currentInfo;

    public override void OnBindData(params object[] param)
    {
        _currentInfo = param[0] as BuildingInfo;
        if (_currentInfo == null) return;

        int costValue = 0;
        if (_currentInfo.IsInBuilding()) {
            costValue = _currentInfo.GetQuickLevelUpCost(true);
            _title.text = Str.Get("UI_MSG_QUICK_UPGRADE_TITLE");
            _detail.text = string.Format(Str.Get("UI_MSG_QUICK_UPGRADE_DETAIL"), costValue, _currentInfo.Cfg.BuildingName);
            _cost.text = _currentInfo.GetQuickLevelUpCost(true).ToString();
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            // 快速升级兵种
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                costValue = tbinfo.GetQuickTrainCost();
                _title.text = Str.Get("UI_MSG_QUICK_UPGRADE_TITLE");
                SoldierConfig cfg = SoldierConfigLoader.GetConfig(tbinfo.TrainSoldierCfgID);
                _detail.text = string.Format(Str.Get("UI_MSG_QUICK_UPGRADE_DETAIL"), costValue, cfg.SoldierName);
                _cost.text = tbinfo.GetQuickTrainCost().ToString();
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 快速生产士兵
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                costValue = tbinfo.GetQuickProducingCost();
                _title.text = Str.Get("UI_MSG_QUICK_PRODUCE_SOLDIER");
                _detail.text = string.Format(Str.Get("UI_MSG_QUICK_PRODUCE_SOLDIER_DETAIL"), costValue, tbinfo.SoldierCfg.SoldierName);
                _cost.text = tbinfo.GetQuickProducingCost().ToString();
            }
        }

        RectTransform rc = _imgFlag.transform as RectTransform;
        if (rc) {
            rc.anchoredPosition = new Vector2(-(_imgFlag.preferredWidth + _cost.preferredWidth) / 2 - 2, rc.anchoredPosition.y);
        }

        if (costValue <= 0) {
            CloseWindow();
        }
    }

    public void OnClickOK()
    {
        if (_currentInfo.IsInBuilding()) {
            // 立刻升级建筑
            CityManager.Instance.RequestQuickUpgradeBuilding(_currentInfo.EntityID, false);
            CloseWindow();
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            // 快速升级兵种
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                CityManager.Instance.RequestQuickTrainSoldier(tbinfo.TrainSoldierCfgID, false);
                CloseWindow();
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 快速生产士兵
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                CityManager.Instance.RequestQuickProduceSoldier(_currentInfo.EntityID, tbinfo.SoldierConfigID);
                CloseWindow();
            }
        } else {
            CloseWindow();
        }
        
    }
}
