using System;
using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

// 关卡类型
public enum LevelType
{
    NORMAL,
    ELITE,
    BOSS,
    EVENT,
}

// 章节类型
public enum ChapterType
{
    NORMAL,
    ELITE,
}

public class BattleDataHeroInfo
{
    public int heroID;
    public int level;
    public int damage;
    public int damageGet;
    public int kill;
    public float time;

    // 外部计算赋值
    public int totalDamage;
    public int totalDamageGet;
    public int totalKill;
    public float totalTime;
}

public class BattleDataCostInfo
{
    public int soldierID;
    public int costCount;
}

public class BattleDataInfo
{
    public int totalDamage = 0;
    public int totalDamageGet = 0;
    public int totalKill = 0;
    public float totalTime = 0;
    public int totalEnemyDamage = 0;
    public int totalEnemyDamageGet = 0;
    public int totalEnemyKill = 0;
    public float totalEnemyTime = 0;

    public List<BattleDataHeroInfo> heroInfo = new List<BattleDataHeroInfo>();
    public List<BattleDataHeroInfo> enemyHeroInfo = new List<BattleDataHeroInfo>();
    public List<BattleDataCostInfo> costInfo = new List<BattleDataCostInfo>();

    public void CalcData()
    {
        totalDamage = 0;
        totalDamageGet = 0;
        totalKill = 0;
        totalTime = 0;
        foreach (var item in heroInfo) {
            totalDamage += item.damage;
            totalDamageGet += item.damageGet;
            totalKill += item.kill;
            totalTime += item.time;
        }

        totalEnemyDamage = 0;
        totalEnemyDamageGet = 0;
        totalEnemyKill = 0;
        totalEnemyTime = 0;
        foreach (var item in enemyHeroInfo) {
            totalEnemyDamage += item.damage;
            totalEnemyDamageGet += item.damageGet;
            totalEnemyKill += item.kill;
            totalEnemyTime += item.time;
        }

    }
}

public class BattleResultHeroInfo
{
    public long heroID;
    public int addExp;
}

public class BattleResultInfo
{
    public int star;
    public int addPlayerExp;
    public int addMoney;

    public List<BattleResultHeroInfo> heroInfo = new List<BattleResultHeroInfo>();
    public List<ItemInfo> itemInfo = new List<ItemInfo>();  
}

public class LevelInfo
{
    public int levelID;
    public int star;
    public int fightCount;
}

public enum FightAction
{
    FIGHT,
    QUICK_FIGHT,
    QUICK_FIGHT_10,
}

public partial class PVEManager
{
    public static readonly PVEManager Instance = new PVEManager();
    
    public BattleDataInfo BattleData;
    public BattleResultInfo BattleResult;
    
    public int LastLevelID = 0;         // 最新要挑战的关卡id
    public int LastEliteLevelID = 0;

    public int CurrentSelectChapterID = 0;
    public int CurrentSelectLevelID = 0;

    public Dictionary<int, List<LevelInfo>> LevelInfo = new Dictionary<int, List<LevelInfo>>();
    public Dictionary<int, List<int>> ChapterAwardList = new Dictionary<int, List<int>>();    // 章节星级奖励领取情况
    public Dictionary<int, List<int>> EliteChapterAwardList = new Dictionary<int, List<int>>();    // 章节星级奖励领取情况

    public List<BattleResultInfo> QuickFightResult = new List<BattleResultInfo>();  // 当前扫荡结果
     
    public ChapterType ChapterType; // 当前章节是普通章节还是精英章节

    private int _currentChapter = 1;      // 当前选择的是哪个章节
    public int CurrentChapter
    {
        get { return Mathf.Max(1, _currentChapter); }

        set { _currentChapter = Mathf.Max(1, value); }
    }

    // 获取关卡信息
    public LevelInfo GetLevelInfo(int levelID)
    {
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        int chapterID = cfg.Chapter;
        List<LevelInfo> list;
        if (!LevelInfo.TryGetValue(chapterID, out list)) {
            return null;
        }
        return list.Find(x => x.levelID == levelID);
    }

    public void AddLevelInfo(int levelID, int count, int star)
    {
        LevelInfo info = new LevelInfo();
        info.levelID = levelID;
        info.star = star;
        info.fightCount = count;

        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        int chapterID = cfg.Chapter;
        List<LevelInfo> list;
        if (!LevelInfo.TryGetValue(chapterID, out list)) {
            list = new List<LevelInfo>();
            LevelInfo[chapterID] = list;
        }

        list.Add(info);
    }

    // 此关卡是否可以攻打
    public bool IsLevelEnable(int levelID)
    {
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        if (cfg == null) return false;

        int lastLevelID = 0;
        if (ChapterType == ChapterType.NORMAL) {
            lastLevelID = GetNextLevel(LastLevelID, ChapterType.NORMAL);
            // 可重复挑战的boss关卡，只要id小于最后一个可攻打的关卡即可
            return lastLevelID == 0 || levelID <= lastLevelID;
        } else {
            // 可重复挑战的boss关卡，只要id小于最后一个可攻打的关卡即可
            lastLevelID = GetNextLevel(LastEliteLevelID, ChapterType.ELITE);
            return lastLevelID == 0 || levelID <= lastLevelID;
        }
    }

    // 此章节是否可以选择(存在当前章节的关卡，或者上一个章节已通关)
    public bool IsChapterEnable(int chapterID, ChapterType chapterType)
    {
        // 第一个场景必然是可以打的（TODO 考虑精英副本）
        if (chapterID == 1) {
            return true;
        }

        // 当前章节有已通关的关卡
        List<LevelInfo> list;
        if (LevelInfo.TryGetValue(chapterID, out list)) {
            foreach (var item in list) {
                MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(item.levelID);
                if (cfg.MissionDegree == (int)chapterType) {
                    return true;
                }
            }
        }

        // 上一个关卡已通关，则此关卡是可以打的
        if (IsFullPass(chapterID - 1, chapterType)) {
            return true;
        }
        return false;
    }

    // 当前章节是否通关（当前章节的所有关卡都已通关）
    public bool IsFullPass(int chapterID, ChapterType chapterType)
    {
        bool find = false;
        foreach (var item in MissionConstConfigLoader.Data) {
            MissionConstConfig cfg = item.Value;
            if (cfg.Chapter == chapterID && cfg.MissionDegree == (int)chapterType) {
                find = true;
                LevelInfo info = GetLevelInfo(cfg.Id);
                if (info == null || info.star <= 0) {
                    return false;
                }
            }
        }

        if (!find) {
            return false;
        }

        return true;
    }


    // 一场战斗胜利，更新level中的星级数据，并且更新最新关卡
    public void OnBattleResult(PBattleReport data)
    {
        if (data.pos == null) {
            Log.Error("OnBattleResult error, PBattleReport 没有设置 data.pos");
            return;
        }

        int levelID = data.pos.x;
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        if (cfg == null) return;

        UserManager.Instance.SP = Mathf.Max(0, UserManager.Instance.SP - cfg.StaminaCost);

        bool isAttacker = UserManager.Instance.EntityID == data.attackerId;
        if (isAttacker) {
            if (data.winner == eBattleSide.SIDE_DEFENSER) {
                // 输了
                UIManager.Instance.OpenWindow<UIPVEBattleResultFailView>();
                return;
            }
        } else {
            if (data.winner == eBattleSide.SIDE_ATTACKER) {
                // 输了
                UIManager.Instance.OpenWindow<UIPVEBattleResultFailView>();
                return;
            }
        }
        
        // 赢了
        LevelInfo info = GetLevelInfo(levelID);
        if (info != null) {
            info.star = data.star;
            ++info.fightCount;
        } else {
            AddLevelInfo(levelID, cfg.TimesLimit > 0 ? cfg.TimesLimit - 1 : 0, data.star);
        }

        ChapterType chapterType = (ChapterType)cfg.MissionDegree;
        if (chapterType == ChapterType.NORMAL) {
            LastLevelID = Mathf.Max(LastLevelID, levelID);
            int normalLastLevelID = GetLastLevelID(ChapterType.NORMAL);

            // 最后一个关卡打完后，自动跳到下一个
            if (levelID == normalLastLevelID) {
                CurrentSelectLevelID = GetNextLevel(levelID, chapterType);
            }
        } else if (chapterType == ChapterType.ELITE) {
            LastEliteLevelID = Mathf.Max(LastEliteLevelID, levelID);
            int eliteLastLevelID = GetLastLevelID(ChapterType.ELITE);

            // 最后一个关卡打完后，自动跳到下一个
            if (levelID == eliteLastLevelID) {
                CurrentSelectLevelID = GetNextLevel(levelID, chapterType);
            }
        }

        // 刷新副本界面
        UIManager.Instance.RefreshWindow<UINewPVEEntranceView>();

        // 战斗结果
        BattleResult = new BattleResultInfo();
        BattleResult.addPlayerExp = data.awdExp;
        BattleResult.star = data.star;
        
        foreach (var item in data.awardPoits) {
            if (item.roleType == eRoleType.ROLE_LORD) {
            } else if (item.roleType == eRoleType.ROLE_HERO) {
                BattleResultHeroInfo heroInfo = new BattleResultHeroInfo();
                heroInfo.heroID = item.geter;
                heroInfo.addExp = item.awExp;
                BattleResult.heroInfo.Add(heroInfo);
            }

            foreach (var item2 in item.awardItems) {
                if (item2.type == eItem.CURRENCY) {
                    // 金钱
                    BattleResult.addMoney = item2.num;
                } else {
                    ItemInfo itemInfo = new ItemInfo();
                    itemInfo.Deserialize(item2);
                    BattleResult.itemInfo.Add(itemInfo);
                }
            }
        }
        
        UIManager.Instance.OpenWindow<UIPVEBattleResultView>();
    }

    // 获取下一个关卡的id
    public int GetNextLevel(int curLevelID, ChapterType chapterType)
    {
        if (curLevelID == 0) {
            // 获取第一个关卡
            foreach (var item in MissionConstConfigLoader.Data) {
                if (item.Value.MissionDegree == (int)chapterType) {
                    return item.Key;
                }
            }
        } else {
            bool find = false;
            foreach (var item in MissionConstConfigLoader.Data) {
                if (item.Key == curLevelID) {
                    find = true;
                    continue;
                }

                if (find && (item.Value.MissionDegree == (int)chapterType)) {
                    return item.Key;
                }
            }
        }
        
        return 0;
    }

    // 获取当前章节的星数
    public int GetChapterStar(int chapterID, ChapterType chapterType)
    {
        int star = 0;
        List<LevelInfo> list;
        LevelInfo.TryGetValue(chapterID, out list);
        if (list == null) return 0;

        foreach (var item in list) {
            MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(item.levelID);
            if (cfg.MissionDegree == (int)chapterType) {
                // 只有精英和boss关卡统计星级
                star += item.star;
            }
        }

        return star;
    }

    // 获取总星数
    public int GetFullChapterStar(int chapterID, ChapterType chapterType)
    {
        int star = 0;

        foreach (var item in MissionConstConfigLoader.Data) {
            if (item.Value.Chapter == chapterID && item.Value.MissionDegree == (int) chapterType) {
                star += 3;
            }
        }
        return star;
    }

    // 获取最后一个副本的位置
    public int GetLastLevelID(ChapterType chapterType)
    {
        int maxLevelID = 0;
        foreach (var chapter in LevelInfo) {
            foreach (var level in chapter.Value) {
                MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(level.levelID);
                if (cfg.MissionDegree == (int) chapterType && level.levelID > maxLevelID) {
                    maxLevelID = level.levelID;
                }
            }
        }

        return maxLevelID;
    }

    // 是否已经领取对应章节的星级奖励
    public bool HasChapterAward(int chapterID, ChapterType chapterType, int index)
    {
        List<int> list;
        if (chapterType == ChapterType.NORMAL) {
            List<int> fullList = new List<int>();

            foreach (var item in ChapterAwardConfigLoader.Data) {
                if (item.Value.ChapterID == chapterID) {
                    if (item.Value.Degree == (int)ChapterType.NORMAL) {
                        fullList.Add(item.Key);
                    }
                }
            }

            // 普通章节领取情况
            ChapterAwardList.TryGetValue(chapterID, out list);
            return list != null && list.Count > index && fullList.Count > index && list.IndexOf(fullList[index]) != -1;
        } else {
            List<int> fullList = new List<int>();

            foreach (var item in ChapterAwardConfigLoader.Data) {
                if (item.Value.ChapterID == chapterID) {
                    if (item.Value.Degree == (int)ChapterType.ELITE) {
                        fullList.Add(item.Key);
                    }
                }
            }

            // 精英章节领取情况
            EliteChapterAwardList.TryGetValue(chapterID, out list);
            return list != null && list.Count > index && fullList.Count > index && list.IndexOf(fullList[index]) != -1;
        }
    }

    // 此关卡是否还有挑战次数
    public bool HasFightCount(int levelID)
    {
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        if (cfg.TimesLimit <= 0) {
            return true;
        }

        LevelInfo info = GetLevelInfo(levelID);
        if (info == null) {
            return true;
        }

        return info.fightCount < cfg.TimesLimit;
    }

    public int GetQuickFightCount(int levelID)
    {
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        // 受体力限制
        int count = Mathf.Min(GameConfig.PVE_MAX_QUICK_FIGHT_COUNT, UserManager.Instance.SP / cfg.StaminaCost);
        LevelInfo levelInfo = PVEManager.Instance.GetLevelInfo(levelID);
        if (levelInfo != null) {
            // 挑战次数不足
            if (cfg.TimesLimit > 0) {
                count = Mathf.Min(count, cfg.TimesLimit - levelInfo.fightCount);
            }
        } else {
            // 还没有挑战过的关卡
            if (cfg.TimesLimit > 0) {
                count = Mathf.Min(count, cfg.TimesLimit);
            }
        }
        return count;
    }

    public void RequestAddSp()
    {
        Net.Send(eCommand.BUYE_ENERGY);
    }

    // 向服务器请求重置挑战次数
    public void RequestResetFightCount(int levelID)
    {
        PCMInt data = new PCMInt();
        data.arg = levelID;
        Net.Send(eCommand.BUY_ELITE_COUNT, data, (buffer) => {
            CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
            if (!Net.CheckErrorCode(ret.err_code, eCommand.BUY_ELITE_COUNT)) return;

            // 重置攻打次数
            LevelInfo info = GetLevelInfo(levelID);
            if (info != null) {
                info.fightCount = 0;
            }

            UserManager.Instance.CostMoney(GameConfig.PVE_RESET_FIGHT_COUNT_COST, PriceType.GOLD);
            UIManager.Instance.RefreshWindow<UINewPVELevelInfoView>();
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });
    }

    // 请求关卡信息
    public void RequestLevelInfo(int chapter)
    {
        PCMInt data = new PCMInt();
        data.arg = chapter;
        Net.Send(eCommand.CHAPTER_PASS_LIST, data);
    }

    public void RequestLevelPosition()
    {
    }

    // 请求攻打关卡
    public void RequestFight(int levelID)
    {
        Log.Info("RequestFight:  {0}", levelID);
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);
        if (cfg == null) return;

        if (UserManager.Instance.SP < cfg.StaminaCost) {
            UIUtil.ShowErrMsgFormat("UI_NOT_ENOUGTH_SP");
            return;
        }

        eCommand cmd = eCommand.READY_BT_FUBEN_NORMAL;
        if (cfg.MissionDegree == (int)ChapterType.NORMAL) {
            cmd = eCommand.READY_BT_FUBEN_NORMAL;
        } else if (cfg.MissionDegree == (int) ChapterType.ELITE) {
            cmd = eCommand.READY_BT_FUBEN_ELITE;
        }

        PAttack data = new PAttack();
//        foreach (var item in UserManager.Instance.PVEHeroList) {
//            data.attackerId.Add(item.EntityID);
//        }
//        
        if (UserManager.Instance.HeroList.Count <= 0) {
            Log.Error("RequestFight error, no hero");
            return;
        }

        for (int i = 0; i < 3; ++i) {
            if (i < UserManager.Instance.HeroList.Count) {
                data.attackerId.Add(UserManager.Instance.HeroList[i].EntityID);
            }
        }

        data.pos = new PVector();
        data.pos.x = levelID;

        NetworkManager.Instance.Send(cmd, data, (buffer) =>
        {
            PBattlVerify ret = Net.Deserialize<PBattlVerify>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, cmd)) return;

            BattleManager.Instance.StartBattle(ret.battleId, LogicBattleType.PVE);
        });
    }

    // 请求快速扫荡
    public void RequestQuickFight(int levelID, int count = 1)
    {
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(levelID);

        PMopUpFuben data = new PMopUpFuben();
        data.pos = levelID;
        data.llDegree = cfg != null ? (eLLDegree)cfg.MissionDegree : eLLDegree.LLD_NORMAL;
        data.force = true;
        data.mouUpTimes = count;
        Net.Send(eCommand.MOP_UP_FUBEN, data);
    }
    
    // 请求章节奖励领取数据
    public void RequestChapterAwardInfo()
    {
        Net.Send(eCommand.FUBEN_STAR_AWARD_INFO);
    }

    // 请求领取关卡星级奖励
    public void RequestGetChapterAward(int awardID, int chapterID, ChapterType chapterType)
    {
        PCMInt data = new PCMInt();
        data.arg = awardID;

        NetworkManager.Instance.Send(eCommand.OBTAIN_FUBEN_STAR_AWARD, data, (buffer) => {
            PAwardPointList ret = Net.Deserialize<PAwardPointList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.OBTAIN_FUBEN_STAR_AWARD)) return;
            
            // 领取奖励成功

            List<ItemInfo> itemList = new List<ItemInfo>();
            foreach (var item in ret.awardPoits) {
                UserManager.Instance.OnAddUserExp(item.awExp);

                foreach (var itemData in item.awardItems) {
                    ItemInfo itemInfo = new ItemInfo();
                    itemInfo.Deserialize(itemData);
                    itemList.Add(itemInfo);
                }
            }

            // 实际添加物品
            UserManager.Instance.AddItem(itemList, true);

            // 显示获得新物品
            if (itemList.Count > 0) {
               UIManager.Instance.OpenWindow<UIGetItemView>(itemList);                                                                            
            }

            // 更新现存数据
            List<int> list;
            if (chapterType == ChapterType.NORMAL) {
                ChapterAwardList.TryGetValue(chapterID, out list);
                if (list == null) {
                    list = new List<int>();
                    ChapterAwardList[chapterID] = list;
                }
            } else {
                EliteChapterAwardList.TryGetValue(chapterID, out list);
                if (list == null) {
                    list = new List<int>();
                    EliteChapterAwardList[chapterID] = list;
                }
            }

            list.Add(awardID);
            UIManager.Instance.RefreshWindow<UINewPVEEntranceView>();
            UIManager.Instance.RefreshWindow<UIPVEStarAwardView>();
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });
    }
}
