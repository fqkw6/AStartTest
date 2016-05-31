using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 取消xxx的确认界面
public class UIMsgBoxCancelView : UIWindow
{
    public const string Name = "City/UIMsgBoxCancelView";
    public Text _title;
    public Text _detail;

    private BuildingInfo _currentInfo;

    public override void OnBindData(params object[] param)
    {
        _currentInfo = param[0] as BuildingInfo;
        if (_currentInfo == null) return;

        if (_currentInfo.IsInBuilding()) {
            _title.text = Str.Get("UI_MSG_CANCEL");
            _detail.text = string.Format(Str.Get("UI_MSG_CANCEL_DETAIL"), _currentInfo.Cfg.BuildingName);
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            // 快速升级兵种
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                _title.text = Str.Get("UI_MSG_CANCEL");
                SoldierConfig cfg = SoldierConfigLoader.GetConfig(tbinfo.TrainSoldierCfgID);
                _detail.text = string.Format(Str.Get("UI_MSG_CANCEL_DETAIL"), cfg.SoldierName);
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 快速生产士兵
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                _title.text = Str.Get("UI_MSG_CANCEL_SOLDIER");
                _detail.text = string.Format(Str.Get("UI_MSG_CANCEL_SOLDIER_DETAIL"), tbinfo.SoldierCfg.SoldierName);
            }
        }
    }

    public void OnClickOK()
    {
        if (_currentInfo.IsInBuilding()) {
            // 立刻升级建筑
            CityManager.Instance.RequestCancelUpgradeBuilding(_currentInfo.EntityID);
            CloseWindow();
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            // 快速升级兵种
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                CityManager.Instance.RequestCancelTrainSoldier(tbinfo.TrainSoldierCfgID);
                CloseWindow();
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 快速生产士兵
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                CityManager.Instance.RequestCancelProduceSoldier(_currentInfo.EntityID, tbinfo.SoldierConfigID);
                CloseWindow();
            }
        } else {
            CloseWindow();
        }
    }
}
