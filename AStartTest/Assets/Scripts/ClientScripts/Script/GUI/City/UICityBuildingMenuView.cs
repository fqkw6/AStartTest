using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

// 点击正在升级中的建筑，可以弹出这个菜单界面，可以取消升级或者快速完成升级；生产和升级兵种也是如此
// 生产兵种和升级建筑时无法做任何操作
public class UICityBuildingMenuView : UIWindow
{
    public const string Name = "City/UICityBuildingMenuView";
    public Image _leftIcon;
    public Image _progress;
    public Text _time;
    public Text _cost;
    public Image _imgFlag;
    public RectTransform _buttonAnimationStart;
    public Sprite _levelUpIcon;
    public Button _btnCancel;
    public Button _btnQuick;
    public Button _btnEnter;
    
    protected BuildingInfo _currentInfo;
    private CityBuilding _building;
    private RectTransform _cityViewTransform;

    public override void OnOpenWindow()
    {
        EventDispatcher.AddEventListener(EventID.EVENT_CITY_BUILDING_MENU_CLOSE, OnClickClose);
        PlayStartAnimation();

    }

    public override void OnBindData(params object[] param)
    {
        SetInfo(param[0] as BuildingInfo);

        _building = param[1] as CityBuilding;
        _cityViewTransform = param[2] as RectTransform;
        
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_SHOW_PANEL, _currentInfo.EntityID, false);
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener(EventID.EVENT_CITY_BUILDING_MENU_CLOSE, OnClickClose);
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_SHOW_PANEL, _currentInfo.EntityID, true);
    }

    void Update()
    {
        if (_building == null || _cityViewTransform == null || Camera.main == null) return;

        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform == null) return;

        Vector2 uiPos;
        Canvas canvas = UIManager.Instance.Canvas;
        Vector3 buildingPos = Camera.main.WorldToScreenPoint(_building.transform.position);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_cityViewTransform, buildingPos, canvas.worldCamera, out uiPos)) {

            if (_currentInfo.BuildingType == CityBuildingType.WOOD) {
                rectTransform.anchoredPosition = uiPos + new Vector2(0, 200);
            } else {
                rectTransform.anchoredPosition = uiPos;
            }
        }
    }

    private void PlayStartAnimation()
    {
        Vector3 pos1 = _btnCancel.transform.localPosition;
        Vector3 pos2 = _btnQuick.transform.localPosition;

        _btnCancel.transform.localPosition = _buttonAnimationStart.localPosition;
        _btnQuick.transform.localPosition = _buttonAnimationStart.localPosition;

        _btnCancel.transform.DOLocalMove(pos1, 0.3f).SetEase(Ease.OutBack);
        _btnQuick.transform.DOLocalMove(pos2, 0.3f).SetEase(Ease.OutBack);

        if (_btnEnter != null) {
            Vector3 pos3 = _btnEnter.transform.localPosition;
            _btnEnter.transform.localPosition = _buttonAnimationStart.localPosition;
            _btnEnter.transform.DOLocalMove(pos3, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private void SetInfo(BuildingInfo info)
    {
        _currentInfo = info;
        if (_currentInfo == null) return;

        if (_currentInfo.IsInBuilding()) {
            // 建筑正在升级
            _leftIcon.sprite = _levelUpIcon;
            _cost.text = _currentInfo.GetQuickLevelUpCost(true).ToString();
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 如果是兵营的话
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                // 如果正在生产士兵，则显示士兵头像
                _leftIcon.sprite = ResourceManager.Instance.GetSoldierIcon(tbinfo.SoldierConfigID);
                _cost.text = tbinfo.GetQuickProducingCost().ToString();
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                _leftIcon.sprite = ResourceManager.Instance.GetSoldierIcon(tbinfo.TrainSoldierCfgID);
                _cost.text = tbinfo.GetQuickTrainCost().ToString();
            }
        }

        RectTransform rc = _imgFlag.transform as RectTransform;
        if (rc) {
            rc.anchoredPosition = new Vector2(-(_imgFlag.preferredWidth + _cost.preferredWidth) / 2 - 2, rc.anchoredPosition.y);
        }

        InvokeRepeating("UpdateTime", 0, 1);
    }

    void UpdateTime()
    {
        if (_currentInfo.IsInBuilding()) {
            // 建筑正在升级
            int cd = _currentInfo.GetLevelUpCD();
            _progress.fillAmount = 1.0f * cd / Utils.GetSeconds(_currentInfo.CfgLevel.UpgradeTime);
            _time.text = Utils.GetCountDownString(cd);
        } else if (_currentInfo.BuildingType == CityBuildingType.TROOP) {
            // 如果是兵营的话
            TroopBuildingInfo tbinfo = _currentInfo as TroopBuildingInfo;
            if (tbinfo != null && tbinfo.IsProducingSoldier()) {
                // 如果正在生产士兵，则显示士兵头像
                int cd = tbinfo.GetProducingCD();
                _progress.fillAmount = 1.0f * cd / tbinfo.GetMaxProduceTime();
                _time.text = Utils.GetCountDownString(cd);
            }
        } else if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            TrainBuildingInfo tbinfo = _currentInfo as TrainBuildingInfo;
            if (tbinfo != null && tbinfo.IsTrainingSoldier()) {
                int cd = tbinfo.GetTrainCD();
                _progress.fillAmount = 1.0f * cd / tbinfo.GetMaxTrainTime();
                _time.text = Utils.GetCountDownString(cd);
            }
        }
    }

    // 取消任务
    public void OnClickCancel()
    {
        UIManager.Instance.OpenWindow<UIMsgBoxCancelView>(_currentInfo);
        CloseWindow();
    }

    // 快速完成任务
    public void OnClickQuick()
    {
        // 显示确认框
        UIManager.Instance.OpenWindow<UIMsgBoxQuickView>(_currentInfo);
        CloseWindow();
    }

    // 当校场升级或者研究的时候，依然可以进入校场
    public void OnClickEnter()
    {
        if (_currentInfo.BuildingType == CityBuildingType.TRAIN) {
            UIManager.Instance.OpenWindow<UICityTrainSelectView>();
            CloseWindow();
        }
    }
}
