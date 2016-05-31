using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

public partial class EventID
{
    public const string EVENT_CITY_BUILDING_LEVELUP = "EVENT_CITY_BUILDING_LEVELUP";            // 升级某个建筑
    public const string EVENT_CITY_BUILDING_LEVELUP_FINISH = "EVENT_CITY_BUILDING_LEVELUP_FINISH";     // 建筑升级完毕 参数为建筑info
    public const string EVENT_CITY_BUILDING_REFRESH = "EVENT_CITY_BUILDING_REFRESH";            // 刷新某个建筑信息
    public const string EVENT_CITY_BUILDING_MENU_CLOSE = "EVENT_CITY_BUILDING_MENU_CLOSE";         // 建筑或兵种升级完毕或者士兵产出完毕后关闭菜单
    public const string EVENT_CITY_BUILDING_SHOW_PANEL = "EVENT_CITY_BUILDING_SHOW_PANEL";         // 控制升级进度的显示和隐藏
    public const string EVENT_CITY_BUILDING_SHOW_PRODUCE_PANEL = "EVENT_CITY_BUILDING_SHOW_PRODUCE_PANEL";         // 控制收获资源图标的显示和隐藏
}

public enum CityBuildingType
{
    TROOP = 1,//1 兵营
    HOUSE = 2,// 民房
    STONE = 3,// 采石场
    WOOD = 4,// 伐木场
    STONE_STORAGE,//5 石材库
    WOOD_STORAGE,//6 木材库
    MONEY_STORAGE,//7 银库
    TRAIN = 8, //校场
    PALACE = 9,// 主城
    SMITHY = 10,    // 铁匠铺
    COLLEGE = 11,   // 书院
}

//产生的资源类型
public enum ResourceType
{
    NONE = 0,       // 不产出
    SOLDIER = 1,    // 士兵
    MONEY = 2,      // 银两
    WOOD = 3,       // 木材
    STONE = 4,      // 石材
    GOLD = 5,       // 黄金
}


public partial class CityManager
{
    public static readonly CityManager Instance = new CityManager();

    public List<BuildingInfo> BuildingList = new List<BuildingInfo>();
    public Dictionary<int, int> SoldierLevelList = new Dictionary<int, int>();
    
    public BuildingInfo CreateBuilding(int cfgID)
    {
        BuildingConstConfig cfg = BuildingConstConfigLoader.GetConfig(cfgID);
        if (cfg != null) {
            CityBuildingType bt = (CityBuildingType)cfg.BuildingType;
            switch (bt) {
                case CityBuildingType.HOUSE:
                case CityBuildingType.WOOD:
                case CityBuildingType.STONE:
                    return new ProduceBuildingInfo();
                case CityBuildingType.MONEY_STORAGE:
                case CityBuildingType.STONE_STORAGE:
                case CityBuildingType.WOOD_STORAGE:
                case CityBuildingType.PALACE:
                case CityBuildingType.SMITHY:
                case CityBuildingType.COLLEGE:
                    return new BuildingInfo();
                case CityBuildingType.TRAIN:
                    return new TrainBuildingInfo();
                case CityBuildingType.TROOP:
                    return new TroopBuildingInfo();
            }
        }

        return null;
    }

    public BuildingInfo GetBuilding(long bulidingID)
    {
        return BuildingList.Find(x => x.EntityID == bulidingID);
    }

    public BuildingInfo GetBuildingByType(CityBuildingType bt)
    {
        return BuildingList.Find(x => x.BuildingType == bt);
    }
    
    public BuildingInfo GetBuildingByConfigID(int cfgID)
    {
        return BuildingList.Find((x) => x.ConfigID == cfgID);
    }

    public int GetSoldierLevel(int cfgID)
    {
        int level = 0;
        SoldierLevelList.TryGetValue(cfgID, out level);
        return level;
    }

    // 主城等级
    public int GetPalaceLevel()
    {
        BuildingInfo building = GetBuildingByType(CityBuildingType.PALACE);
        return building != null ? building.Level : 0;
    }

    // 获取校场等级
    public int GetTrainLevel()
    {
        BuildingInfo building = GetBuildingByType(CityBuildingType.TRAIN);
        return building != null ? building.Level : 0;
    }

    // 返回资源的总单位产量
    public int GetTotalProduce(ResourceType resType)
    {
        int value = 0;
        if (resType == ResourceType.MONEY) {
            foreach (var item in BuildingList) {
                if (item.BuildingType == CityBuildingType.HOUSE) {
                    value += item.CfgLevel.OutputNumber;
                }
            }
            return value;
        } else if (resType == ResourceType.WOOD) {
            foreach (var item in BuildingList) {
                if (item.BuildingType == CityBuildingType.WOOD) {
                    value += item.CfgLevel.OutputNumber;
                }
            }
            return value;
        } else if (resType == ResourceType.STONE) {
            foreach (var item in BuildingList) {
                if (item.BuildingType == CityBuildingType.STONE) {
                    value += item.CfgLevel.OutputNumber;
                }
            }
            return value;
        }

        return 0;
    }

    // 请求建筑列表
    public void RequestBuildingList()
    {
        Net.Send(eCommand.GET_BUILD_INFO);
    }

    public bool IsInBuilding()
    {
        foreach (var item in BuildingList) {
            if (item.IsInBuilding()) {
                return true;
            }
        }
        return false;
    }

    // 请求升级建筑
    public void RequestUpgradeBuilding(long buildingID)
    {
        if (IsInBuilding()) {
            return;
        }

        BuildingInfo infoSend = GetBuilding(buildingID);
        if (infoSend == null) return;

        PCMLongInt data = new PCMLongInt();
        data.arg1 = buildingID;
        data.arg2 = infoSend.Cfg.BuildingType;

        Net.Send(eCommand.UPGRADE_BUILD_LEVEL, data, (byte[] buffer) =>
        {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.UPGRADE_BUILD_LEVEL)) return;

            BuildingInfo info = GetBuilding(buildingID);
            if (info != null) {
                // 扣除相应资源
                UserManager.Instance.CostMoney(info.CfgLevel.CostStone, PriceType.STONE);
                UserManager.Instance.CostMoney(info.CfgLevel.CostWood, PriceType.WOOD);

                info.Deserialize(ret);
            }

            // 升级的时候有增加的资源（当资源满的特殊情况）
            if (ret.addNum > 0) {
                DoHarvest(buildingID, ret.addNum);
            }

            RefreshUI(buildingID);
            EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_LEVELUP, buildingID);
        });
    }
    
    // 检查建筑升级的条件
    public bool CheckBuildingLevelUp(BuildingInfo info, bool quick, bool enableFreeTime)
    {
        // 尚未解锁或者是正在升级中
        if (info == null || info.IsInBuilding()) return false;

        // 主城等级检查
        if (GetPalaceLevel() < info.CfgLevel.HomeLevelDemand) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_NEED_LEVEL", info.CfgLevel.HomeLevelDemand);
            return false;
        }

        // 玩家等级检查
        if (UserManager.Instance.Level < info.CfgLevel.PlayerLevelDemand) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_NEED_PLAYER_LEVEL", info.CfgLevel.PlayerLevelDemand);
            return false;
        }

        // 快速升级黄金检查
        if (quick) {
            if (info.IsInBuilding()) {
                if (UserManager.Instance.Gold < info.GetQuickLevelUpCost(enableFreeTime)) {
                    UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_GOLD_LIMIT");
                    return false;
                }
            }
        }

        // 资源检查
        if (UserManager.Instance.Wood < info.CfgLevel.CostWood) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_WOOD_LIMIT");
            return false;
        }

        if (UserManager.Instance.Stone < info.CfgLevel.CostStone) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_STONE_LIMIT");
            return false;
        }

        if (BuildingLevelConfigLoader.GetConfig(info.ConfigID, info.Level + 1, false) == null) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MAX_LEVEL");
            return false;
        }

        return true;
    }

    // 请求收获
    public void RequestHarvest(long buildingID)
    {
        BuildingInfo infoSend = GetBuilding(buildingID);
        if (infoSend == null) return;

        PCMLongInt data = new PCMLongInt();
        data.arg1 = buildingID;
        data.arg2 = infoSend.Cfg.BuildingType;

        Net.Send(eCommand.COLLECT_RESOURCE, data, (byte[] buffer) => 
        {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.COLLECT_RESOURCE)) return;

            DoHarvest(buildingID, ret.addNum);
            
            // 刷新城池
            EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_REFRESH, buildingID);

            // 刷新主界面的数据
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });   
    }

    // 执行收获逻辑
    private void DoHarvest(long buildingID, int value)
    {
        ProduceBuildingInfo info = GetBuilding(buildingID) as ProduceBuildingInfo;
        if (info == null) return;

        // 实际增加资产
        switch (info.BuildingType) {
            case CityBuildingType.HOUSE:
                UserManager.Instance.Money += value;
                UIUtil.AddFloatingMsg(Str.Get("UI_CITY_BUILDING_MONEY") + "+" + value.ToString());
                break;
            case CityBuildingType.WOOD:
                UserManager.Instance.Wood += value;
                UIUtil.AddFloatingMsg(Str.Get("UI_CITY_BUILDING_WOOD") + "+" + value.ToString());
                break;
            case CityBuildingType.STONE:
                UserManager.Instance.Stone += value;
                UIUtil.AddFloatingMsg(Str.Get("UI_CITY_BUILDING_STONE") + "+" + value.ToString());
                break;
        }

        // 收获完清理建筑的产值
        info.ClearProduceValue();
    }

    // 根据建筑计算玩家的最大储量（银两、木材、石材）
    public void UpdateMaxStorage()
    {
        int maxMoney = 0;
        int maxWood = 0;
        int maxStone = 0;

        foreach (var item in BuildingList) {
            BuildingLevelConfig cfg = BuildingLevelConfigLoader.GetConfig(item.ConfigID, item.Level);
            int value = cfg.MaxStorage;
            switch (item.BuildingType) {
                case CityBuildingType.PALACE:
                    // 主城会同时增加最大金钱、最大木材、石材储量
                    maxMoney += value;
                    maxWood += value;
                    maxStone += value;
                    break;
                case CityBuildingType.MONEY_STORAGE:
                    maxMoney += value;
                    break;
                case CityBuildingType.WOOD_STORAGE:
                    maxWood += value;
                    break;
                case CityBuildingType.STONE_STORAGE:
                    maxStone += value;
                    break;
            }
        }

        UserManager.Instance.MaxMoneyStorage = maxMoney;
        UserManager.Instance.MaxWoodStorage = maxWood;
        UserManager.Instance.MaxStoneStorage = maxStone;

        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
    }

    // 请求升级士兵
    public void RequestTrainSoldier(int soldierID, int cost)
    {
        PCMInt data = new PCMInt();
        data.arg = soldierID;

        Net.Send(eCommand.TRAIN_SOLIDERS, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.TRAIN_SOLIDERS)) return;

            TrainBuildingInfo info = GetBuilding(ret.buildId) as TrainBuildingInfo;
            if (info != null) {
                UserManager.Instance.CostMoney(cost, PriceType.MONEY);
                info.Deserialize(ret);
                RefreshUI(info.EntityID);
            }
        });
    }

    // 请求切换士兵
    public void RequestSwitchSoldier(long buildingID, int soldierID, int cost)
    {
        PProductSolider data = new PProductSolider();
        data.buildId = buildingID;
        data.soliderCfgId = soldierID;

        Net.Send(eCommand.CHANGE_AND_PRODUCT_SOLIDERS, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.CHANGE_AND_PRODUCT_SOLIDERS)) return;

            TroopBuildingInfo info = GetBuilding(ret.buildId) as TroopBuildingInfo;
            if (info != null) {
                // 扣去相应资源
                UserManager.Instance.CostMoney(cost, PriceType.MONEY);
                info.Deserialize(ret);
                RefreshUI(buildingID);
                UIManager.Instance.RefreshWindow<UICitySoldierSwitchView>();
            }
        });
    }

    // 请求补充士兵
    public void RequestProduceSoldier(long buildingID, int soldierID, int cost)
    {
        PProductSolider data = new PProductSolider();
        data.buildId = buildingID;
        data.soliderCfgId = soldierID;
        
        Net.Send(eCommand.PRODUCT_SOLIDERS, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.PRODUCT_SOLIDERS)) return;

            TroopBuildingInfo info = GetBuilding(ret.buildId) as TroopBuildingInfo;
            if (info != null) {
                // 扣去相应资源
                UserManager.Instance.CostMoney(cost, PriceType.MONEY);
                info.Deserialize(ret);
                RefreshUI(buildingID);
                UIManager.Instance.CloseWindow<UICitySoldierSelectView>();
                UIManager.Instance.RefreshWindow<UICitySoldierSwitchView>();
            }
        });
    }

    // 请求取消升级建筑
    public void RequestCancelUpgradeBuilding(long buildingID)
    {
        BuildingInfo infoSend = GetBuilding(buildingID);
        if (infoSend == null) return;

        PCMLongInt data = new PCMLongInt();
        data.arg1 = buildingID;
        data.arg2 = infoSend.Cfg.BuildingType;

        Net.Send(eCommand.CANCLE_UPGRADE_BUILD, data, (buffer) => {
            CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
            if (!Net.CheckErrorCode(ret.err_code, eCommand.CANCLE_UPGRADE_BUILD)) return;

            // 返还资源
            UserManager.Instance.AddMoney(Mathf.FloorToInt(infoSend.CfgLevel.CostStone * GameConfig.CITY_BUILDING_CANCEL_BACK), PriceType.STONE);
            UserManager.Instance.AddMoney(Mathf.FloorToInt(infoSend.CfgLevel.CostWood * GameConfig.CITY_BUILDING_CANCEL_BACK), PriceType.WOOD);

            infoSend.LevelUpRemainTime.Reset();

            // TODO 返还资源由服务器推送
            RefreshUI(buildingID);
        });
    }
    
    // 请求取消升级士兵
    public void RequestCancelTrainSoldier(int soldierCfgID)
    {
        PCMInt data = new PCMInt();
        data.arg = soldierCfgID;

        Net.Send(eCommand.CANCLE_TRAIN_SOLIDER, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.CANCLE_TRAIN_SOLIDER)) return;

            TrainBuildingInfo tbinfo = GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
            if (tbinfo != null) {
                int cost = tbinfo.GetTrainCost(tbinfo.TrainSoldierCfgID);

                // 返还资源
                UserManager.Instance.AddMoney(Mathf.FloorToInt(cost * GameConfig.CITY_BUILDING_CANCEL_BACK), PriceType.MONEY);

                tbinfo.TrainSoldierCfgID = 0;
                tbinfo.TrainRemainTime = 0;
                tbinfo.TrainSyncTime = 0;
                
                RefreshUI(tbinfo.EntityID);
            }
        });
    }

    // 请求取消生产士兵
    public void RequestCancelProduceSoldier(long buildingID, int soldierID)
    {
        TroopBuildingInfo infoSend = GetBuilding(buildingID) as TroopBuildingInfo;
        if (infoSend == null) return;

        PCMLongInt data = new PCMLongInt();
        data.arg1 = buildingID;
        data.arg2 = soldierID;

        Net.Send(eCommand.CANCLE_PRODUCT_SOLIDER, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.CANCLE_PRODUCT_SOLIDER)) return;
            
            infoSend.Deserialize(ret);

            // 请求同步数据（因为士兵有生产过程，所以客户端很难去计算正确的结果）
            UserManager.Instance.RequestSyncRes();
            RefreshUI(buildingID);
        });
    }

    // 请求立即升级建筑
    public void RequestQuickUpgradeBuilding(long buildingID, bool costRes)
    {
        BuildingInfo infoSend = GetBuilding(buildingID);
        if (infoSend == null) return;

        PCMLongInt data = new PCMLongInt();
        data.arg1 = buildingID;
        data.arg2 = infoSend.Cfg.BuildingType;

        Net.Send(eCommand.UPGRADE_BUILD_RIGHT_NOW, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.UPGRADE_BUILD_RIGHT_NOW)) return;

            BuildingInfo info = GetBuilding(buildingID);
            if (info != null) {
                if (costRes) {
                    // 扣除资源(如果是在升级过程中点击快速升级，则不扣除资源)
                    UserManager.Instance.CostMoney(info.CfgLevel.CostStone, PriceType.STONE);
                    UserManager.Instance.CostMoney(info.CfgLevel.CostWood, PriceType.WOOD);
                }

                UserManager.Instance.CostMoney(info.GetQuickLevelUpCost(false), PriceType.GOLD);
                info.Deserialize(ret);
                info.OnLevelUpFinish();

                if (info.BuildingType == CityBuildingType.PALACE) {
                    // 如果是主城升级完毕，重新取建筑列表，可能会解锁建筑
                    RequestBuildingList();
                }
            }

            // 升级成功，更新建筑物的最大储量
            UpdateMaxStorage();
            RefreshUI(buildingID);
        });
    }

    // 请求快速升级士兵
    public void RequestQuickTrainSoldier(int soldierCfgID, bool costRes)
    {
        PCMInt data = new PCMInt();
        data.arg = soldierCfgID;

        Net.Send(eCommand.TRAIN_SOLIDER_RIGHT_NOW, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);

            if (!Net.CheckErrorCode(ret.errorCode, eCommand.TRAIN_SOLIDER_RIGHT_NOW)) return;

            TrainBuildingInfo tbinfo = GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
            if (tbinfo != null) {
                if (costRes) {
                    // 扣去相应资源(先减再升级)
                    UserManager.Instance.CostMoney(tbinfo.GetTrainCost(soldierCfgID), PriceType.MONEY);
                }

                // 无论哪种情况都要把黄金扣了
                UserManager.Instance.CostMoney(tbinfo.GetQuickTrainCost(), PriceType.GOLD);

                tbinfo.Deserialize(ret);
                RefreshUI(tbinfo.EntityID);

                UIManager.Instance.RefreshWindow<UICityTrainSelectView>();
            }
        });   
    }

    public void RequestQuickProduceSoldier(long buildingID, int soldierID)
    {
        TroopBuildingInfo infoSend = GetBuilding(buildingID) as TroopBuildingInfo;
        if (infoSend == null) return;

        PCMLongInt data = new PCMLongInt();
        data.arg1 = buildingID;
        data.arg2 = soldierID;

        Net.Send(eCommand.PRODUCT_SOLIDER_RIGHT_NOW, data, (buffer) => {
            PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.PRODUCT_SOLIDER_RIGHT_NOW)) return;
            
            UserManager.Instance.CostMoney(infoSend.GetQuickProducingCost(), PriceType.GOLD);

            infoSend.Deserialize(ret);
            RefreshUI(buildingID);

            UIManager.Instance.CloseWindow<UICitySoldierSelectView>();
            UIManager.Instance.CloseWindow<UICitySoldierSwitchView>();
        });
    }

    private void RefreshUI(long buildingID)
    {
        UIManager.Instance.RefreshWindow<UICityBuildingUplevelView>();
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_REFRESH, buildingID);
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
    }
    
    // 根据资源类型获取建筑名字
    public string GetBuildingNameByRes(ResourceType resType)
    {
        foreach (var item in BuildingConstConfigLoader.Data) {
            if (ResourceType.MONEY == resType && item.Value.BuildingType == (int)CityBuildingType.HOUSE) {
                return item.Value.BuildingName;
            } else if (ResourceType.WOOD == resType && item.Value.BuildingType == (int)CityBuildingType.WOOD) {
                return item.Value.BuildingName;
            } else if (ResourceType.STONE == resType && item.Value.BuildingType ==(int)CityBuildingType.STONE) {
                return item.Value.BuildingName;
            }
        }

        return "";
    }

    //根据资源类型获取当前产量
    public int GetProduceByteRes(ResourceType resType)
    {
        int ret = 0;
        foreach (var item in BuildingList) {
            if (resType == ResourceType.MONEY) {
                if (item.BuildingType == CityBuildingType.HOUSE) {
                    ret += item.CfgLevel.OutputNumber;
                }
            }
            else if (resType == ResourceType.WOOD) {
                if (item.BuildingType == CityBuildingType.WOOD) {
                    ret += item.CfgLevel.OutputNumber;
                }
            }
            else if (resType == ResourceType.STONE) {
                if (item.BuildingType == CityBuildingType.STONE) {
                    ret += item.CfgLevel.OutputNumber;
                }
            }
        }

        return ret;
    }

    public int GetFightScore()
    {
        int fightScore = 0;
        foreach (var item in BuildingList) {
            TroopBuildingInfo info = item as TroopBuildingInfo;
            if (info != null) {
                fightScore += info.GetFightScore();
            }
        }
        return fightScore;
    }

    // 获取铁匠铺的等级
    public int GetSmithyLevel()
    {
        foreach (var item in BuildingList) {
            if (item.BuildingType == CityBuildingType.SMITHY) {
                return item.Level;
            }    
        }
        return 0;
    }
}
