using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 选择升级士兵的头像
public class SoldierTrainHaveWidget : ListItemWidget
{
    public Image _soldierIcon;
    public Text _cost;
    public Text _time;
    public Text _soldierLevel;

    private int _currentSoldierCfgID;
    private SoldierLevelConfig _levelupCfg = null;

    public void SetInfo(int soldierCfgID)
    {
        _currentSoldierCfgID = soldierCfgID;
        int level = CityManager.Instance.GetSoldierLevel(soldierCfgID);
        _levelupCfg = SoldierLevelConfigLoader.GetConfig(soldierCfgID, level);

        _soldierIcon.sprite = ResourceManager.Instance.GetSoldierIcon(_currentSoldierCfgID);

        _cost.text = _levelupCfg.UpgradeCost.ToString();

        // 银两不足，显示红色
        if (UserManager.Instance.Money >= _levelupCfg.UpgradeCost) {
            _cost.color = Color.white;
        } else {
            _cost.color = Color.red;
        }

        _time.text = Utils.GetCountDownString(Utils.GetSeconds(_levelupCfg.UpgradeTime));
        _soldierLevel.text = "Lv" + level;
    }

    public override void OnClick()
    {
        int level = CityManager.Instance.GetSoldierLevel(_currentSoldierCfgID);

        TrainBuildingInfo tbinfo = CityManager.Instance.GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
        if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
            UIUtil.ShowMsgFormat("UI_CITY_BUILDING_TRAIN_NOW");
            return;
        }

        // 最大等级
        if (SoldierLevelConfigLoader.GetConfig(_currentSoldierCfgID, level + 1, false) == null) {
            UIUtil.ShowMsgFormat("UI_CITY_BUILDING_TRAIN_MAX");
            return;
        }

        UIManager.Instance.OpenWindow<UICitySoldierUplevelView>(_currentSoldierCfgID);
    }

    public void OnClickInfo()
    {
        // 显示信息
        int level = CityManager.Instance.GetSoldierLevel(_currentSoldierCfgID);
        UIManager.Instance.OpenWindow<UICitySoldierInfoView>(_currentSoldierCfgID, level);
    }
}
