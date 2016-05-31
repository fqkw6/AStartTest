using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 校场选择士兵提升等级的界面
public class UICityTrainSelectView : UIWindow
{
    public const string Name = "City/UICityTrainSelectView";
    public Button _btnLevelUp;
    public Text _textDesc;
    public UIListView _listView;

    private TrainBuildingInfo _currentInfo;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
    }

    public override void OnRefreshWindow()
    {
        _currentInfo = CityManager.Instance.GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
        if (_currentInfo == null) return;
        _btnLevelUp.gameObject.SetActive(!_currentInfo.IsInBuilding());
        _textDesc.text = _currentInfo.Cfg.BuildingDescription;

        UpdateList();
    }

    private void UpdateList()
    {
        List<SoldierConfig> list = new List<SoldierConfig>();
        foreach (var item in SoldierConfigLoader.Data) {
            list.Add(item.Value);
        }

        TrainBuildingInfo tbinfo = CityManager.Instance.GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
        if (tbinfo == null) return; // 尚未解锁校场

        // 设置数据源
        _listView.MaxCount = SoldierConfigLoader.Data.Count;
        _listView.OnListItemAtIndex = (index) => {
            SoldierConfig cfg = list[index];
            int level = CityManager.Instance.GetSoldierLevel(cfg.SoldierId);
            if (level <= 0) {
                // 未解锁兵种
                SoldierTrainNotHaveWidget go = _listView.CreateListItemWidget<SoldierTrainNotHaveWidget>(1);
                go.SetInfo(cfg.SoldierId);
                return go;
            } else {
                SoldierLevelConfig cfgLevel = SoldierLevelConfigLoader.GetConfig(cfg.SoldierId, level, false);
                if (cfgLevel == null || tbinfo.IsInBuilding() // 校场正在升级
                    || cfgLevel.UpgradeMilitaryLevelDemand > tbinfo.Level // 不能升级，校场等级不足
                    || (tbinfo.IsTrainingSoldier() && tbinfo.TrainSoldierCfgID == cfg.SoldierId)) {
                    // 不能升级或者正在升级
                    SoldierTrainNotHaveWidget go = _listView.CreateListItemWidget<SoldierTrainNotHaveWidget>(1);
                    go.SetInfo(cfg.SoldierId);
                    return go;
                } else {
                    // 已解锁
                    SoldierTrainHaveWidget go = _listView.CreateListItemWidget<SoldierTrainHaveWidget>(0);
                    go.SetInfo(cfg.SoldierId);
                    return go;
                }
            }
        };

        // 进行刷新
        _listView.Refresh();
    }

    public void OnClickLevelUp()
    {
        if (_currentInfo.IsMaxLevel()) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MAX_LEVEL");
            return;
        }
        UIManager.Instance.OpenWindow<UICityPalaceTrainLevelUpView>(_currentInfo);
    }
}