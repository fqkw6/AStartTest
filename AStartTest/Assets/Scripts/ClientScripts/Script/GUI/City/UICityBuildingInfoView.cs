using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 通用的建筑信息界面
public class UICityBuildingInfoView : UIWindow
{
    public const string Name = "City/UICityBuildingInfoView";
    public Text _title;
    public Text _txtLevel;
    public Text _desc;
    public Image _buildingImage;
    public Text _textMaxContain;
    public Image _imageMaxContain;
    public Text _txtMaxContain;
    public Text _maxContainValue;
    public Text _txtProduceText;
    public Image _imageProduce;
    public Text _produceValue;
    public Text _textLevelup;
    public Text _levelupTime;
    public Text _txtFull;

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
        _buildingImage.sprite = ResourceManager.Instance.GetBuildingIcon(info.ConfigID);

         // 如果建筑正在升级，不显示升级按钮
        _btnLevelUp.gameObject.SetActive(!_currentInfo.IsInBuilding());
        
        _maxContainValue.text = _currentInfo.GetMaxContainValue().ToString();
        _levelupTime.text = Utils.GetCountDownString(Utils.GetSeconds(_currentInfo.CfgLevel.UpgradeTime));
        
        switch (_currentInfo.BuildingType) {
            case CityBuildingType.HOUSE:
                _imageMaxContain.sprite = ResourceManager.Instance.GetResIcon(ResourceType.MONEY);
                _imageProduce.sprite = ResourceManager.Instance.GetResIcon(ResourceType.MONEY);
                break;
            case CityBuildingType.STONE:
                _imageMaxContain.sprite = ResourceManager.Instance.GetResIcon(ResourceType.STONE);
                _imageProduce.sprite = ResourceManager.Instance.GetResIcon(ResourceType.STONE);
                break;
            case CityBuildingType.WOOD:
                _imageMaxContain.sprite = ResourceManager.Instance.GetResIcon(ResourceType.WOOD);
                _imageProduce.sprite = ResourceManager.Instance.GetResIcon(ResourceType.WOOD);
                break;
            case CityBuildingType.MONEY_STORAGE:
                _imageMaxContain.sprite = ResourceManager.Instance.GetResIcon(ResourceType.MONEY);
                break;
            case CityBuildingType.STONE_STORAGE:
                _imageMaxContain.sprite = ResourceManager.Instance.GetResIcon(ResourceType.STONE);
                break;
            case CityBuildingType.WOOD_STORAGE:
                _imageMaxContain.sprite = ResourceManager.Instance.GetResIcon(ResourceType.WOOD);
                break;
        }

        ProduceBuildingInfo pbinfo = _currentInfo as ProduceBuildingInfo;
        if (pbinfo != null) {
            _produceValue.text = pbinfo.GetUnitProduceValue().ToString();

            if (pbinfo.IsProduceFull()) {
                // 资源满了，提示升级建筑
                _txtFull.text = Str.Format("UI_MSG_RES_FULL", _currentInfo.GetContainerBuildingName(), _currentInfo.GetResName());
                _txtFull.gameObject.SetActive(true);
            } else {
                _txtFull.gameObject.SetActive(false);
            }

            _txtMaxContain.text = Str.Get("UI_CITY_BUILDING_MAX_CONTAIN");
            _txtProduceText.gameObject.SetActive(true);
        } else {
            _txtProduceText.gameObject.SetActive(false);
            _txtFull.gameObject.SetActive(false);
            _txtMaxContain.text = Str.Get("UI_CITY_BUILDING_CONTAIN");
        }
    }

    // 点击升级建筑
    public void OnClickLevup()
    {
        if (_currentInfo.IsMaxLevel()) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MAX_LEVEL");
            return;
        }
        UIManager.Instance.OpenWindow<UICityBuildingUplevelView>(_currentInfo);
    }
}
