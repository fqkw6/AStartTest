using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

// 竞技场对手数据
public class PVPPlayerInfo
{
    public long EntityID;
    public string Name;
    public int Level;
    public int Rank;
    public int Icon;
    public int FightScore;
    public string GuildName;
    public List<HeroInfo> HeroList = new List<HeroInfo>();
    public List<SoldierInfo> SoldierList = new List<SoldierInfo>();

    public void Deserialize(PAthletics data)
    {
        EntityID = data.playerId;
        Name = data.playerName;
        Level = data.level;
        Icon = data.headImage;
        FightScore = data.totalFighting;
        Rank = data.ranking;
        GuildName = data.guildName;
    }

    public void Deserialize(PPlayerInfo data)
    {
        if (data.pBattlePT.Count < 2) {
            Log.Error("Error Battle Formation");
            return;
        }

        // pvp的阵型数据
        PBattlePT formation = data.pBattlePT[1];

        // 英雄
        HeroList.Clear();
        foreach (var item in formation.heroPT) {
            HeroInfo info = new HeroInfo();
            info.Deserialize(item.hero, false);
            HeroList.Add(info);
        }

        SoldierList.Clear();
        foreach (var item in formation.buildPT) {
            BuildingInfo binfo = CityManager.Instance.CreateBuilding(item.build.cfgId);
            if (binfo == null) {
                continue;
            }

            TroopBuildingInfo tbinfo = binfo as TroopBuildingInfo;
            if (tbinfo != null) {
                // TODO 暂时只处理兵营数据，将来再处理校场，校场含有士兵等级数据
                binfo.Deserialize(item.build);

                SoldierInfo info = new SoldierInfo();
                info.ConfigID = tbinfo.SoldierConfigID;
                SoldierList.Add(info);
            }


            if (binfo.BuildingType == CityBuildingType.TRAIN) {
                // 校场，持有士兵等级数据
            } else if (binfo.BuildingType == CityBuildingType.TROOP) {
                // 兵营
            }
        }
    }
}

// 竞技场排行榜数据
public class PVPRankInfo
{
    public long EntityID;
    public int Rank;
    public int Level;
    public int Icon;
    public string Name;
    public string GuildName;
    public int FightScore;

    public void Deserialize(PAthletics data)
    {
        EntityID = data.playerId;
        Name = data.playerName;
        Level = data.level;
        Icon = data.headImage;
        FightScore = data.totalFighting;
        Rank = data.ranking;
        GuildName = "";
    }
}

// 竞技场战报数据
public class PVPReportInfo
{
    public bool Win;    // 胜利还是失败
    public int Number;  // 前进名次，如果失败就是下降名次
    public int Icon;
    public int Level;
    public int FightScore;
    public string Name;
    public ElapseTime BattleTime;// 攻击的时间
    public string ReportFile;   // 战报路径

    public void Deserialize(PAthleticsLog data)
    {
        Name = data.roleName;
        ReportFile = data.btVideoFile;
        BattleTime.SetTimeMilliseconds(data.btTime);
        Win = (data.btResult == eBattleResult.BTR_WIN || data.btResult == eBattleResult.BTR_NARROW_VICTORY || data.btResult == eBattleResult.BTR_PERFECT_VICTORY);
        Number = data.rankChange;
        FightScore = data.fighting;
        Icon = data.headImage;
        Level = data.level;
    }
}

