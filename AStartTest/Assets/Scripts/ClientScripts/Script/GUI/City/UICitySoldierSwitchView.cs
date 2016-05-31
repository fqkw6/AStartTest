using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 兵营不为空的时候，选择切换兵种的界面
public class UICitySoldierSwitchView : UIWindow
{
    public const string Name = "City/UICitySoldierSwitchView";
    public Text _title;
    public Image _soldierImage;
    public Image _soldierIcon1;
    public Text _soldierCount1;
    public Image _soldierIcon2;
    public Text _soldierCount2;
    public Text _textCost;
    public Text _textTime;
    public Text _textDesc;

    public Text _text3;
    public Image _imageCostFlag;
    public Image _imageTimeFlag;
    public Button _btnAddSoldier;

    private TroopBuildingInfo _currentInfo = null;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        SetInfo(param[0] as BuildingInfo);
    }

    private void SetInfo(BuildingInfo info)
    {
        _currentInfo = info as TroopBuildingInfo;
        if (_currentInfo == null) return;

        int cfgID = _currentInfo.SoldierConfigID;
        SoldierConfig cfg = SoldierConfigLoader.GetConfig(cfgID);
        int level = Mathf.Max(CityManager.Instance.GetSoldierLevel(cfgID), 1);

        SoldierLevelConfig cfgLevel = SoldierLevelConfigLoader.GetConfig(cfgID, level);

        _title.text = cfg.SoldierName + " Lv" + level;
        _soldierImage.sprite = ResourceManager.Instance.GetSoldierImage(cfgID);
        _textDesc.text = cfg.SoldierDescription;

        _soldierIcon1.sprite = ResourceManager.Instance.GetSoldierIcon(cfgID);
        _soldierIcon2.sprite = ResourceManager.Instance.GetSoldierIcon(cfgID);
        _soldierCount1.text = _currentInfo.SoldierCount.ToString();

        int addCount = _currentInfo.GetMaxSoldierCount(cfgID) - _currentInfo.SoldierCount;
        if (addCount > 0) {
            _soldierCount2.text = addCount.ToString();
            _textCost.text = (addCount * cfgLevel.ProduceCost).ToString();
            _textTime.text = Utils.GetCountDownString(addCount*Utils.GetSeconds(_currentInfo.SoldierCfg.Producetime));

            _soldierIcon2.gameObject.SetActive(true);
            _textCost.gameObject.SetActive(true);
            _textTime.gameObject.SetActive(true);
            _text3.gameObject.SetActive(true);
            _imageCostFlag.gameObject.SetActive(true);
            _imageTimeFlag.gameObject.SetActive(true);
            _btnAddSoldier.gameObject.SetActive(true);
        } else {
            // 兵营满了，不需要补充
            _soldierIcon2.gameObject.SetActive(false);
            _textCost.gameObject.SetActive(false);
            _textTime.gameObject.SetActive(false);
            _text3.gameObject.SetActive(false);
            _imageCostFlag.gameObject.SetActive(false);
            _imageTimeFlag.gameObject.SetActive(false);
            _btnAddSoldier.gameObject.SetActive(false);
        }
    }

    // 切换士兵
    public void OnClickChangeSoldier()
    {
        UIManager.Instance.OpenWindow<UICitySoldierSelectView>(_currentInfo);
        CloseWindow();
    }

    // 补充士兵
    public void OnClickAddSoldier()
    { 
        CityManager.Instance.RequestProduceSoldier(_currentInfo.EntityID, _currentInfo.SoldierConfigID, _currentInfo.GetAddCost(_currentInfo.SoldierConfigID));
        CloseWindow();
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
