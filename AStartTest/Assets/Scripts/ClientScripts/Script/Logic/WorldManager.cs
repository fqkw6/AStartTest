using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

public partial class EventID
{
    public const string EVENT_WORLD_ADD_CITY = "EVENT_WORLD_ADD_CITY";                    // 添加一个新的城池
    public const string EVENT_WORLD_REMOVE_CITY = "EVENT_WORLD_REMOVE_CITY";                 // 移除一个城池
    public const string EVENT_WORLD_SWITCH_CITY = "EVENT_WORLD_SWITCH_CITY";                 // 更换一个城池
    public const string EVENT_WORLD_REFRESH_CITY = "EVENT_WORLD_REFRESH_CITY";                // 刷新城池数据
}

public class WorldBattleResultInfo
{
    // 战功牌
    public int Token = 0;

    // 资源
    public int Money = 0;
    public int Wood = 0;
    public int Stone = 0;
    public int Gold = 0;
}

// 大地图管理
public partial class WorldManager
{
    public static readonly WorldManager Instance = new WorldManager();

    // 城池列表(包含主城和资源城，资源城为WorldResTownInfo)
    public List<WorldCityInfo> CityList = new List<WorldCityInfo>();

    public WorldBattleResultInfo BattleResult;

    public WorldCityInfo CreateCity(int cfgID)
    {
        return null;
    }

    public WorldCityInfo GetCity(int mapPos)
    {
        return CityList.Find(x => x.MapPosition == mapPos);
    }

    public int GetTotalProduce(ResourceType resType)
    {
        int value = 0;
        foreach (var item in CityList) {
            WorldResTownInfo info = item as WorldResTownInfo;
            if (info != null && info.IsMyCity() && info.ProduceType == resType) {
                value += info.ProduceValue;
            }
        }
        return value;
    }

    public void OnBattleResult(PBattleReport data)
    {
        bool isAttacker = UserManager.Instance.EntityID == data.attackerId;
        if (isAttacker) {
            if (data.winner == eBattleSide.SIDE_DEFENSER) {
                // 输了
                UIManager.Instance.OpenWindow<UIWorldBattleResultFailView>();
                return;
            }
        } else {
            if (data.winner == eBattleSide.SIDE_ATTACKER) {
                // 输了
                UIManager.Instance.OpenWindow<UIWorldBattleResultFailView>();
                return;
            }
        }

        // 赢了
        
        // 战斗结果
        BattleResult = new WorldBattleResultInfo();
        BattleResult.Token = data.reserve1;
        BattleResult.Money = data.awdGold;
        BattleResult.Wood = data.awdWood;
        BattleResult.Stone = data.awdStone;
        BattleResult.Gold = data.awdYuanbao;

        UserManager.Instance.Money += BattleResult.Money;
        UserManager.Instance.Wood += BattleResult.Wood;
        UserManager.Instance.Stone += BattleResult.Stone;
        UserManager.Instance.Gold += BattleResult.Gold;

        UIManager.Instance.OpenWindow<UIWorldBattleResultView>(BattleResult);
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
    }

    // 请求大地图数据
    public void RequestWorldInfo()
    {
        Net.Send(eCommand.GET_PLYAYER_WORLD_MAP);
    }

    // 请求攻打城池
    public void RequestAttack(int mapPos, long playerID, bool isNpc, int cost)
    {
        PAttack data = new PAttack();
        data.pos = new PVector();
        data.pos.x = mapPos;
        data.pos.y = (int)(isNpc ? eWorldMapType.NPC_CITY_MAP : eWorldMapType.PLAYER_CITY_MAP);
        data.defenserId = playerID;

        NetworkManager.Instance.Send(eCommand.READY_BT_OCCUPY_CITY, data, (buffer) => {
            PBattlVerify ret = Net.Deserialize<PBattlVerify>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.READY_BT_OCCUPY_CITY)) return;
            
            BattleManager.Instance.StartBattle(ret.battleId, LogicBattleType.WORLD);

            UserManager.Instance.CostMoney(cost, PriceType.MONEY);
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);

            // 刷新地图
            // UIManager.Instance.RefreshWindow<UIWorldView>();
        });
    }

    // 请求更换城池
    public void RequestSwitch(int mapPos)
    {
        PCMInt data = new PCMInt();
        data.arg = mapPos;
        NetworkManager.Instance.Send(eCommand.REFRESH_CITY, data, (buffer) =>
        {
            PPlayerMapInfo ret = Net.Deserialize<PPlayerMapInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.REFRESH_CITY)) return;

            WorldCityInfo info = GetCity(mapPos);
            if (info != null) {
                info.Deserialize(ret);   
            }

            // 刷新地图
            //UIManager.Instance.RefreshWindow("World/UIWorldView");
            EventDispatcher.TriggerEvent(EventID.EVENT_WORLD_REFRESH_CITY, mapPos);
        });
    }

    // 请求侦查城池
    public void RequestDetect(int mapPos)
    {
        PCMInt data = new PCMInt();
        data.arg = mapPos;
        NetworkManager.Instance.Send(eCommand.SURVEY_CITY, data, (buffer) => {
            PPlayerMapInfo ret = Net.Deserialize<PPlayerMapInfo>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.SURVEY_CITY)) return;

            WorldCityInfo info = GetCity(mapPos);
            if (info != null) {
                info.Deserialize(ret);
            }

            // 刷新地图
            EventDispatcher.TriggerEvent(EventID.EVENT_WORLD_REFRESH_CITY, mapPos);
            UIManager.Instance.RefreshWindow<UIWorldCityInfoView>();
        });
    }

    // 请求收集资源岛的资源
    public void RequestCollectResource()
    {
    }

    // 获取所有资源城的对应产量
    public int GetProduceByteRes(ResourceType resType)
    {
        int ret = 0;
        foreach (var item in CityList) {
            WorldResTownInfo tinfo = item as WorldResTownInfo;
            if (tinfo == null) {
                continue;
            }
            if (ResourceType.MONEY == resType && tinfo.ProduceType == resType) {
                ret += tinfo.ProduceValue;
            } else if (ResourceType.WOOD == resType && tinfo.ProduceType == resType) {
                ret += tinfo.ProduceValue;
            } else if (ResourceType.STONE == resType && tinfo.ProduceType == resType) {
                ret += tinfo.ProduceValue;
            }
        }

        return ret;
    }

}
