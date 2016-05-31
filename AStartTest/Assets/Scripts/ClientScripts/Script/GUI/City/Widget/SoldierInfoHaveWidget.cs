using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 选择士兵生产的头像
public class SoldierInfoHaveWidget : ListItemWidget {
    public Image _soldierIcon;
    public Text _cost;
    public Text _time;
    public Text _count;
    public Text _soldierLevel;

    private int _maxCount = 0;
    private TroopBuildingInfo _currentBuildingInfo;
    private int _currentSoldierCfgID;
    private SoldierLevelConfig _levelupCfg = null;
    private int _costValue = 0;
    
    public void SetInfo(int soldierCfgID, BuildingInfo buildingInfo)
    {
        _currentSoldierCfgID = soldierCfgID;
        int level = CityManager.Instance.GetSoldierLevel(soldierCfgID);
        _levelupCfg = SoldierLevelConfigLoader.GetConfig(soldierCfgID, level);
        if (_levelupCfg == null) {
            return;
        }
 
        _soldierIcon.sprite = ResourceManager.Instance.GetSoldierIcon(_currentSoldierCfgID);

        // 兵营界面
        _currentBuildingInfo = buildingInfo as TroopBuildingInfo;
        if (_currentBuildingInfo == null) return;

        int maxCount = _currentBuildingInfo.CfgLevel.MaxStorage;
        SoldierConfig cfg = SoldierConfigLoader.GetConfig(_currentSoldierCfgID);
        if (cfg == null) return;

        _maxCount = maxCount / cfg.States;
        _count.text = "x" + (maxCount/cfg.States);
        
        int count = _currentBuildingInfo.GetMaxSoldierCount(_currentSoldierCfgID);
        _costValue = _currentBuildingInfo.GetSwitchCost(_currentSoldierCfgID);
        int timeValue = Utils.GetSeconds(cfg.Producetime * count);
        _cost.text = Mathf.Max(0, _costValue).ToString();

        // 银两不足，显示红色
        if (UserManager.Instance.Money >= _costValue) {
            _cost.color = Color.white;
        } else {
            _cost.color = Color.red;
        }

        _time.text = Utils.GetCountDownString(timeValue);

        // 士兵生产数目
        _count.gameObject.SetActive(true);

        _soldierLevel.text = "Lv" + level;
    }

    public override void OnClick()
    {
        // 银两不足
        if (UserManager.Instance.Money < _costValue) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
            return;
        }

        if (_currentBuildingInfo.SoldierCount <= 0) {
            // 请求生产兵种
            CityManager.Instance.RequestProduceSoldier(_currentBuildingInfo.EntityID, _currentSoldierCfgID, _costValue);
        } else {
            // 请求切换兵种
            CityManager.Instance.RequestSwitchSoldier(_currentBuildingInfo.EntityID, _currentSoldierCfgID, _costValue);
        }

        UIManager.Instance.CloseWindow<UICitySoldierSelectView>();
    }

    public void OnClickInfo()
    {
        // 显示信息
        int level = CityManager.Instance.GetSoldierLevel(_currentSoldierCfgID);
        UIManager.Instance.OpenWindow<UICitySoldierInfoView>(_currentSoldierCfgID, level);
    }
}
