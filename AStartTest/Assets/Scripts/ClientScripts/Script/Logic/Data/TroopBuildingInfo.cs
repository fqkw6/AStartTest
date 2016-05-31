using UnityEngine;
using System.Collections;
using comrt.comnet;

// 兵营
public class TroopBuildingInfo : BuildingInfo
{
    // 兵营
    public float SoldierProduceRemainTime;       // 训练剩余时间
    public float SoldierProduceSyncTime;         // 同步消息时的时间
    public int SoldierCount;            // 当前士兵数目
    public int SoldierConfigID;         // 哪个士兵
    public int SoldierTotalCount;       // 当前生产总共生产几个士兵

    private SoldierConfig _soldierConfig = null;

    public SoldierConfig SoldierCfg
    {
        get
        {
            _soldierConfig = SoldierConfigLoader.GetConfig(SoldierConfigID);
            return _soldierConfig;
        }
    }

    public override void Deserialize(PBuildInfo data)
    {
        base.Deserialize(data);

        // 兵营只有一个士兵数据
        if (data.soliders.soliderList.Count > 0) {
            PSolider sdata = data.soliders.soliderList[0];
            SoldierConfigID = sdata.soliderCfgId;
            SoldierCount = sdata.curNum;
            SoldierProduceRemainTime = Utils.GetSeconds(sdata.elpaseTime);
            SoldierTotalCount = sdata.trainNum;

            if (sdata.elpaseTime > 0) {
                SoldierProduceSyncTime = Time.realtimeSinceStartup;
            } else {
                SoldierProduceSyncTime = 0;
            }
        } else {
            SoldierConfigID = 0;
            SoldierCount = 0;
            SoldierProduceRemainTime = 0;
            SoldierProduceSyncTime = 0;
            SoldierTotalCount = 0;
        }
    }

    public void OnProduceFinish()
    {
        SoldierProduceRemainTime = 0;
        SoldierProduceSyncTime = 0;

        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_MENU_CLOSE);
    }

    // 是否正在生产士兵
    public bool IsProducingSoldier()
    {
        return SoldierProduceRemainTime > 0 && SoldierProduceSyncTime > 0;
    }

    // 获取当前兵营兵种的最大数量
    public int GetMaxSoldierCount(int soldierCfgID)
    {
        SoldierConfig cfg = SoldierConfigLoader.GetConfig(soldierCfgID);
        if (cfg != null) {
            return CfgLevel.MaxStorage / cfg.States;
        }

        return 0;
    }

    // 获取当前可生产的最大数目，考虑当前数目
    public int GetTotalProduceCount(int soldierCfgID)
    {
        if (IsProducingSoldier()) {
            // 如果正在生产，则以服务器记录的生产数目为准
            return SoldierTotalCount;
        } else {
            return GetMaxSoldierCount(soldierCfgID) - SoldierCount;
        }
    }

    // 获取正在生产中的士兵的倒计时
    public int GetProducingCD()
    {
        if (IsProducingSoldier()) {
            // 正在生产士兵
            return Mathf.Max(Mathf.FloorToInt(SoldierProduceRemainTime - (Time.realtimeSinceStartup - SoldierProduceSyncTime)), 0);
        } else {
            return GetMaxProduceTime();
        }
    }

    // 获取补充士兵的消耗
    public int GetAddCost(int soldierID)
    {
        int level = CityManager.Instance.GetSoldierLevel(soldierID);
        SoldierLevelConfig cfgLevup = SoldierLevelConfigLoader.GetConfig(soldierID, level);
        int count = GetTotalProduceCount(soldierID);
        return cfgLevup.ProduceCost*count;
    }

    // 获取更换士兵的消耗
    public int GetSwitchCost(int soldierID)
    {
        int oldMoney = 0;
        if (SoldierConfigID != 0) {
            int levelOld = CityManager.Instance.GetSoldierLevel(SoldierConfigID);
            SoldierLevelConfig cfgLevupOld = SoldierLevelConfigLoader.GetConfig(SoldierConfigID, levelOld);
            oldMoney = cfgLevupOld.ProduceCost * SoldierCount;
        }
        int level = CityManager.Instance.GetSoldierLevel(soldierID);
        SoldierLevelConfig cfgLevup = SoldierLevelConfigLoader.GetConfig(soldierID, level);
        int count = GetMaxSoldierCount(soldierID);
        return cfgLevup.ProduceCost * count - oldMoney;
    }

    // 获取生产士兵的最大消耗
    public int GetMaxProduceTime()
    {
        return Utils.GetSeconds(SoldierCfg.Producetime)*SoldierTotalCount;
    }

    // 获取快速生产士兵的钻石消耗
    public int GetQuickProducingCost()
    {
        int time = GetProducingCD();
        return Formula.GetLevelUpQuickCost(time);
    }

    // 获取战斗力
    public int GetFightScore()
    {
        if (SoldierConfigID == 0) {
            return 0;
        }

        int maxCount = GetMaxSoldierCount(SoldierConfigID);
        int level = CityManager.Instance.GetSoldierLevel(SoldierConfigID);
        SoldierLevelConfig cfg = SoldierLevelConfigLoader.GetConfig(SoldierConfigID, level);
        if (cfg == null) {
            return 0;
        }

        return maxCount*Mathf.FloorToInt(cfg.SoldierAttack + 0.2f*cfg.SoldierHp);
    }
}
