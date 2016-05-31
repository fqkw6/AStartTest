using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 兵营为空的时候，选择士兵生产的界面
public class UICitySoldierSelectView : UIWindow
{
    public const string Name = "City/UICitySoldierSelectView";
    public Text _textDesc;
    public UIListView _listView;

    private TroopBuildingInfo _currentInfo;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        _currentInfo = param[0] as TroopBuildingInfo;

        if (_currentInfo == null) return;
        _textDesc.text = _currentInfo.Cfg.BuildingDescription;
    }

    public override void OnRefreshWindow()
    {
        UpdateList();
    }

    private void UpdateList()
    {
        List<SoldierConfig> list = new List<SoldierConfig>();
        List<SoldierConfig> notHaveList = new List<SoldierConfig>();
        foreach (var item in SoldierConfigLoader.Data) {
            int level = CityManager.Instance.GetSoldierLevel(item.Value.SoldierId);
            if (level > 0) {
                list.Add(item.Value);
            } else {
                notHaveList.Add(item.Value);
            }
        }

        // 设置数据源
        _listView.MaxCount = SoldierConfigLoader.Data.Count;
        _listView.OnListItemAtIndex = (index) =>
        {
            if (index < list.Count) {
                SoldierConfig cfg = list[index];
                if (cfg.SoldierId == _currentInfo.SoldierConfigID) {
                    // 当前兵营就是此兵种
                    SoldierInfoNotHaveWidget go = _listView.CreateListItemWidget<SoldierInfoNotHaveWidget>(1);
                    go.SetInfo(cfg.SoldierId, true);
                    return go;
                } else {
                    // 只能选择已解锁的兵种
                    SoldierInfoHaveWidget go = _listView.CreateListItemWidget<SoldierInfoHaveWidget>(0);
                    // 士兵信息和兵营信息
                    go.SetInfo(cfg.SoldierId, _currentInfo);
                    return go;
                }
            } else {
                SoldierConfig cfg = notHaveList[index - list.Count];
                SoldierInfoNotHaveWidget go = _listView.CreateListItemWidget<SoldierInfoNotHaveWidget>(1);
                go.SetInfo(cfg.SoldierId, false);
                return go;
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
        UIManager.Instance.OpenWindow<UICityBuildingUplevelView>(_currentInfo);
    }
}
