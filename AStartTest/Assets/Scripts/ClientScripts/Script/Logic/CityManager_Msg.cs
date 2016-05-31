using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;


public partial class CityManager
{
    public void RegisterMsg()
    {
        Net.Register(eCommand.GET_BUILD_INFO, OnMsgGetBuildInfo);
        Net.Register(eCommand.PUSH_BUILD_STATUS, OnMsgPushBuildStatus);
        Net.Register(eCommand.PUSH_GET_BUILD, OnMsgPushGetBuild);
        Net.Register(eCommand.PUSH_GET_SOLIDER, OnMsgPushGetSoldier);
        Net.Register(eCommand.PUSH_TRAIN_SOLIDER_FINISH, OnMsgPushTrainSoldierFinish);
        Net.Register(eCommand.PUSH_UNLOCK_SOLIDER, OnMsgPushUnlockNewSoldiers);
    }

    private void OnMsgGetBuildInfo(byte[] buffer)
    {
        PBuildList ret = Net.Deserialize<PBuildList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_BUILD_INFO)) return;
        
        foreach (var item in ret.buildList) {
            BuildingInfo info = GetBuilding(item.buildId);
            if (info == null) {
                // 新增建筑
                info = CreateBuilding(item.cfgId);
                BuildingList.Add(info);
            }

            info.Deserialize(item);
        }

        // 更新最大储量
        UpdateMaxStorage();

        // 刷新地图
        UIManager.Instance.RefreshWindow<UICityView>();
    }

    private void OnMsgPushBuildStatus(byte[] buffer)
    {
        PBuildInfo ret = Net.Deserialize<PBuildInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.PUSH_BUILD_STATUS)) return;

        BuildingInfo info = GetBuildingByConfigID(ret.cfgId);
        if (info == null) return;

        info.Deserialize(ret);
        info.OnLevelUpFinish();

        if (info.BuildingType == CityBuildingType.PALACE) {
            // 如果是主城升级完毕，重新取建筑列表，可能会解锁建筑
            RequestBuildingList();
        }
    }

    private void OnMsgPushGetBuild(byte[] buffer)
    {
        PBuildList ret = Net.Deserialize<PBuildList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.PUSH_GET_BUILD)) return;

        foreach (var item in ret.buildList) {
            BuildingInfo info = CreateBuilding(item.cfgId);
            info.Deserialize(item);
            BuildingList.Add(info);
        }

        // 刷新地图
        UIManager.Instance.RefreshWindow<UICityView>();
    }

    private void OnMsgPushGetSoldier(byte[] buffer)
    {
        PPushSolilerGet ret = Net.Deserialize<PPushSolilerGet>(buffer);


        TroopBuildingInfo info = GetBuilding(ret.buildId) as TroopBuildingInfo;
        if (info == null) return;

        // 添加士兵数量
        info.SoldierCount += ret.addNum;
        Log.Info("添加新士兵" + info.SoldierCount);

        // 全部生产完毕
        if (ret.isFinished) {
            info.OnProduceFinish();
        }

        // 刷新对应建筑
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_REFRESH, ret.buildId);
    }

    private void OnMsgPushTrainSoldierFinish(byte[] buffer)
    {
        PSolider data = Net.Deserialize<PSolider>(buffer);

        TrainBuildingInfo tbinfo = GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;

        if (tbinfo != null) {
            // 刷新对应建筑
            Log.Info("兵种升级完毕" + data.level);
            tbinfo.OnTrainFinish(data.level);
        }
    }

    private void OnMsgPushUnlockNewSoldiers(byte[] buffer)
    {
        PSoliderInfo data = Net.Deserialize<PSoliderInfo>(buffer);

        Log.Info("解锁新兵种");

        TrainBuildingInfo info = GetBuildingByType(CityBuildingType.TRAIN) as TrainBuildingInfo;
        if (info == null) return;

        foreach (var item in data.soliderList) {
            SoldierLevelList[item.soliderCfgId] = item.level;
        }

        // 刷新对应建筑
        EventDispatcher.TriggerEvent(EventID.EVENT_CITY_BUILDING_REFRESH, info.EntityID);

        UIManager.Instance.RefreshWindow<UICityTrainSelectView>();
    }
}