using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;

// 大地图
public class UIWorldMapView : UIWindow
{
    public const string Name = "World/UIWorldMapView";
    public RectTransform _panelCity;

    public UserInfoPanel _userInfoPanelPrefab;
    public List<WorldCity> _worldCityPrefabList;
    public List<WorldCity> _resTownPrefabList;

    public MyWorldCity _myWorldCity;

    private List<WorldSlot> _slotList = new List<WorldSlot>();              // 主城池空位
    private List<WorldSlot> _resSlotList = new List<WorldSlot>();           // 资源城空位
    private List<WorldCity> _cityList = new List<WorldCity>();

    public override void OnOpenWindow()
    {
        IsMainWindow = true;

        WorldSlot[] slot = GetComponentsInChildren<WorldSlot>();
        foreach (var item in slot) {
            if (item._isResTown) {
                _resSlotList.Add(item);
            } else {
                _slotList.Add(item);
            }
        }

        // 请求大地图城池信息
        WorldManager.Instance.RequestWorldInfo();

        EventDispatcher.AddEventListener<WorldCityInfo>(EventID.EVENT_WORLD_ADD_CITY, OnAddCity);
        EventDispatcher.AddEventListener<int>(EventID.EVENT_WORLD_REMOVE_CITY, OnRemoveCity);
        EventDispatcher.AddEventListener<WorldCityInfo>(EventID.EVENT_WORLD_SWITCH_CITY, OnSwitchCity);
        EventDispatcher.AddEventListener<int>(EventID.EVENT_WORLD_REFRESH_CITY, OnRefreshCity);
        EventDispatcher.AddEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshValue);
    }

    public override void OnRefreshWindow()
    {
        InitWorldCity();
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener<WorldCityInfo>(EventID.EVENT_WORLD_ADD_CITY, OnAddCity);
        EventDispatcher.RemoveEventListener<int>(EventID.EVENT_WORLD_REMOVE_CITY, OnRemoveCity);
        EventDispatcher.RemoveEventListener<WorldCityInfo>(EventID.EVENT_WORLD_SWITCH_CITY, OnSwitchCity);
        EventDispatcher.RemoveEventListener<int>(EventID.EVENT_WORLD_REFRESH_CITY, OnRefreshCity);
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshValue);
    }

    private void InitWorldCity()
    {
        foreach (var item in _cityList) {
            Destroy(item.gameObject);
        }

        _cityList.Clear();
        
        // 城池列表
        foreach (var item in WorldManager.Instance.CityList) {
            CreateWorldCity(item);
        }
    }

    private void CreateWorldCity(WorldCityInfo info)
    {
        WorldCity city = null;
        var townInfo = info as WorldResTownInfo;
        if (townInfo != null) {
            // 资源城
            if (townInfo.ProduceType == ResourceType.STONE) {
                city = Instantiate(_resTownPrefabList[1]);
            } else {
                city = Instantiate(_resTownPrefabList[0]);
            }
        } else {
            // 主城
            city = Instantiate(_worldCityPrefabList[0]);
        }

        city.gameObject.SetActive(true);
        Vector3 pos = GetSlotPosition(info.MapPosition, info is WorldResTownInfo);
        city.transform.SetParent(_panelCity, false);
        city.transform.localPosition = pos;
        city.CreateUserPanel(_userInfoPanelPrefab);
        city.SetInfo(info);

        _cityList.Add(city);
    }

    private void OnRefreshValue()
    {
        _myWorldCity.Refresh();
    }

    // 获取城池
    private WorldCity GetCity(int mapPos)
    {
        return _cityList.Find(x => x.MapPosition == mapPos);
    }

    // 获取一个空位置的坐标
    private Vector3 GetSlotPosition(int index, bool isResTown)
    {
        if (isResTown) {
            if (index >= _resSlotList.Count) {
                return Vector3.zero;
            }

            return _resSlotList[index - 1].transform.localPosition;
        } else {
            if (index > _slotList.Count) {
                return Vector3.zero;
            }

            return _slotList[index - 1].transform.localPosition;
        }
    }

    // 添加一个新的城池
    private void OnAddCity(WorldCityInfo info)
    {
        CreateWorldCity(info);
    }

    // 移除一个城池
    private void OnRemoveCity(int mapPos)
    {
        WorldCity city = GetCity(mapPos);
        if (city != null) {
            _cityList.Remove(city);
            Destroy(city.gameObject);
        }
    }

    // 更换对手
    private void OnSwitchCity(WorldCityInfo info)
    {
        WorldCity oldCity = GetCity(info.MapPosition);
        if (oldCity != null) {
            _cityList.Remove(oldCity);
            Destroy(oldCity.gameObject);
        }

        CreateWorldCity(info);
    }

    // 刷新城池数据
    private void OnRefreshCity(int mapPos)
    {
        WorldCity city = GetCity(mapPos);
        if (city != null) {
            city.Refresh();
        }
    }
}
