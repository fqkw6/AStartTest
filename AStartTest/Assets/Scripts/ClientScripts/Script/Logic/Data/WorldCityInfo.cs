using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

// 大地图玩家城池数据
public class WorldCityInfo
{
    public class WorldCityHeroInfo
    {
        public int heroCfgID;
        public int heroLevel;
        public int heroQuality;
        public int heroStar;
        public int heroFightScore;
    }
    
    public long UserEntityID;
    public int MapPosition;

    // 玩家信息
    public string UserName;     // 玩家姓名
    public int UserIcon;
    public int UserLevel;       // 玩家等级
    public int UserFightScore;  // 玩家战斗力
    public int UserPalaceLevel;

    //  玩家可获得的资源
    public int RewardMoney;
    public int RewardWood;
    public int RewardStone;
    public int RewardGold;

    // 英雄列表
    public List<WorldCityHeroInfo> HeroInfoList = new List<WorldCityHeroInfo>();

    // 刷新剩余倒计时
    public RemainTime RefreshRemainTime;

    // 是否已经被打败
    public bool IsConquered;
    public bool IsNpc = false;

    public virtual void Deserialize(PPlayerMapInfo data)
    {
        MapPosition = data.mapPos;

        if (data.mapType == eWorldMapType.PLAYER_CITY_MAP) {
            IsNpc = false;
            if (data.simRoleAttr != null) {
                UserEntityID = data.simRoleAttr.playerId;
                UserName = data.simRoleAttr.roleName;
                UserIcon = data.simRoleAttr.headImage;

                UserLevel = data.simRoleAttr.level;
                UserPalaceLevel = data.simRoleAttr.mainBuildLevel;
                UserFightScore = data.simRoleAttr.fighting;
            }
        } else if (data.mapType == eWorldMapType.NPC_CITY_MAP) {
            IsNpc = true;
            if (data.simRoleAttr != null) {
                UserEntityID = data.simRoleAttr.playerId;

                WorldMapConfig cfg = WorldMapConfigLoader.GetConfig((int)UserEntityID);

                UserName = cfg.CityName;
                UserIcon = data.simRoleAttr.headImage;

                if (data.getAssertInfo != null) {
                    // 已经侦查
                    UserLevel = cfg.PlayerLevel;
                    UserPalaceLevel = cfg.CityLevel;
                    UserFightScore = cfg.BattlePower;
                    HeroInfoList.Clear();
                    AddNpcHero(cfg.DefenseHero1);
                    AddNpcHero(cfg.DefenseHero2);
                    AddNpcHero(cfg.DefenseHero3);
                } else {
                    // 尚未侦查
                    UserLevel = 0;
                    UserPalaceLevel = 0;
                    UserFightScore = -1;
                }
            }
        }

        if (data.getAssertInfo != null) {
            RewardMoney = data.getAssertInfo.gold;
            RewardWood = data.getAssertInfo.wood;
            RewardStone = data.getAssertInfo.stone;
            RewardGold = data.getAssertInfo.yuanbao;
        } else {
            // 尚未侦查
            RewardMoney = -1;
            RewardWood = -1;
            RewardStone = -1;
            RewardGold = -1;
        }

        if (data.heroInfos != null) {
            foreach (var item in data.heroInfos) {
                WorldCityHeroInfo info = new WorldCityHeroInfo();
                info.heroCfgID = item.heroCfgId;
                info.heroLevel = item.level;
                info.heroQuality = item.jinjie;
                info.heroStar = item.heroStar;
                info.heroFightScore = item.fighting;
                HeroInfoList.Add(info);
            }
        }

        RefreshRemainTime.SetTimeMilliseconds(data.refreshLeftTime);
    }

    private void AddNpcHero(int cfgID)
    {
        WorldCityHeroInfo info = new WorldCityHeroInfo();
        info.heroCfgID = cfgID;
        info.heroLevel = 1;
        info.heroQuality = 1;
        info.heroStar = 1;
        info.heroFightScore = 0;
        HeroInfoList.Add(info);
    }

    public virtual void Deserialize(PResourceMapInfo data)
    {
        
    }

    // 是否是我的城池
    public bool IsMyCity()
    {
        return UserEntityID == UserManager.Instance.EntityID;
    }

    // 是否可以手动刷新
    public bool CouldRefresh()
    {
        return RefreshRemainTime.GetTime() <= 0;
    }
}
