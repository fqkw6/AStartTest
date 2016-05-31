using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 官邸信息界面
public class UICityPalaceInfoView : UIWindow
{
    public const string Name = "City/UICityPalaceInfoView";
    public Text _title;
    public Text _txtLevel;
    public Text _desc;
    public Image _buildingImage;
    public Text _maxContainValue;
    public Text _maxContainValueWood;
    public Text _maxContainValueStone;
    public Text _levelupTime;

    public Button _btnLevelUp;

    private BuildingInfo _currentInfo = null;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        SetInfo(param[0] as BuildingInfo);
    }

    public override void OnRefreshWindow()
    {
    }

    private void SetInfo(BuildingInfo info)
    {
        _currentInfo = info;
        if (_currentInfo == null) return;

        _title.text = info.Cfg.BuildingName;
        _txtLevel.text = Str.Format("UI_LEVEL", info.Level);
        _desc.text = info.Cfg.BuildingDescription;

        // 如果建筑正在升级，不显示升级按钮
        _btnLevelUp.gameObject.SetActive(!_currentInfo.IsInBuilding());

        _maxContainValue.text = _currentInfo.GetMaxContainValue().ToString();
        _maxContainValueWood.text = _currentInfo.GetMaxContainValue().ToString();
        _maxContainValueStone.text = _currentInfo.GetMaxContainValue().ToString();
        _levelupTime.text = Utils.GetCountDownString(Utils.GetSeconds(_currentInfo.CfgLevel.UpgradeTime));
    }

    // 点击升级建筑
    public void OnClickLevup()
    {
        if (_currentInfo.IsMaxLevel()) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MAX_LEVEL");
            return;
        }
        UIManager.Instance.OpenWindow<UICityPalaceTrainLevelUpView>(_currentInfo);
    }
}
