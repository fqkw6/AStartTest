using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

public partial class PVEManager
{
    public void RegisterMsg()
    {
        Net.Register(eCommand.BUYE_ENERGY, OnMsgBuyEnergy);
        Net.Register(eCommand.CHAPTER_PASS_LIST, OnMsgChapterPassList);
        Net.Register(eCommand.FUBEN_STAR_AWARD_INFO, OnMsgFubenStarAwardInfo);
        Net.Register(eCommand.GET_ENERGY, OnMsgGetEnergy);
        Net.Register(eCommand.GET_FUBEN_POS, OnMsgGetFuBenPos);
        Net.Register(eCommand.MOP_UP_FUBEN, OnMsgMopUpFuben);
        Net.Register(eCommand.MOU_UP_FUBEN_INFO, OnMsgMopUpFubenInfo);
    }

    private void OnMsgBuyEnergy(byte[] buffer)
    {
        PRoleEnergy ret = Net.Deserialize<PRoleEnergy>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.BUYE_ENERGY)) return;

        UserManager.Instance.SP += GameConfig.BUY_SP_GET;
        UserManager.Instance.CostMoney(GameConfig.BUY_SP_COST, PriceType.GOLD);
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_REFRESH_SP);
    }

    private void OnMsgChapterPassList(byte[] buffer)
    {
        PLilianPassList ret = Net.Deserialize<PLilianPassList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.CHAPTER_PASS_LIST)) return;

        if (ret.LlPass.Count <= 0) {
            // 刷新地图
            UIManager.Instance.RefreshWindow<UINewPVEEntranceView>();
            return;
        }
        
        LevelInfo.Clear();

        foreach (var item in ret.LlPass) {
            MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(item.Id);
            int chapterID = cfg.Chapter;

            List<LevelInfo> list;
            if (!LevelInfo.TryGetValue(chapterID, out list)) {
                list = new List<LevelInfo>();
                LevelInfo[chapterID] = list;
            }
            
            LevelInfo info = new LevelInfo();
            info.levelID = item.Id;
            info.star = item.star;
            info.fightCount = item.count;
            list.Add(info);
        }

        LastLevelID = GetLastLevelID(ChapterType.NORMAL);
        LastEliteLevelID = GetLastLevelID(ChapterType.ELITE);

        // 刷新地图
        UIManager.Instance.RefreshWindow<UINewPVEEntranceView>();
    }

    private void OnMsgFubenStarAwardInfo(byte[] buffer)
    {
        PFubenStarAwardInfo ret = Net.Deserialize<PFubenStarAwardInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.FUBEN_STAR_AWARD_INFO)) return;

        foreach (var item in ret.fubenGiftList) {
            int chapterID = item.chapterId;

            List<int> awardIdList = new List<int>();
            List<int> eliteAwardIdList = new List<int>();

            foreach (var retAward in item.starAwards) {
                ChapterType chapterType = (ChapterType)retAward.llDegree;
                foreach (var retAwardID in retAward.obtainIds) {
                    if (chapterType == (int)ChapterType.NORMAL) {
                        // 普通副本对应奖励已领取
                        awardIdList.Add(retAwardID);
                    } else {
                        // 精英副本对应奖励已领取
                        eliteAwardIdList.Add(retAwardID);
                    }
                }
            }

            ChapterAwardList[chapterID] = awardIdList;
            EliteChapterAwardList[chapterID] = eliteAwardIdList;
        }

        UIManager.Instance.RefreshWindow<UINewPVEEntranceView>();
    }

    private void OnMsgGetEnergy(byte[] buffer)
    {
        PRoleEnergy ret = Net.Deserialize<PRoleEnergy>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_ENERGY)) return;

        UserManager.Instance.SP = ret.energy;
        // ret.usedTimes
        // ret.freeTimes
        // ret.totalTimes

        EventDispatcher.TriggerEvent(EventID.EVENT_UI_REFRESH_SP);
    }

    private void OnMsgGetFuBenPos(byte[] buffer)
    {
        
    }

    private void OnMsgMopUpFuben(byte[] buffer)
    {
        MopUpResult ret = Net.Deserialize<MopUpResult>(buffer);

        if (!Net.CheckErrorCode(ret.errorCode, eCommand.MOP_UP_FUBEN)) return;

        // 添加奖励
        QuickFightResult.Clear();

        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(CurrentSelectLevelID);
        if (cfg == null) return;

        List<ItemInfo> list = new List<ItemInfo>();
        int awardExp = 0;
        int awardMoney = 0;
        foreach (var item in ret.awardCollections) {
            BattleResultInfo info = new BattleResultInfo();
            info.addPlayerExp = item.awdExp;
            awardExp += item.awdExp;

            foreach (var itemData in item.awardItems) {
                ItemInfo itemInfo = new ItemInfo();
                itemInfo.Deserialize(itemData);

                Debug.Log(itemInfo.ConfigID);

                if (itemInfo.ConfigID == GameConfig.ITEM_CONFIG_ID_MONEY) {
                    info.addMoney += itemInfo.Number;
                    awardMoney += itemInfo.Number;
                } else if (itemInfo.ConfigID == GameConfig.ITEM_CONFIG_ID_WOOD) {

                } else if (itemInfo.ConfigID == GameConfig.ITEM_CONFIG_ID_STONE) {

                } else if (itemInfo.ConfigID == GameConfig.ITEM_CONFIG_ID_GOLD) {

                } else {
                    info.itemInfo.Add(itemInfo);
                    list.Add(itemInfo);
                }
            }

            // 减少体力消耗
            UserManager.Instance.SP = Mathf.Max(0, UserManager.Instance.SP - cfg.StaminaCost);

            QuickFightResult.Add(info);
        }

        LevelInfo levelInfo = GetLevelInfo(CurrentSelectLevelID);
        if (levelInfo != null) {
            levelInfo.fightCount += QuickFightResult.Count;
        }

        // 打开界面显示结果
        if (QuickFightResult.Count > 1) {
            // 扫荡10次的结果
            UIManager.Instance.OpenWindow<UIPVEQuickFight10ResultView>();
        } else {
            // 扫荡1次
            UIManager.Instance.OpenWindow<UIPVEQuickFightResultView>();
        }

        UserManager.Instance.AddItem(list, true);
        UserManager.Instance.AddMoney(awardMoney, PriceType.MONEY);
        UserManager.Instance.OnAddUserExp(awardExp);

        // 刷新体力和金钱
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_REFRESH_SP);
        UIManager.Instance.RefreshWindow<UINewPVEEntranceView>();
    }

    private void OnMsgMopUpFubenInfo(byte[] buffer)
    {
    }
}