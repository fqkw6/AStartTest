using UnityEngine;
using System.Collections;
using ProtoBuf;
using comrt.comnet;

// 建筑物的信息
public class BuildingInfo
{
    public long EntityID;
    public int ConfigID;

    public RemainTime LevelUpRemainTime;      // 升级剩余时间

    public ElapseTime LastClickTime;    // 上次资源满的时候点击的时间

    private int _level = 0;
    public int Level                       // 当前等级
    {
        get { return _level; }
        set
        {
            _level = value;

            _cfgLevel = BuildingLevelConfigLoader.GetConfig(ConfigID, Level);
            _cfgNextLevel = BuildingLevelConfigLoader.GetConfig(ConfigID, Level + 1, false);
        }
    }
    private BuildingConstConfig _cfg;

    public BuildingConstConfig Cfg
    {
        get
        {
            if (_cfg == null) {
                _cfg = BuildingConstConfigLoader.GetConfig(ConfigID);
            }
            return _cfg;
        }
    }

    private BuildingLevelConfig _cfgLevel;
    public BuildingLevelConfig CfgLevel
    {
        get
        {
            if (_cfgLevel == null) {
                _cfgLevel = BuildingLevelConfigLoader.GetConfig(ConfigID, Level);
            }
            return _cfgLevel;
        }
    }

    private BuildingLevelConfig _cfgNextLevel;
    public BuildingLevelConfig CfgNextLevel
    {
        get
        {
            if (_cfgNextLevel == null) {
                _cfgNextLevel = BuildingLevelConfigLoader.GetConfig(ConfigID, Level + 1, false);
            }
            return _cfgNextLevel;
        }
    }

    public CityBuildingType BuildingType
    {
        get { return (CityBuildingType) Cfg.BuildingType; }
    }

    public virtual void Deserialize(PBuildInfo data)
    {
        EntityID = data.buildId;
        ConfigID = data.cfgId;
        Level = data.level;
        LevelUpRemainTime.SetTimeMilliseconds(data.nextStatusTime);
    }

    // 此建筑是否满级（根据level表判断）
    public bool IsMaxLevel()
    {
        return CfgNextLevel == null;
    }

    public void OnLevelUpFinish()
    {
        LevelUpRemainTime.Reset();
        CityManager.Instance.UpdateMaxStorage();

        // 信息提示
        UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_FINISH", Cfg.BuildingName, Level);

        // 刷新地图
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_REFRESH, EntityID);
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_MENU_CLOSE);
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_LEVELUP_FINISH, EntityID);
    }

    // 获取建筑的最大储量
    public int GetMaxContainValue()
    {
        return CfgLevel.MaxStorage;
    }

    // 获取下一个等级增加的最大储量
    public int GetNextMaxContainValue()
    {
        if (IsMaxLevel()) {
            return 0;
        } else {
            return CfgNextLevel.MaxStorage - CfgLevel.MaxStorage;
        }
    }

    // 建筑是否正在升级中
    public bool IsInBuilding()
    {
        return LevelUpRemainTime.IsValid() && LevelUpRemainTime.GetTime() > 0;
    }

    // 获取建筑升级倒计时
    public int GetLevelUpCD()
    {
        if (!IsInBuilding()) {
            return Utils.GetSeconds(CfgLevel.UpgradeTime);
        } else {
            return Mathf.Max((int)LevelUpRemainTime.GetTime(), 0);
        }
    }

    // 获取建筑快速升级所需要的消耗
    public int GetQuickLevelUpCost(bool enableFreeTime)
    {
        int time = GetLevelUpCD();

        // 小于一定时间免费 TODO将来改了ui在执行此逻辑
        //if (enableFreeTime && time <= GlobalVariable.QUICK_LEVELUP_FREE_TIME) {
        //    return 0;
        //}

        return Formula.GetLevelUpQuickCost(time);
    }

    // 获取产出的资源的名字
    public string GetResName()
    {
        switch (BuildingType) {
            case CityBuildingType.HOUSE:
                return Str.Get("UI_CITY_BUILDING_MONEY");
            case CityBuildingType.STONE:
                return Str.Get("UI_CITY_BUILDING_STONE");
            case CityBuildingType.WOOD:
                return Str.Get("UI_CITY_BUILDING_WOOD");
        }

        return null;
    }

    // 获取对应仓库的名字
    public string GetContainerBuildingName()
    {
        CityBuildingType bt;
        switch (BuildingType) {
            case CityBuildingType.HOUSE:
                bt = CityBuildingType.MONEY_STORAGE;
                break;
            case CityBuildingType.STONE:
                bt = CityBuildingType.STONE_STORAGE;
                break;
            case CityBuildingType.WOOD:
                bt = CityBuildingType.WOOD_STORAGE;
                break;
            default:
                return null;
        }

        foreach (var item in BuildingConstConfigLoader.Data) {
            if (item.Value.BuildingType == (int) bt) {
                return item.Value.BuildingName;
            }
        }

        return null;
    }
}
