using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 提升建筑等级的界面
public class UICityBuildingUplevelView : UIWindow
{
    public const string Name = "City/UICityBuildingUplevelView";
    public Text _title;
    public Text _txtLevel;
    public Text _desc;
    public Text _maxContainText;
    public Text _maxContainValue;
    public Text _maxContainAddValue;
    public Image _maxContainImage;
    public Text _txtProductText;
    public Text _productValue;
    public Text _productAddValue;
    public Image _produceImage;
    public Text _levelUpText;
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
        _maxContainValue.text = _currentInfo.GetMaxContainValue().ToString();
        _txtLevel.text = Str.Format("UI_LEVEL", info.Level);
        _desc.text = _currentInfo.Cfg.BuildingDescription;
        _buildingImage.sprite = ResourceManager.Instance.GetBuildingIcon(info.ConfigID);

        if (info.BuildingType == CityBuildingType.TROOP) {
            // 如果是兵营的话 显示最大人口
            _maxContainText.text = Str.Get("UI_CITY_BUILDING_MAX_SPACE");
        } else {
            // 显示最大容量
            _maxContainText.text = Str.Get("UI_CITY_BUILDING_MAX_CONTAIN");
        }

        int maxContainAddValue = _currentInfo.GetNextMaxContainValue();
        if (maxContainAddValue > 0) {
            _maxContainAddValue.gameObject.SetActive(true);
            _maxContainAddValue.text = string.Format("(+{0})", maxContainAddValue);
        } else {
            _maxContainAddValue.gameObject.SetActive(false);
        }

        _levupTime.text = Utils.GetCountDownString(_currentInfo.GetLevelUpCD());

        int quickCost = _currentInfo.GetQuickLevelUpCost(false);
        _levupNowCost.text = quickCost.ToString();
        _levupNowCost.color = UserManager.Instance.Gold < quickCost ? Color.red : Color.white;

        _levupStoneCost.text = _currentInfo.CfgLevel.CostStone.ToString();
        _levupStoneCost.color = UserManager.Instance.Stone < _currentInfo.CfgLevel.CostStone ? Color.red : Color.white;
        _levupWoodCost.text = _currentInfo.CfgLevel.CostWood.ToString();
        _levupWoodCost.color = UserManager.Instance.Wood < _currentInfo.CfgLevel.CostWood ? Color.red : Color.white;

        switch (_currentInfo.BuildingType) {
            case CityBuildingType.HOUSE:
                _maxContainImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.MONEY);
                _produceImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.MONEY);
                break;
            case CityBuildingType.STONE:
                _maxContainImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.STONE);
                _produceImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.STONE);
                break;
            case CityBuildingType.WOOD:
                _maxContainImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.WOOD);
                _produceImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.WOOD);
                break;
            case CityBuildingType.MONEY_STORAGE:
                _maxContainImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.MONEY);
                break;
            case CityBuildingType.STONE_STORAGE:
                _maxContainImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.STONE);
                break;
            case CityBuildingType.WOOD_STORAGE:
                _maxContainImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.WOOD);
                break;
            case CityBuildingType.TROOP:
                _maxContainImage.sprite = ResourceManager.Instance.GetResIcon(ResourceType.SOLDIER);
                break;
        }
        _maxContainImage.SetNativeSize();
        _produceImage.SetNativeSize();

        ProduceBuildingInfo pbinfo = _currentInfo as ProduceBuildingInfo;
        if (pbinfo != null) {
            // 资源生产建筑
            _productValue.text = pbinfo.GetUnitProduceValue().ToString();

            int produceAddValue = pbinfo.GetNextProduceValue();
            if (produceAddValue > 0) {
                _productAddValue.gameObject.SetActive(true);
                _productAddValue.text = string.Format("(+{0})", produceAddValue);
            } else {
                _productAddValue.gameObject.SetActive(false);
            }
        } else {
            _txtProductText.gameObject.SetActive(false);
            _levelUpText.transform.position = _txtProductText.transform.position;
        }
    }


    private void CloseOtherWindow()
    {
        UIManager.Instance.CloseWindow<UICityTrainSelectView>();
        UIManager.Instance.CloseWindow<UICitySoldierSelectView>();
        UIManager.Instance.CloseWindow<UICitySoldierSwitchView>();
    }

    // 点击升级
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
