using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 士兵升级界面
public class UICitySoldierUplevelView : UIWindow
{
    public const string Name = "City/UICitySoldierUplevelView";
    public Text _title;
    public Text _hpValue;
    public Text _hpAddValue;
    public Text _attackValue;
    public Text _attackAddValue;
    public Text _trainCost;
    public Text _trainCostAdd;
    public Text _uplevTime;

    public Text _uplevNowCost;
    public Text _uplevCost;
    public Image _imgSoldier;

    private int _currentSoldierCfgID;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        SetInfo((int)param[0]);
    }

    private void SetInfo(int soldierCfgID)
    {
        _currentSoldierCfgID = soldierCfgID;
        int level = CityManager.Instance.GetSoldierLevel(soldierCfgID);
        if (level <= 0) return;

        SoldierConfig cfg = SoldierConfigLoader.GetConfig(_currentSoldierCfgID);
        _title.text = string.Format(Str.Get("UI_CITY_BUILDING_LEVELUP"), cfg.SoldierName, level + 1);

        SoldierLevelConfig cfgLevel = SoldierLevelConfigLoader.GetConfig(_currentSoldierCfgID, level);
        SoldierLevelConfig cfgNextLevel = SoldierLevelConfigLoader.GetConfig(_currentSoldierCfgID, level + 1);
        _hpValue.text = cfgLevel.SoldierHp.ToString();
        _trainCost.text = cfgLevel.ProduceCost.ToString();
        _attackValue.text = cfgLevel.SoldierAttack.ToString();
        _imgSoldier.sprite = ResourceManager.Instance.GetSoldierImage(soldierCfgID);

        if (cfgNextLevel != null) {
            // 增加变化的数值
            _hpAddValue.gameObject.SetActive(true);
            _attackAddValue.gameObject.SetActive(true);
            _trainCostAdd.gameObject.SetActive(true);
            _hpAddValue.text = string.Format("(+{0})", cfgNextLevel.SoldierHp - cfgLevel.SoldierHp);
            _attackAddValue.text = string.Format("(+{0})", cfgNextLevel.SoldierAttack - cfgLevel.SoldierAttack);
            _trainCostAdd.text = string.Format("(+{0})", cfgNextLevel.ProduceCost - cfgLevel.ProduceCost);
        } else {
            _hpAddValue.gameObject.SetActive(false);
            _attackAddValue.gameObject.SetActive(false);
            _trainCostAdd.gameObject.SetActive(false);
        }

        _uplevTime.text = Utils.GetCountDownString(Utils.GetSeconds(cfgLevel.UpgradeTime));
        _uplevNowCost.text = Formula.GetLevelUpQuickCost(Utils.GetSeconds(cfgLevel.UpgradeTime)).ToString();
        _uplevCost.text = cfgLevel.UpgradeCost.ToString();
    }

    // 立即升级
    public void OnClickLevupNow()
    {
        int level = CityManager.Instance.GetSoldierLevel(_currentSoldierCfgID);
        if (level <= 0) return;

        SoldierConfig cfg = SoldierConfigLoader.GetConfig(_currentSoldierCfgID);
        SoldierLevelConfig cfgLevel = SoldierLevelConfigLoader.GetConfig(_currentSoldierCfgID, level);

        // 检查金钱
        if (UserManager.Instance.Money < cfgLevel.UpgradeCost) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
            return;
        }

        // 检查元宝
        if (UserManager.Instance.Gold < Formula.GetLevelUpQuickCost(Utils.GetSeconds(cfgLevel.UpgradeTime))) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_GOLD_LIMIT");
            return;
        }

        CityManager.Instance.RequestQuickTrainSoldier(_currentSoldierCfgID, true);
        UIManager.Instance.CloseWindow<UICityTrainSelectView>();
        CloseWindow();
    }

    // 升级
    public void OnClickLevup()
    {

        int level = CityManager.Instance.GetSoldierLevel(_currentSoldierCfgID);
        if (level <= 0) return;

        SoldierConfig cfg = SoldierConfigLoader.GetConfig(_currentSoldierCfgID);
        SoldierLevelConfig cfgLevel = SoldierLevelConfigLoader.GetConfig(_currentSoldierCfgID, level);

        // 检查金钱
        if (UserManager.Instance.Money < cfgLevel.UpgradeCost) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
            return;
        }
        
        CityManager.Instance.RequestTrainSoldier(_currentSoldierCfgID, cfgLevel.UpgradeCost);

        UIManager.Instance.CloseWindow<UICityTrainSelectView>();
        CloseWindow();
    }
}
