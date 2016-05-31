using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

public partial class PVPManager
{
    public void RegisterMsg()
    {
        Net.Register(eCommand.ATHTECLIC_LIST, OnMsgAthteclicList);
        Net.Register(eCommand.ATHTECLIC_RANK_AWARD_INFO, OnMsgAthteclicRankAwardInfo);
        Net.Register(eCommand.ATHTECLIC_RANK_LIST, OnMsgAthteclicRankList);
        Net.Register(eCommand.ATHTECLIC_ENERY_LIST, OnMsgAthteclicReportList);
        Net.Register(eCommand.ATHTECLIC_SCORE_AWARD_INFO, OnMsgAthteclicScoreAwardInfo);
        Net.Register(eCommand.OTHER_HERO_INF0, OnMsgOtherHeroInfo);
        Net.Register(eCommand.OTHER_PLAYER_INFO, OnMsgOtherPlayerInfo);
        Net.Register(eCommand.READY_ATHTECLIC_CHALLENGE, OnMsgReadyAthteclicChallenge);
    }

    private void OnMsgAthteclicList(byte[] buffer)
    {
        PAthleticsList ret = Net.Deserialize<PAthleticsList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.ATHTECLIC_LIST)) return;

        PVPManager.Instance.AttackCount = ret.freeTimes;    // 剩余次数
        PVPManager.Instance.AttackRemainTime.SetTimeMilliseconds(ret.nextAthlecticTime);  // 攻击冷却时间
        PVPManager.Instance.MyRank = ret.rank;  // 我的排名
        PVPManager.Instance.MyHighRank = ret.maxRank;   // 我的最高排名

        // 对手
        PVPManager.Instance.PlayerList.Clear();
        foreach (var item in ret.athletics) {
            PVPPlayerInfo info = new PVPPlayerInfo();
            info.Deserialize(item);
            PVPManager.Instance.PlayerList.Add(info);
        }

        PVPManager.Instance.SortPlayer();

        // 刷新ui
        UIManager.Instance.RefreshWindow<UIPVPView>();
    }

    private void OnMsgAthteclicRankAwardInfo(byte[] buffer)
    {
        PRankAwardinfo ret = Net.Deserialize<PRankAwardinfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.ATHTECLIC_RANK_AWARD_INFO)) return;

        PVPManager.Instance.MyHighAwardList.Clear();
        foreach (var item in ret.obtainedIds) {
            PVPManager.Instance.MyHighAwardList[item] = true;
        }

        // 刷新ui
        UIManager.Instance.RefreshWindow<UIPVPAwardView>();
    }

    private void OnMsgAthteclicRankList(byte[] buffer)
    {
        PAthleticsList ret = Net.Deserialize<PAthleticsList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.ATHTECLIC_RANK_LIST)) return;

        // 对手
        PVPManager.Instance.RankList.Clear();
        foreach (var item in ret.athletics) {
            PVPRankInfo info = new PVPRankInfo();
            info.Deserialize(item);
            PVPManager.Instance.RankList.Add(info);
        }

        // 刷新ui
        UIManager.Instance.RefreshWindow<UIPVPRankView>();
    }

    private void OnMsgAthteclicReportList(byte[] buffer)
    {
        PAthleticsLogList ret = Net.Deserialize<PAthleticsLogList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.ATHTECLIC_ENERY_LIST)) return;

        PVPManager.Instance.ReportList.Clear();

        foreach (var item in ret.athleticsLogs) {
            PVPReportInfo info = new PVPReportInfo();
            info.Deserialize(item);
            PVPManager.Instance.ReportList.Add(info);
        }

        // 刷新ui
        UIManager.Instance.RefreshWindow<UIPVPReportView>();
    }

    private void OnMsgAthteclicScoreAwardInfo(byte[] buffer)
    {
        PArenaScoreAward ret = Net.Deserialize<PArenaScoreAward>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.ATHTECLIC_SCORE_AWARD_INFO)) return;

        PVPManager.Instance.MyScoreAwardList.Clear();

        PVPManager.Instance.MyScore = ret.curScore;
        foreach (var item in ret.obtainedIds) {
            PVPManager.Instance.MyScoreAwardList[item] = true;
        }

        // 刷新ui
        UIManager.Instance.RefreshWindow<UIPVPScoreView>();
    }

    private void OnMsgOtherHeroInfo(byte[] buffer)
    {
        PPlayerInfo ret = Net.Deserialize<PPlayerInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.OTHER_HERO_INF0)) return;

        UserManager.Instance.Deserialize(ret);
    }

    private void OnMsgOtherPlayerInfo(byte[] buffer)
    {
        PPlayerInfo ret = Net.Deserialize<PPlayerInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.OTHER_PLAYER_INFO)) return;

        PVPPlayerInfo info = PVPManager.Instance.GetPlayer(ret.roleAttrs.userId);
        if (info != null) {
            info.Deserialize(ret);
        }

        UIManager.Instance.RefreshWindow<UIPVPPlayerInfoView>();
    }

    private void OnMsgReadyAthteclicChallenge(byte[] buffer)
    {
        PBattlVerify ret = Net.Deserialize<PBattlVerify>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.READY_ATHTECLIC_CHALLENGE)) return;

        BattleManager.Instance.StartBattle(ret.battleId, LogicBattleType.PVP);
    }



}