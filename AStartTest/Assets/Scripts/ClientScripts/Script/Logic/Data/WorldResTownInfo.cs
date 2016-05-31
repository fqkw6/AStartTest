using UnityEngine;
using System.Collections;
using comrt.comnet;

// 大地图资源城数据
public class WorldResTownInfo : WorldCityInfo
{
    // 资源城的产量（可能有玩家天赋加成）
    public int ProduceValue;
    public ResourceType ProduceType;
    public int MapLevel;
    public int MapConfigID;
    
    // 上次选择收获的经过时间
    public float ProduceRewardElapseTime;
    public float ProduceRewardSyncTime;

    // 占领的剩余时间
    public float ConquerRemainTime;
    public float ConquerSyncTime;

    public override void Deserialize(PResourceMapInfo data)
    {
        base.Deserialize(data);
        
        MapConfigID = data.cfgid;
        MapPosition = data.mapPos;

		if (data.simRoleAttr == null) {
			UserEntityID = 0;
			UserName = "";
			UserLevel = 0;
			UserPalaceLevel = 0;
			UserIcon = 0;
			UserFightScore = -1;
		} else {
			UserEntityID = data.simRoleAttr.playerId;
			UserName = data.simRoleAttr.roleName;
			UserLevel = data.simRoleAttr.level;
			UserPalaceLevel = data.simRoleAttr.mainBuildLevel;
			UserIcon = data.simRoleAttr.headImage;
			UserFightScore = data.simRoleAttr.fighting;
		}
		
        MapLevel = data.mapLevel;
        
        RefreshRemainTime.SetTimeMilliseconds(data.refreshLeftTime);

		ConquerRemainTime = Utils.GetSeconds(data.occupyLeftTime);
        ConquerSyncTime = Time.realtimeSinceStartup;

		ProduceRewardElapseTime = Utils.GetSeconds(data.elapseTime);
        ProduceRewardSyncTime = Time.realtimeSinceStartup;

        ProduceValue = data.perHourGain;

        switch (data.sourceType) {
            case eSourceMapType.GOLD_OUT:
                ProduceType = ResourceType.MONEY;
                break;
            case eSourceMapType.STONE_OUT:
                ProduceType = ResourceType.STONE;
                break;
            case eSourceMapType.WOOD_OUT:
                ProduceType = ResourceType.WOOD;
                break;
            case eSourceMapType.YUANBAO_OUT:
                ProduceType = ResourceType.GOLD;
                break;
        }

		foreach (var item in data.heroAttrs) {
			WorldCityHeroInfo info = new WorldCityHeroInfo();
			info.heroCfgID = item.heroCfgId;
			info.heroLevel = item.level;
			info.heroQuality = item.jinjie;
			info.heroStar = item.heroStar;
			HeroInfoList.Add(info);
		}

    }

    // 获取占领剩余时间
    public float GetConquerCD()
    {
        return ConquerRemainTime - (Time.realtimeSinceStartup - ConquerSyncTime);
    }

    // 获取总的产量
    public int GetTotalProduceValue()
    {
        float elapse = GameConfig.WORLD_RES_TOWN_CONQUER_TIME - ConquerRemainTime;

        // 10分钟的产量
        float speedValue = ProduceValue / (3600f / GameConfig.PRODUCE_REWARD_INTERVAL);

        // 有多少个10分钟
        int countValue = Mathf.FloorToInt((elapse + Time.realtimeSinceStartup - ConquerSyncTime) / GameConfig.PRODUCE_REWARD_INTERVAL);
        int value = Mathf.FloorToInt(countValue * speedValue);

        return value;
    }

    public void OnCollectResource(int value)
    {
        switch (ProduceType) {
            case ResourceType.MONEY:
                UserManager.Instance.Money += value;
                break;
            case ResourceType.WOOD:
                UserManager.Instance.Wood += value;
                break;
            case ResourceType.STONE:
                UserManager.Instance.Stone += value;
                break;
            case ResourceType.GOLD:
                UserManager.Instance.Gold += value;
                break;
        }

        ProduceRewardElapseTime = 0;
        ProduceRewardSyncTime = Time.realtimeSinceStartup;
    }
}
