using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 提升主城和校场等级的界面
public class UICityPalaceTrainLevelUpView : UIWindow
{
    public const string Name = "City/UICityPalaceTrainLevelUpView";
    public Text _title;
    public Text _txtLevel;
    public Text _desc;
    public Text _maxContainValue;
    public Text _maxContainValueWood;
    public Text _maxContainValueStone;
    public Text _levupTime;
    
    public Text _levupNowCost;
    public Text _levupStoneCost;
    public Text _levupWoodCost;

    public Text _unlockText;
    public Text _textLevelUpTime;
    public Image[] _unlockBuildingBg;
    public Image[] _unlockBuildingIcon;
    public Text[] _unlockBuildingName;

    public Image _buildingImage;
    public Image _imgTrain;

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

        //_title.text = string.Format(Str.Get("UI_CITY_BUILDING_LEVELUP"), _currentInfo.Cfg.BuildingName, _currentInfo.Level + 1);
        if (_currentInfo.BuildingType == CityBuildingType.PALACE) {
            _buildingImage.sprite = ResourceManager.Instance.GetBuildingIcon(info.ConfigID);
            _imgTrain.gameObject.SetActive(false);
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            _imgTrain.sprite = ResourceManager.Instance.GetBuildingIcon(info.ConfigID);
            _buildingImage.gameObject.SetActive(false);
        }

        _levupTime.text = Utils.GetCountDownString(_currentInfo.GetLevelUpCD());

        _title.text = info.Cfg.BuildingName;
        _txtLevel.text = Str.Format("UI_LEVEL", info.Level);
        _desc.text = info.Cfg.BuildingDescription;
        _maxContainValue.text = _currentInfo.GetMaxContainValue().ToString();
        _maxContainValueWood.text = _currentInfo.GetMaxContainValue().ToString();
        _maxContainValueStone.text = _currentInfo.GetMaxContainValue().ToString();

        if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            _maxContainValue.transform.parent.gameObject.SetActive(false);
            _maxContainValueWood.transform.parent.gameObject.SetActive(false);
            _maxContainValueStone.transform.parent.gameObject.SetActive(false);
        }

        int quickCost = _currentInfo.GetQuickLevelUpCost(false);
        _levupNowCost.text = quickCost.ToString();
        _levupNowCost.color = UserManager.Instance.Gold < quickCost ? Color.red : Color.white;

        _levupStoneCost.text = _currentInfo.CfgLevel.CostStone.ToString();
        _levupStoneCost.color = UserManager.Instance.Stone < _currentInfo.CfgLevel.CostStone ? Color.red : Color.white;
        _levupWoodCost.text = _currentInfo.CfgLevel.CostWood.ToString();
        _levupWoodCost.color = UserManager.Instance.Wood < _currentInfo.CfgLevel.CostWood ? Color.red : Color.white;

        foreach (var item in _unlockBuildingBg) {
            item.gameObject.SetActive(false);
        }
        _unlockText.gameObject.SetActive(false);

        int index = 0;
        if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            // 校场
            _unlockText.text = Str.Get("UI_CITY_BUILDING_UNLOCK_SOLDIER");
            foreach (var item in SoldierConfigLoader.Data) {
                if (_currentInfo.Level + 1 == item.Value.UnlockMilitaryDemand) {
                    // 如果升级后的校场等级是士兵的解锁等级
                    _unlockBuildingBg[index].gameObject.SetActive(true);
                    _unlockBuildingIcon[index].sprite = ResourceManager.Instance.GetSoldierIcon(item.Key);
                    _unlockBuildingName[index].text = item.Value.SoldierName;
                    ++index;
                }
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.PALACE) {
            // 主城
            _unlockText.text = Str.Get("UI_CITY_BUILDING_UNLOCK_BUILDING");

            foreach (var item in BuildingConstConfigLoader.Data) {
                if (_currentInfo.Level + 1 == item.Value.UnlockHomeLevelDemand) {
                    // 如果升级后的校场等级是士兵的解锁等级
                    _unlockBuildingBg[index].gameObject.SetActive(true);
                    _unlockBuildingIcon[index].sprite = ResourceManager.Instance.GetBuildingIcon(item.Key);
                    _unlockBuildingName[index].text = item.Value.BuildingName;
                    ++index;
                }
            }
        }

        if (index > 0) {
            _unlockText.gameObject.SetActive(true);
        } else {
            //_textLevelUpTime.transform.localPosition = _unlockText.transform.localPosition;
        }
    }

    private void CloseOtherWindow()
    {
        UIManager.Instance.CloseWindow<UICityTrainSelectView>();
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
            CloseOtherWindow();
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
            CloseOtherWindow();
            CloseWindow();
        }
    }
}
