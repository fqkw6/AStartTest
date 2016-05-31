using UnityEngine;
using UnityEngine.UI;

// 建造信息展示
public class BuildingInfoPanel : MonoBehaviour
{
    public Image _imageIcon;
    public Image _prgTime;
    public Text _textTime;
    public Sprite _levelupSprite;

    private BuildingInfo _currentInfo;

    private void Start()
    {

    }

    public void Show(bool show)
    {
        if (show) {
            SetInfo(_currentInfo);
        } else {
            gameObject.SetActive(false);
        }
    }

    public void SetInfo(BuildingInfo info)
    {
        _currentInfo = info;

        if (_currentInfo == null) return;

        bool refresh = false;
        if (_currentInfo.IsInBuilding()) {
            // 建筑正在升级
            _imageIcon.sprite = _levelupSprite;
            refresh = true;
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 如果是兵营的话
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                // 如果正在生产士兵，则显示士兵头像
                _imageIcon.sprite = ResourceManager.Instance.GetSoldierIcon(tbinfo.SoldierConfigID);
                refresh = true;
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                _imageIcon.sprite = ResourceManager.Instance.GetSoldierIcon(tbinfo.TrainSoldierCfgID);
                refresh = true;
            }
        }

        if (refresh) {
            // 开启倒计时
            gameObject.SetActive(true);
            InvokeRepeating("UpdateTime", 0, 0.1f);
        } else {
            // 没有需要倒计时的
            gameObject.SetActive(false);
            CancelInvoke("UpdateTime");
        }
    }

    void UpdateTime()
    {
        if (_currentInfo.IsInBuilding()) {
            // 建筑正在升级
            int cd = _currentInfo.GetLevelUpCD();
            _prgTime.fillAmount = 1.0f * cd / Utils.GetSeconds(_currentInfo.CfgLevel.UpgradeTime);
            _textTime.text = Utils.GetCountDownString(cd);
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 如果是兵营的话
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                // 如果正在生产士兵，则显示士兵头像
                int cd = tbinfo.GetProducingCD();
                _prgTime.fillAmount = 1.0f * cd / tbinfo.GetMaxProduceTime();
                _textTime.text = Utils.GetCountDownString(cd);
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                int cd = tbinfo.GetTrainCD();
                _prgTime.fillAmount = 1.0f * cd / tbinfo.GetMaxTrainTime();
                _textTime.text = Utils.GetCountDownString(cd);
            }
        }
    }
}
