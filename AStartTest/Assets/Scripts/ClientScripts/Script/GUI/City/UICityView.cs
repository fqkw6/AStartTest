using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using ETouch;

public partial class EventID
{
    public const string EVENT_CITY_AWARD_WOOD = "EVENT_CITY_AWARD_WOOD";
    public const string EVENT_CITY_AWARD_STONE = "EVENT_CITY_AWARD_STONE";
}

// 主城地图，包括地图层、建筑、动画元素等
// 需要单独使用一个Canvas来渲染，因为其多分辨率的处理机制跟普通ui不同，需要保证不留黑边
// 需要有一个MapCamera，以便加入3d和粒子光效，但是Camera不能添加到Canvas下，且其具体配置与显示无关
// Canvas会自动根据Camera的设置调整其缩放比例
// 添加到这个Canvas下面的元素，会根据地图缩放而改变大小，如建筑的倒计时等
public class UICityView : UIWindow
{
    public const string Name = "City/UICityView";
    public BuildingInfoPanel _buildingInfoPanelPrefab;
    public ProduceInfoPanel _produceInfoPanelPrefab;
    public TroopInfoPanel _troopInfoPanelPrefab;
    public GameObject _unlockPrefab;

    public List<CityBuildingUI> _buildingUIList;
    
    public CityCart _objCart;
    private CityCart _cart;

    private List<CityCitizen> _citizens = new List<CityCitizen>();
    private List<CitySoldier> _soldiers = new List<CitySoldier>(); 
    private List<CityBuilding> _buildingList = new List<CityBuilding>();
    
    public override void OnOpenWindow()
    {
        IsMainWindow = true;

        InitBuildings();
        
        _buildingInfoPanelPrefab.gameObject.SetActive(false);
        _produceInfoPanelPrefab.gameObject.SetActive(false);

        RectTransform rt = transform as RectTransform;
        foreach (var item in _buildingList) {
            item.Parent = rt;
        }

        foreach (var item in _buildingUIList) {
            CityBuilding building = GetBuildingByCfgID(item._buildingCfgID);
            item.Building = building;
            item.Parent = rt;
            item._buildingInfoPanelPrefab = _buildingInfoPanelPrefab;
            item._produceInfoPanelPrefab = _produceInfoPanelPrefab;
            item._troopInfoPanelPrefab = _troopInfoPanelPrefab;
            item._unlockPrefab = _unlockPrefab;
        }

        // 刷新建筑等级等信息
        CityManager.Instance.RequestBuildingList();

        EventDispatcher.AddEventListener<long>(EventID.EVENT_CITY_BUILDING_LEVELUP, OnBuildingLevelup);
        EventDispatcher.AddEventListener<long>(EventID.EVENT_CITY_BUILDING_LEVELUP_FINISH, OnBuildingLevelupFinish);
        EventDispatcher.AddEventListener<long>(EventID.EVENT_CITY_BUILDING_REFRESH, OnBuildingRefresh);

        EventDispatcher.AddEventListener(EventID.EVENT_CITY_AWARD_WOOD, OnAwardWood);
        EventDispatcher.AddEventListener(EventID.EVENT_CITY_AWARD_STONE, OnAwardStone);

        EasyTouch.On_TouchDown += On_TouchDown;
        EasyTouch.On_SimpleTap += On_SimpleTap;

        // 随机产生20个城镇居民
        SpawnCitizen(20);

        _cart = Instantiate(_objCart);
        _cart.transform.position = _cart._originPosition;
    }

    // 产生城镇居民
    private void SpawnCitizen(int maxCount)
    {
//        float range = 50;
//        GameObject prefab = Resources.Load<GameObject>("GUI/City/Scene/gongnengbing");
//        for (int i = 0; i < maxCount; ++i) {
//            GameObject go = Instantiate(prefab);
//            Vector2 random = UnityEngine.Random.insideUnitCircle;
//            go.transform.position = new Vector3(random.x * range, 2.8f, random.y * range);
//            _citizens.Add(go.GetComponent<CityCitizen>());
//        }
    }

    // 初始化建筑
    private void InitBuildings()
    {
        _buildingList.Clear();

        // 根据配置中的建筑物物件名字添加建筑
        foreach (var item in BuildingConstConfigLoader.Data) {
            AddBuildingByName(item.Value.BuildingPinyin, item.Key);
        }
    }

    private void AddBuildingByName(string objName, int cfgID)
    {
        GameObject go = GameObject.Find(objName);
        if (go == null) return;

        CityBuilding building = go.GetComponent<CityBuilding>();
        if (building == null) {
            building = go.AddComponent<CityBuilding>();
        }
        
        building._buildingCfgID = cfgID;

        _buildingList.Add(building);
    }

    public override void OnRefreshWindow()
    {
        foreach (var item in _buildingList) {
            item.Refresh();
        }

        foreach (var item in _buildingUIList) {
            item.Refresh();
        }
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener<long>(EventID.EVENT_CITY_BUILDING_LEVELUP, OnBuildingLevelup);
        EventDispatcher.RemoveEventListener<long>(EventID.EVENT_CITY_BUILDING_LEVELUP_FINISH, OnBuildingLevelupFinish);
        EventDispatcher.RemoveEventListener<long>(EventID.EVENT_CITY_BUILDING_REFRESH, OnBuildingRefresh);

        EventDispatcher.RemoveEventListener(EventID.EVENT_CITY_AWARD_WOOD, OnAwardWood);
        EventDispatcher.RemoveEventListener(EventID.EVENT_CITY_AWARD_STONE, OnAwardStone);

        EasyTouch.On_TouchDown -= On_TouchDown;
        EasyTouch.On_SimpleTap -= On_SimpleTap;

        foreach (var item in _citizens) {
            Destroy(item.gameObject);
        }

        foreach (var item in _soldiers) {
            Destroy(item.gameObject);
        }

        if (_cart != null) {
            Destroy(_cart.gameObject);
            _cart = null;
        }
    }

    // 获取建筑模型
    private CityBuilding GetBuilding(long buildingID)
    {
        return _buildingList.Find((x) => x.EntityID == buildingID);
    }

    private CityBuilding GetBuildingByCfgID(int cfgID)
    {
        return _buildingList.Find((x) => x._buildingCfgID == cfgID);
    }

    private CityBuildingUI GetBuildingUI(long buildingID)
    {
        return _buildingUIList.Find((x) => x.EntityID == buildingID);
    }

    private void OnAwardWood()
    {
        if (_cart != null) {
            _cart.PlayWood();
        }
    }

    private void OnAwardStone()
    {
        if (_cart != null) {
            _cart.PlayStone();
        }
    }

    // 建筑开始升级
    private void OnBuildingLevelup(long buildingID)
    {
        CityBuilding building = GetBuilding(buildingID);
        if (building != null) {
            building.Refresh();

            // 遍历所有的工人，选三个最近的
            List<CityCitizen> list = new List<CityCitizen>();
            foreach (var item in _citizens) {
                if (item.CouldWork()) {
                    list.Add(item);
                }
            }

            list.Sort((a, b) =>
            {
                float disA = Vector3.Distance(gameObject.transform.position, a.transform.position);
                float disB = Vector3.Distance(gameObject.transform.position, b.transform.position);
                return disA.CompareTo(disB);
            });
            list = list.GetRange(0, Mathf.Min(3, list.Count));
            foreach (var item in list) {
                item.RunToBuilding(building);
            }
        }

        CityBuildingUI buildingUI = GetBuildingUI(buildingID);
        if (buildingUI != null) {
            buildingUI.Refresh();
        }
    }

    private void OnBuildingRefresh(long buildingID)
    {
        CityBuilding building = GetBuilding(buildingID);
        if (building != null) {
            building.Refresh();
        }

        CityBuildingUI buildingUI = GetBuildingUI(buildingID);
        if (buildingUI != null) {
            buildingUI.Refresh();
        }
    }

    // 建筑升级结束
    private void OnBuildingLevelupFinish(long buildingID)
    {
        CityBuilding building = GetBuilding(buildingID);
        if (building != null) {
            building.Refresh();
            building.OnLevelUp();
        }

        // 正在工作的工人结束建筑
        foreach (var item in _citizens) {
            if (item.IsWorking()) {
                item.WorkFinish();
            }
        }

        CityBuildingUI buildingUI = GetBuildingUI(buildingID);
        if (buildingUI != null) {
            buildingUI.Refresh();
        }

        // 刷新生产建筑的图标（资源满、未满的提示区分）
        foreach (var item in _buildingUIList) {
            item.UpdatePanel();
        }
    }

    // 检测一下点击开始时是否点到ui，防止因为点击操作造成ui关闭后，Tap事件处理不正常的问题
    private bool _touchUI = false;
    void On_TouchDown(ETouch.Gesture gesture)
    {
        _touchUI = UIUtil.IsTouchUI();
    }

    void On_SimpleTap(ETouch.Gesture gesture)
    {
        if (_touchUI || UIUtil.IsTouchUI()) {
            return;
        }

        if (Camera.main == null) return;
        Ray ray = Camera.main.ScreenPointToRay(gesture.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            CityBuilding building = hit.collider.GetComponent<CityBuilding>();
            if (building != null) {
                building.OnClick();
            }
        }
    }
}
