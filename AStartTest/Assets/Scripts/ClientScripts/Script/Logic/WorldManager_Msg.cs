using UnityEngine;
using System.Collections;
using comrt.comnet;

public partial class WorldManager
{
    public void RegisterMsg()
    {
        Net.Register(eCommand.COLLECT_RESOURCE, OnMsgCollectCityResource);
        Net.Register(eCommand.GET_PLYAYER_WORLD_MAP, OnMsgGetPlayerWorldMap);
    }

    private void OnMsgCollectCityResource(byte[] buffer)
    {
        
    }

    private void OnMsgGetPlayerWorldMap(byte[] buffer)
    {
        PWorldMapInfo ret = Net.Deserialize<PWorldMapInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_PLYAYER_WORLD_MAP)) return;

        WorldManager.Instance.CityList.Clear();

        //Log.Info("城池数据: {0}  {1}", ret.playerMaps.Count, ret.sourceMaps.Count);

        foreach (var item in ret.playerMaps) {
            WorldCityInfo info = new WorldCityInfo();
            info.Deserialize(item);
            WorldManager.Instance.CityList.Add(info);
        }

        // 刷新地图
        UIManager.Instance.RefreshWindow<UIWorldMapView>();
    }
}