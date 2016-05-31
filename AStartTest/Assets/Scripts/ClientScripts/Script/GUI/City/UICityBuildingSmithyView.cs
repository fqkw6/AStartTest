using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 铁匠铺信息界面
public class UICityBuildingSmithyView : UIWindow
{
    public const string Name = "City/UICityBuildingSmithyView";
    public Text _title;
    public Text _txtLevel;
    public Text _desc;
    public Text _curLevel;  // 当前可锻造等级
    public Text _nextLevel; // 下一级可锻造
    public Text _levupTime;

    public Text _levupNowCost;
    public Text _levupStoneCost;
    public Text _levupWoodCost;

    public Image _buildingImage;

    private BuildingInfo _currentInfo = null;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        SetInfo(param[0] as BuildingInfo);
    }

    private void SetInfo(BuildingInfo info)
    {
        _currentInfo = info;
        if (_currentInfo == null) return;

        _title.text = _currentInfo.Cfg.BuildingName;// string.Format(Str.Get("UI_CITY_BUILDING_LEVELUP"), _currentInfo.Cfg.BuildingName, _currentInfo.Level + 1);
        _txtLevel.text = Str.Format("UI_LEVEL", info.Level);
        _desc.text = _currentInfo.Cfg.BuildingDescription;

        _levupTime.text = Utils.GetCountDownString(_currentInfo.GetLevelUpCD());

        int quickCost = _currentInfo.GetQuickLevelUpCost(false);
        _levupNowCost.text = quickCost.ToString();
        _levupNowCost.color = UserManager.Instance.Gold < quickCost ? Color.red : Color.white;

        _levupStoneCost.text = _currentInfo.CfgLevel.CostStone.ToString();
        _levupStoneCost.color = UserManager.Instance.Stone < _currentInfo.CfgLevel.CostStone ? Color.red : Color.white;
        _levupWoodCost.text = _currentInfo.CfgLevel.CostWood.ToString();
        _levupWoodCost.color = UserManager.Instance.Wood < _currentInfo.CfgLevel.CostWood ? Color.red : Color.white;

        _curLevel.text = Str.Format("UI_SMITHY_EQUIP_LEVEL", GetEquipLevel(_currentInfo.Level));

        if (_currentInfo.IsMaxLevel()) {
            _nextLevel.transform.parent.gameObject.SetActive(false);
        } else {
            _nextLevel.transform.parent.gameObject.SetActive(true);
            _nextLevel.text = Str.Format("UI_SMITHY_EQUIP_LEVEL", GetEquipLevel(_currentInfo.Level));
        }
    }

    // 获取锻造铺可以锻造的最高装备等级
    private int GetEquipLevel(int smithyLevel)
    {
        int maxLevel = 0;
        foreach (var item in EquipmentConfigLoader.Data) {
            if (item.Value.BuildingLevelDemand == smithyLevel) {
                if (item.Value.EquipLevel > maxLevel) {
                    maxLevel = item.Value.EquipLevel;
                }
            }
        }

        return maxLevel;
    }
    
    public void OnClickLevup()
    {
        if (CityManager.Instance.IsInBuilding()) {
            UIUtil.ShowMsgFormat("MSG_CITY_BULIDING_BUSY");
            return;
        }

        if (CityManager.Instance.CheckBuildingLevelUp(_currentInfo, false, false)) {
            // 检查成功，向服务器发起请求
            CityManager.Instance.RequestUpgradeBuilding(_currentInfo.EntityID);
            CloseWindow();
        }
    }

    // 点击立即升级
    public void OnClickLevupNow()
    {
        if (CityManager.Instance.IsInBuilding()) {
            UIUtil.ShowMsgFormat("MSG_CITY_BULIDING_BUSY");
            return;
        }

        if (CityManager.Instance.CheckBuildingLevelUp(_currentInfo, true, false)) {
            // 检查成功，向服务器发起请求
            CityManager.Instance.RequestQuickUpgradeBuilding(_currentInfo.EntityID, true);
            CloseWindow();
        }
    }

    public void OnClickEnter()
    {
        UIManager.Instance.OpenWindow<UISmithyView>(_currentInfo.Level);
        CloseWindow();
    }
}
