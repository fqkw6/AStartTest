using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProduceInfoPanel : MonoBehaviour
{
    public Image _imageIcon;
    
    private ProduceBuildingInfo _currentInfo;

	void Start () {
	
	}
	
    public void SetInfo(BuildingInfo info)
    {
        _currentInfo = info as ProduceBuildingInfo;

        if (_currentInfo == null) return;

        if (_currentInfo.BuildingType == CityBuildingType.HOUSE) {
            _imageIcon.sprite = ResourceManager.Instance.GetResIcon(ResourceType.MONEY);
        } else if (_currentInfo.BuildingType == CityBuildingType.WOOD) {
            _imageIcon.sprite = ResourceManager.Instance.GetResIcon(ResourceType.WOOD);
        } else if (_currentInfo.BuildingType == CityBuildingType.STONE) {
            _imageIcon.sprite = ResourceManager.Instance.GetResIcon(ResourceType.STONE);
        }
        
        // 点击会隐藏资源图标，无论有没有收集完毕，即使没有收集完毕，在一定时间内也不会重复显示图标
        if (_currentInfo.IsInBuilding() || _currentInfo.LastClickTime.IsValid() && _currentInfo.LastClickTime.GetTime() <= GameConfig.PRODUCE_REWARD_INTERVAL) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(_currentInfo.GetCurrentProduceValue() > 0);
        }

        UpdateIconColor();
    }

    public void Show(bool show)
    {
        if (show && _currentInfo.IsInBuilding()) return;
        gameObject.SetActive(show);

        UpdateIconColor();
    }

    public void UpdateIconColor()
    {
        if (_currentInfo.IsProduceFull()) {
            _imageIcon.color = Color.red;
        } else {
            _imageIcon.color = Color.white;
        }
    }

    public void OnClick()
    {
        if (gameObject.activeInHierarchy) {
            _currentInfo.LastClickTime.SetTime(0);
        }

        Show(false);

        ProduceBuildingInfo pbinfo = _currentInfo as ProduceBuildingInfo;
        if (pbinfo == null) return;

        if (!pbinfo.IsProduceFull() && pbinfo.GetCurrentProduceValue() > 0) {
            // 如果资源未满，并且有产出 则请求收货
            CityManager.Instance.RequestHarvest(_currentInfo.EntityID);
            if (pbinfo.BuildingType == CityBuildingType.WOOD) {
                EventDispatcher.TriggerEvent(EventID.EVENT_CITY_AWARD_WOOD);
            } else if (pbinfo.BuildingType == CityBuildingType.STONE) {
                EventDispatcher.TriggerEvent(EventID.EVENT_CITY_AWARD_STONE);
            }
        } else {
            // 资源已满
            UIUtil.ShowMsgFormat("UI_MSG_RES_FULL", _currentInfo.GetContainerBuildingName(), _currentInfo.GetResName());
        }
    }
}
