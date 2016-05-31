using UnityEngine;
using System.Collections;
using comrt.comnet;

// 校场
public class TrainBuildingInfo : BuildingInfo
{
    public float TrainRemainTime; // 升级兵种剩余时间
    public float TrainSyncTime;   // 升级兵种同步时间
    public int TrainSoldierCfgID;

    public override void Deserialize(PBuildInfo data)
    {
        base.Deserialize(data);

        TrainRemainTime = 0;
        TrainSyncTime = 0;
        TrainSoldierCfgID = 0;

        // 处理兵种等级数据
        CityManager.Instance.SoldierLevelList.Clear();
        foreach (var item in data.soliders.soliderList) {
            if (item.elpaseTime > 0) {
                // 如果有士兵正在升级
                TrainRemainTime = Utils.GetSeconds(item.elpaseTime);
                TrainSyncTime = Time.realtimeSinceStartup;
                TrainSoldierCfgID = item.soliderCfgId;
            }

            CityManager.Instance.SoldierLevelList[item.soliderCfgId] = item.level;
        }
    }

    // 是否正在升级兵种
    public bool IsTrainingSoldier()
    {
        return TrainRemainTime > 0 && TrainSyncTime > 0;
    }

    // 获取当前正在升级的兵种升级倒计时
    public int GetTrainCD()
    {
        if (IsTrainingSoldier()) {
            // 正在升级中
            return Mathf.Max(Mathf.FloorToInt(TrainRemainTime - (Time.realtimeSinceStartup - TrainSyncTime)), 0);
        } else {
            return GetMaxTrainTime();
        }
    }

    // 获取升级消耗
    public int GetTrainCost(int soldierCfgID)
    {
        int level = CityManager.Instance.GetSoldierLevel(soldierCfgID);
        SoldierLevelConfig cfgLevup = SoldierLevelConfigLoader.GetConfig(soldierCfgID, level);
        return cfgLevup.UpgradeCost;
    }

    public void OnTrainFinish(int newLevel)
    {
        CityManager.Instance.SoldierLevelList[TrainSoldierCfgID] = newLevel;
        SoldierConfig cfg = SoldierConfigLoader.GetConfig(TrainSoldierCfgID);

        // 消息提示
        if (cfg != null) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_FINISH", cfg.SoldierName, newLevel);
        }

        TrainRemainTime = 0;
        TrainSyncTime = 0;
        TrainSoldierCfgID = 0;

        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_REFRESH, EntityID);
        UIManager.Instance.RefreshWindow<UICityTrainSelectView>();
    }

    // 获取当前升级的最大时间
    public int GetMaxTrainTime()
    {
        if (TrainSoldierCfgID == 0) return 0;

        int level = CityManager.Instance.GetSoldierLevel(TrainSoldierCfgID);
        SoldierLevelConfig cfg = SoldierLevelConfigLoader.GetConfig(TrainSoldierCfgID, level);
        return Utils.GetSeconds(cfg.UpgradeTime);
    }

    // 获取快速升级兵种所需要的消耗
    public int GetQuickTrainCost()
    {
        int time = GetTrainCD();
        return Formula.GetLevelUpQuickCost(time);
    }
}
