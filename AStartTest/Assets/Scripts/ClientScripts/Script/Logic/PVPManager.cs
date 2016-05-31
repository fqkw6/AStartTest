using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

public class PVPBattleResult
{
    // 变化的排名和积分
    public int AddRank = 0;
    public int AddScore = 0;
}

// pvp的管理
public partial class PVPManager
{
    public static PVPManager Instance = new PVPManager();

    public List<PVPPlayerInfo> PlayerList = new List<PVPPlayerInfo>();
    public int AttackCount; // 剩余挑战次数
    public int TotalAttackCount;    // 总挑战次数
    public RemainTime AttackRemainTime; // 攻击cd时间
    public int ResetCount;  // 重置次数，影响重置消耗
    public int ChangeCount; // 换一批次数，影响换一批消耗

    public int MyRank;  // 我的排名
    public int MyHighRank;    // 我的历史最高排名
    public int MyScore; // 我今天的积分
    public List<HeroInfo> MyHeroList = new List<HeroInfo>();    // 我的上阵英雄
    public List<SoldierInfo> MySoldierList = new List<SoldierInfo>();   // 我的上阵士兵

    public Dictionary<int, bool> MyScoreAwardList = new Dictionary<int, bool>();  // 我的积分奖励的领取情况
    public Dictionary<int, bool> MyHighAwardList = new Dictionary<int, bool>();   // 我的最高奖励的领取情况 
    public List<PVPReportInfo> ReportList = new List<PVPReportInfo>();  // 我的战报数据
    public List<PVPRankInfo> RankList = new List<PVPRankInfo>();    // 排行榜数据  

    public PVPBattleResult BattleResult;

    private bool _hasRequestInfo = false;
    private ElapseTime _lastRequestTime = new ElapseTime();
    
    // 请求玩家自己的pvp相关数据
    public void RequestPVPInfo()
    {
        // 如果短时间内有请求过，那么不重复请求
        if (_hasRequestInfo && _lastRequestTime.IsValid() && _lastRequestTime.GetTime() <= 30*60) {
            return;
        }

        // 自己的挑战次数  对手信息
        Net.Send(eCommand.ATHTECLIC_LIST);
        // 积分奖励数据
        Net.Send(eCommand.ATHTECLIC_SCORE_AWARD_INFO);
        // 最高排名奖励领取情况
        Net.Send(eCommand.ATHTECLIC_RANK_AWARD_INFO);

        _lastRequestTime.SetTime(Time.realtimeSinceStartup);
        _hasRequestInfo = true;
    }

    // 请求排行榜数据
    public void RequestRankInfo()
    {
        Net.Send(eCommand.ATHTECLIC_RANK_LIST);
    }

    // 请求战报数据
    public void RequestReportInfo()
    {
        Net.Send(eCommand.ATHTECLIC_ENERY_LIST);
    }

    // 请求换一批对手
    public void RequestChangePlayer()
    {
        Net.Send(eCommand.ATHTECLIC_LIST);
    }

    // 请求攻击
    public void RequestAttack(long playerID)
    {
        PAttack data = new PAttack();
        data.defenserId = playerID;
        Net.Send(eCommand.READY_ATHTECLIC_CHALLENGE, data);
    }

    // 请求调整阵型
    public void RequestModify()
    {
        
    }

    // 请求购买攻击次数
    public void RequestBuyAttackChance(int count)
    {
        if (count <= 0) return;

        if (UserManager.Instance.Gold < count * GameConfig.PVP_GOLD_PER_ATTACK_COUNT) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_GOLD_LIMIT");
            return;
        }
        
        PChance data = new PChance();
        data.num = count;
        data.priceType = ePriceType.DIAMOND;
        data.chance = eChance.CHAN_ATHLETICS;

        NetworkManager.Instance.Send(eCommand.BUY_CHANCE, data, (buffer) =>
        {
            PChance ret = Net.Deserialize<PChance>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.BUY_CHANCE)) return;

            switch (ret.chance) {
                case eChance.CHAN_ATHLETICS:
                    PVPManager.Instance.AttackCount += ret.num;
                    UserManager.Instance.CostMoney(GameConfig.PVP_GOLD_PER_ATTACK_COUNT*ret.num, PriceType.GOLD);
                    UIManager.Instance.RefreshWindow<UIPVPView>();
                    break;
            }

            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });
    }

    // 请求领取积分奖励
    public void RequestGetScoreAward(int scoreID)
    {
        PCMInt data = new PCMInt();
        data.arg = scoreID;

        NetworkManager.Instance.Send(eCommand.ATHTECLIC_SCORE_AWARD_OBTAIN, data, (buffer) =>
        {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.ATHTECLIC_SCORE_AWARD_OBTAIN)) return;

            MyScoreAwardList[scoreID] = true;
            UserManager.Instance.AddItemWithUI(ret);

            UIManager.Instance.RefreshWindow<UIPVPScoreView>();
        });
    }

    // 请求领取最高排名奖励
    public void RequestGetHighAward(int highID)
    {
        PCMInt data = new PCMInt();
        data.arg = highID;

        NetworkManager.Instance.Send(eCommand.ATHTECLIC_RANK_AWARD_OBTAIN, data, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.ATHTECLIC_RANK_AWARD_OBTAIN)) return;

            MyHighAwardList[highID] = true;
            UserManager.Instance.AddItemWithUI(ret);
            UIManager.Instance.RefreshWindow<UIPVPAwardView>();
        });
    }

    public PVPPlayerInfo GetPlayer(long playerID)
    {
        return PlayerList.Find(x => x.EntityID == playerID);
    }

    // 根据竞技场排名排序
    public void SortPlayer()
    {
        PlayerList.Sort((a, b) => a.Rank.CompareTo(b.Rank));    
    }

    public void OnBattleResult(PBattleReport data)
    {
        // 减少攻击次数
        --AttackCount;

        int myRank = 0;
        int optRank = 0;

        // 输了的话排名不变
        bool isWin = true;
        bool isAttacker = UserManager.Instance.EntityID == data.attackerId;
        if (isAttacker) {
            // 我是进攻方，记录排名情况
            myRank = data.pos.x;
            optRank = data.pos.y;
            if (data.winner == eBattleSide.SIDE_DEFENSER) {
                isWin = false;
                // 输了
                UIManager.Instance.OpenWindow<UIPVPBattleResultFailView>();
            }
        } else {
            // 我是防守方，记录排名情况
            myRank = data.pos.y;
            optRank = data.pos.x;
            if (data.winner == eBattleSide.SIDE_ATTACKER) {
                isWin = false;
                // 输了
                UIManager.Instance.OpenWindow<UIPVPBattleResultFailView>();
            }
        }

        if (!isWin) {
            MyScore += GameConfig.PVP_SCORE_LOSE_ADD;
            return;
        }

        // 战斗结果
        MyScore += GameConfig.PVP_SCORE_WIN_ADD;

        BattleResult = new PVPBattleResult();
        BattleResult.AddScore = GameConfig.PVP_SCORE_WIN_ADD;

        // 如果我的排名更低的话，则排名提升
        if (myRank > optRank) {
            BattleResult.AddRank = myRank - optRank;
            MyRank = optRank;
        }

        // 每打完一场，更换所有对手
        RequestChangePlayer();

        // 显示战斗结果
        UIManager.Instance.OpenWindow<UIPVPBattleResultView>(BattleResult);
    }

    // 获取逝去的时间应该正确显示的字符串
    public string GetElapseTimeString(float loginTime)
    {
        if (loginTime <= 0) {
            // 在线
            return Str.Get("UI_GUILD_NOW");
        } else if (loginTime <= 3600) {
            // 1小时内
            return Str.Get("UI_GUILD_HOUR_IN");
        } else if (loginTime <= 3600 * 24) {
            int hour = Mathf.FloorToInt(loginTime / 3600);
            return Str.Format("UI_GUILD_HOUR", hour);
        } else {
            int day = Mathf.FloorToInt(loginTime / (3600 * 24));
            return Str.Format("UI_GUILD_DAY", day);
        }
    }

    // 是否已领取积分奖励
    public bool HasGetScoreAward(int scoreID)
    {
        bool hasGet = false;
        MyScoreAwardList.TryGetValue(scoreID, out hasGet);
        return hasGet;
    }

    // 是否已领取最高排名奖励
    public bool HasGetHighAward(int highID)
    {
        bool hasGet = false;
        MyHighAwardList.TryGetValue(highID, out hasGet);
        return hasGet;
    }
}
