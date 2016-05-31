using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using comrt.comnet;

public partial class EventID
{
    public const string EVENT_UI_HERO_VIEW_CLOSE = "EVENT_UI_HERO_VIEW_CLOSE";               // 点击英雄装备界面的关闭按钮，则关闭总英雄界面
    public const string EVENT_UI_HERO_VIEW_CHANGE_TO_INFO = "EVENT_UI_HERO_VIEW_CHANGE_TO_INFO";      // 英雄左侧界面切换
    public const string EVENT_UI_HERO_VIEW_CHANGE_TO_DETAIL = "EVENT_UI_HERO_VIEW_CHANGE_TO_DETAIL";    // 英雄左侧界面切换
    public const string EVENT_UI_HERO_VIEW_CHANGE_TO_SKILL = "EVENT_UI_HERO_VIEW_CHANGE_TO_SKILL";     // 英雄左侧界面切换
}

// 玩家的数据 例如金币 钻石
public partial class UserManager
{
    public static readonly UserManager Instance = new UserManager();

    public List<HeroInfo> HeroList = new List<HeroInfo>(); // 英雄列表
    public List<ItemInfo> ItemList = new List<ItemInfo>(); // 物品列表

    public List<HeroInfo> PVEHeroList = new List<HeroInfo>();   // pve的上场英雄列表 

    // 玩家属性
    public long EntityID;
    public string RoleName;
    public int Icon;
    public int Sex;
    public int Level;
    public int Exp;
    public int VipLevel;
    public int RoleConfigID;

    // 玩家资产
    public int Wood; // 木材
    public int Stone; // 石材
    public int Money; // 银两
    public int Gold; // 黄金（付费）

    public int MaxMoneyStorage; // 最大银两储量
    public int MaxWoodStorage; // 最大木材储量
    public int MaxStoneStorage; // 最大石材储量
    
    public int BattleFeats = 0;     // 战功牌
    public int GuideStep = 0;       // 新手引导步骤

    private int SkillPoint = 0; // 当前技能点数
    private float SkillPointNextTime = 0; // 距离下次获取技能点的剩余秒数
    private float SkillPointSyncTime = 0; // 记录NextTime的时刻

    public int SP = 0;     // 当前体力

    // 英雄是否已上阵
    public bool IsHeroInPVEFormation(int cfgID)
    {
        return PVEHeroList.Find(x => x.ConfigID == cfgID) != null;
    }

    // 获取战斗力
    public int GetFightScore()
    {
        int fightScore = 0;
        foreach (var item in HeroList) {
            fightScore += item.FightingScore;
        }

        fightScore += CityManager.Instance.GetFightScore();
        return fightScore;
    }

    // 获取其他玩家的信息
    public void RequestPlayerInfo(long playerID)
    {
        PComReq builder = new PComReq();
        builder.para1.Add(playerID);
        Net.Send(eCommand.OTHER_PLAYER_INFO, builder);
    }
    
    //吃经验书增加英雄经验
    public void RequestUseExpBook(long heroID,long itemID,int number)
    {
        HeroInfo info = GetHeroInfo(heroID);
        if (info == null) {
            return;
        }

        // 英雄等级不能超过玩家的等级
        if (info.Level >= Level) {
            UIUtil.ShowMsgFormat("MSG_HERO_PLAYER_LEVEL_LIMIT");
            return;
        }

        PUsedExpBook builder = new PUsedExpBook();
        builder.heroId = heroID;
        builder.goodId = itemID;
        builder.goodNum = number;

        NetworkManager.Instance.Send(eCommand.USED_EXPBOOK, builder, (buffer) => {
            PHeroAttr ret = Net.Deserialize<PHeroAttr>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.USED_EXPBOOK)) return;

            info.Deserialize(ret, true);

            UseItem(itemID,number);

            UIManager.Instance.RefreshWindow<UINewHeroView>();
            UIManager.Instance.RefreshWindow<UINewHeroListView>();
            UIManager.Instance.RefreshWindow<UISelectHeroView>();
        });
    }

    public void JoinGame()
    {
        // 在ServerManager中处理接收消息
        PJoinGame builder = new PJoinGame();
        builder.userId = ServerManager.Instance.AccountID;
        builder.roleId = EntityID;
        Net.Send(eCommand.JOIN, builder);
    }

    // 添加英雄
    public void AddHero(HeroInfo info)
    {
        if (HaveHero(info.ConfigID)) {
            return;
        }

        HeroList.Add(info);
    }

    // 是否存在一个英雄
    public bool HaveHero(int unitID)
    {
        HeroInfo exists = GetHeroInfoByUnitID(unitID);
        return exists != null;
    }

    // 获取英雄信息
    public HeroInfo GetHeroInfo(long heroID)
    {
        HeroInfo exists = HeroList.Find(x => x.EntityID == heroID);
        return exists;
    }

    public HeroInfo GetHeroInfoByUnitID(int unitID)
    {
        HeroInfo exists = HeroList.Find(x => x.ConfigID == unitID);
        return exists;
    }
    
    // 获取英雄升星所需要的碎片数目
    public int GetHeroStarUpgradeCost(int heroConfigID, int star)
    {
        HeroStarUpgradeConfig cfg = HeroStarUpgradeConfigLoader.GetConfig(heroConfigID, star);
        if (cfg == null) {
            return 0;
        }
        return cfg.Cost;
    }

    // 获取物品数量
    public int GetItemCount(int itemConfigID)
    {
        ItemInfo info = ItemList.Find((x) => x.ConfigID == itemConfigID);
        return info != null ? info.Number : 0;
    }

    //根据物品实例ID返回一个物品
    public ItemInfo GetItem(long itemID)
    {
        ItemInfo info = ItemList.Find((x) => x.EntityID == itemID);
        return info;
    }

    // 根据物品id获取一个物品信息
    public ItemInfo GetItemByConfigID(int itemCfgID)
    {
        ItemInfo info = ItemList.Find((x) => x.ConfigID == itemCfgID);
        return info;
    }

    // 根据装备类型获取物品
    public ItemInfo GetItemByType(ItemType itemType)
    {
        ItemInfo info = ItemList.Find((x) => x.Cfg.Type == (int)itemType);
        return info;
    }

    public void RemoveItem(ItemInfo item)
    {
        ItemList.Remove(item);
    }

    // 获取背包中的英雄碎片数目
    public int GetHeroPieceCount(int itemCfgID)
    {
        ItemInfo info = GetItemByConfigID(itemCfgID);
        if (info == null) {
            return 0;
        }
        return info.Number;
    }

    // 根据英雄id获取背包中对应的碎片数目
    public int GetHeroPieceCountByHeroID(int heroCfgID)
    {
        HeroConfig cfg = HeroConfigLoader.GetConfig(heroCfgID);
        if (cfg == null) {
            return 0;
        }
        return GetHeroPieceCount(cfg.Cost);
    }

    // 英雄列表排序 高星 高品阶的排在前面
    public void SortHeroList()
    {
        SortHeroList(HeroList);
    }

    public void SortHeroList(List<HeroInfo> list)
    {
        list.Sort((a, b) => {
            if (a.StarLevel != b.StarLevel) {
                return -a.StarLevel.CompareTo(b.StarLevel);
            }

            if (a.Level != b.Level) {
                return -a.Level.CompareTo(b.Level);
            }

            if (a.Exp != b.Exp) {
                return -a.Exp.CompareTo(b.Exp);
            }
            return a.ConfigID.CompareTo(b.ConfigID);
        });
    }

    public int GetMoney(PriceType priceType)
    {
        switch (priceType) {
            case PriceType.GOLD:
                return 0;
            case PriceType.MONEY:
                return Money;
            case PriceType.STONE:
                return Stone;
            case PriceType.WOOD:
                return Wood;
            case PriceType.ARENACURRENCY:
                return 0;
            case PriceType.EXPEDITION:
                return 0;
            case PriceType.PROPS:
                return 0;
        }
        return 0;
    }

    public void AddMoney(int value, PriceType priceType)
    {
        switch (priceType) {
            case PriceType.GOLD:
                break;
            case PriceType.MONEY:
                Money += value;
                break;
            case PriceType.STONE:
                Stone += value;
                break;
            case PriceType.WOOD:
                Wood += value;
                break;
            case PriceType.ARENACURRENCY:
                break;
            case PriceType.EXPEDITION:
                break;
            case PriceType.PROPS:
                break;
        }
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
    }

    public void CostMoney(int value, PriceType priceType)
    {
        switch (priceType) {
            case PriceType.GOLD:
                Gold -= value;
                break;
            case PriceType.MONEY:
                Money -= value;
                break;
            case PriceType.STONE:
                Stone -= value;
                break;
            case PriceType.WOOD:
                Wood -= value;
                break;
            case PriceType.ARENACURRENCY:
                break;
            case PriceType.EXPEDITION:
                break;
            case PriceType.PROPS:
                break;
        }
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
    }

    public void Deserialize(PPlayerInfo data)
    {
        EntityID = data.roleAttrs.roleId;
        RoleConfigID = data.roleAttrs.roleCfgId;
        RoleName = data.roleAttrs.roleName;
        Sex = (int)data.roleAttrs.sex;
        Level = data.roleAttrs.level;
        Exp = data.roleAttrs.xiuwei;
        VipLevel = data.roleAttrs.vipLevel;
        SP = data.roleAttrs.energy;

        Wood = data.assertAttr.wood;
        Stone = data.assertAttr.stone;
        Money = data.assertAttr.gold;
        Gold = data.assertAttr.yuanbao;

        //Forage = data.assertAttr.forage;

        // 英雄
        HeroList.Clear();
        foreach (var item in data.heroAttrs) {
            HeroInfo info = new HeroInfo();
            info.Deserialize(item, false);
            HeroList.Add(info);
        }

        // 初始化物品
        ItemList.Clear();
        foreach (var item in data.itemList) {
            ItemInfo info = new ItemInfo();
            info.Deserialize(item);
            ItemList.Add(info);
        }

        PVEHeroList.Clear();
        foreach (var item in data.pBattlePT) {
            if (item.type == eLineUpType.BT_PVE) {
                foreach (var hero in item.heroPT) {
                    HeroInfo heroInfo = GetHeroInfo(hero.hero.heroId);
                    if (heroInfo != null) {
                        // 上阵英雄
                        PVEHeroList.Add(heroInfo);
                    }
                }
                break;
            }
        }

        // 战斗力和战功牌
        BattleFeats = data.roleAttrs.battleFeat;

        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);

        UIManager.Instance.RefreshWindow<UINewBagView>();
    }

    // 解析玩家数据
    public void Deserialize(PRoleAttr data)
    {
        EntityID = data.roleId;
        RoleConfigID = data.roleCfgId;
        RoleName = data.roleName;
        Sex = (int)data.sex;
        Level = data.level;
        Exp = data.xiuwei;
        VipLevel = data.vipLevel;
        BattleFeats = data.battleFeat;

        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);

        UIManager.Instance.RefreshWindow<UINewBagView>();
    }
    
    // 解析资产数据
    public void Deserialize(PAssertAttr data)
    {
        Wood = data.wood;
        Stone = data.stone;
        Money = data.gold;
        Gold = data.yuanbao;
    }

    public bool CheckHeroUpgradeStarItem(HeroInfo info)
    {
        int starCost = GetHeroStarUpgradeCost(info.ConfigID, info.StarLevel);
        ItemInfo itemInfo = GetItemByConfigID(info.Cfg.Cost);
        if (itemInfo == null || itemInfo.Number < starCost) {
            return false;
        }
        return true;
    }

    // 英雄升星
    public void RequestHeroUpgradeStar(long heroID)
    {
        HeroInfo info = GetHeroInfo(heroID);
        if (info == null) return;

        if (info.StarLevel >= GameConfig.MAX_HERO_STAR) {
            // 已经最高星了，无法继续升级
            UIUtil.ShowMsgFormat("MSG_HERO_STAR_MAX");
            return;
        }

        if (!CheckHeroUpgradeStarItem(info)) {
            UIUtil.ShowMsgFormat("MSG_HERO_UPGRADE_STAR_LIMIT");
            return;
        }

        PCMLong builder = new PCMLong();
        builder.arg = heroID;

        NetworkManager.Instance.Send(eCommand.HERO_UPGRADE_STAR, builder, (buffer) =>
        {
            PHeroAttr ret = Net.Deserialize<PHeroAttr>(buffer);
            Log.Info("英雄升星成功");
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.HERO_UPGRADE_STAR)) return;

            HeroInfo heroInfo = GetHeroInfo(heroID);
            if (heroInfo == null) return;

            int starCost = GetHeroStarUpgradeCost(heroInfo.ConfigID, heroInfo.StarLevel);
            ItemInfo itemInfo = GetItemByConfigID(heroInfo.Cfg.Cost);
            if (itemInfo == null || itemInfo.Number < starCost) {
                Log.Error("碎片不足");
                return;
            }

            HeroInfo oriHeroInfo = heroInfo.Clone();

            // 同步服务器数据 主要是获取新的技能
            heroInfo.Deserialize(ret, false);

            UseItemByConfigID(heroInfo.Cfg.Cost, starCost);

            UIManager.Instance.RefreshWindow<UINewHeroView>();
            UIManager.Instance.RefreshWindow<UINewHeroListView>();

            UIManager.Instance.OpenWindow<UIHeroStarRisingSuccView>(oriHeroInfo, heroInfo);
        });
    }
    
    public void RequestSkillUpgrade(long heroID, int skillID, int index)
    {
        HeroInfo info = GetHeroInfo(heroID);
        if (info == null) return;

        SkillInfo skillInfo = info.GetSkillByID(skillID);
        
        int level = 1;
        if (skillInfo != null) {
            level = skillInfo.Level;
        }

        if (level + 1 > info.Level) {
            UIUtil.ShowMsgFormat("MSG_HERO_SKILL_NEED_HERO_LEVEL", info.Level + 1);
            return;
        }

        int moneyCost = SkillInfo.GetUpgradeCost(index, level);
        if (UserManager.Instance.Money < moneyCost) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
            return;
        }

        if (UserManager.Instance.GetCurrentSkillPoint() <= 0) {
            UIUtil.ShowMsgFormat("MSG_HERO_SKILL_POINT_LIMIT");
            return;
        }

        PSkill builder = new PSkill();
        builder.heroId = heroID;
        builder.cfgId = skillID;

        NetworkManager.Instance.Send(eCommand.UPGRADE_SKILL, builder, (buffer)=>{
            CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
            if (!Net.CheckErrorCode(ret.err_code, eCommand.UPGRADE_SKILL)) return;

            // 减去消耗的金钱和技能点
            Money -= moneyCost;
            UseSkillPoint();

            if (skillInfo != null) {
                ++skillInfo.Level;
            }

            // 刷新英雄技能界面
            UIManager.Instance.RefreshWindow<UINewHeroView>();
            UIManager.Instance.RefreshWindow<UINewHeroListView>();

            // 刷新主界面的金钱数据
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });
    }

    //根据实例ID使用物品
    public void UseItem(long itemID, int count)
    {
        ItemInfo info = GetItem(itemID);
        if (info == null) {
            return;
        }

        info.Number -= count;

        if (info.Number <= 0) {
            ItemList.Remove(info);
        }
    }

    //根据配置ID使用物品
    public void UseItemByConfigID(int itemCfgID, int count = 1)
    {
        ItemInfo info = GetItemByConfigID(itemCfgID);
        UseItem(info.EntityID,count);
        
    }

    // 请求穿戴装备
    public bool RequestEquipItem(long heroID, long itemID)
    {
        HeroInfo heroInfo = GetHeroInfo(heroID);
        if (heroInfo == null) return false;

        if (heroInfo.GetItem(itemID) != null) {
            // 已经装备
            return false;
        }

        ItemInfo itemInfo = GetItem(itemID);
        if (itemInfo == null) {
            // 没有物品
            return false;
        }

        if (itemInfo.Cfg.Level > heroInfo.Level) {
            // 等级不足
            UIUtil.ShowMsgFormat("MSG_HERO_LEVEL_LIMIT");
            return false;
        }

        PUseCommonItem builder = new PUseCommonItem();
        builder.heroId = heroID;

        PComItem ibuilder = new PComItem();
        ibuilder.id = itemInfo.EntityID;
        builder.comItem.Add(ibuilder);

        NetworkManager.Instance.Send(eCommand.FIT_UP_EQUIP, builder, (buffer)=>{
            PHeroAttr ret = Net.Deserialize<PHeroAttr>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.FIT_UP_EQUIP)) return;

            // 把旧装备放在背包里面
            ItemInfo oldItemInfo = heroInfo.GetItemByType((ItemType)itemInfo.Cfg.Type);
            if (oldItemInfo != null) {
                AddItem(oldItemInfo, false);
                heroInfo.EquipedItem.Remove(oldItemInfo);
            }

            // 同步英雄属性
            heroInfo.Deserialize(ret, true);

            // 移除背包内的装备
            UseItem(itemID, 1);

            UIManager.Instance.RefreshWindow<UINewHeroView>();
            UIManager.Instance.RefreshWindow<UINewHeroListView>();
        });

        return true;
    }
    
    private int TestEquipItem(HeroInfo heroInfo, int itemCfgID)
    {
        ItemInfo info = GetItemByConfigID(itemCfgID);
        if (info == null) {
            // 没有此物品
            return 0;
        }

        if (info.Cfg.Level > heroInfo.Level) {
            // 等级限制，无法装备
            return 0;
        }
        return info.ConfigID;
    }
    
    private void EquipItem(HeroInfo info, long equipItemID)
    {
        ItemInfo itemInfo = GetItem(equipItemID);
        if (itemInfo != null) {
            info.EquipedItem.Add(itemInfo);

            UseItem(equipItemID, 1);
        }
    }

    public void RequestSellItem(long itemID, int count)
    {
        
    }

    public void SetSkillPoint(int point, long nextTime)
    {
        SkillPoint = point;

        if (SkillPoint >= GameConfig.MAX_SKILL_POINT) {
            // 满技能点时不需要同步时间
            SkillPointNextTime = 0;
            SkillPointSyncTime = 0;
        } else {
            SkillPointNextTime = nextTime / 1000.0f;
            SkillPointSyncTime = Time.realtimeSinceStartup;
        }
    }

    // 获取获得技能点的倒计时(秒)
    public int GetSkillPointCountDownTime()
    {
        if (SkillPoint >= GameConfig.MAX_SKILL_POINT) {
            // 技能点是满的，不需要考虑倒计时
            return 0;
        }

        int time = Mathf.FloorToInt(Time.realtimeSinceStartup - SkillPointSyncTime);
        if (time < SkillPointNextTime) {
            return Mathf.FloorToInt(SkillPointNextTime - time);
        } else {
            return Mathf.FloorToInt(GameConfig.SKILL_POINT_GET_INTERVAL - (time - SkillPointNextTime) % GameConfig.SKILL_POINT_GET_INTERVAL);
        }
    }

    // 获取当前技能点数
    public int GetCurrentSkillPoint()
    {
        if (SkillPoint >= GameConfig.MAX_SKILL_POINT) {
            // 技能点是满的，不需要考虑倒计时
            return SkillPoint;
        }

        float time = Time.realtimeSinceStartup - SkillPointSyncTime;
        int skillPoint = Mathf.FloorToInt(SkillPoint + (time + GameConfig.SKILL_POINT_GET_INTERVAL - SkillPointNextTime)/GameConfig.SKILL_POINT_GET_INTERVAL);
        if (skillPoint >= GameConfig.MAX_SKILL_POINT) {
            // 技能点已满
            SkillPointNextTime = 0;
            SkillPointSyncTime = 0;
            SkillPoint = GameConfig.MAX_SKILL_POINT;
        }

        return skillPoint;
    }

    public void UseSkillPoint()
    {
        int skillPoint = GetCurrentSkillPoint();
        if (skillPoint >= GameConfig.MAX_SKILL_POINT) {
            // 满技能点的情况
            SkillPoint = GameConfig.MAX_SKILL_POINT - 1;
            SkillPointNextTime = GameConfig.SKILL_POINT_GET_INTERVAL;
            SkillPointSyncTime = Time.realtimeSinceStartup;
        } else {
            int countDown = GetSkillPointCountDownTime();
            SkillPoint = Mathf.Max(skillPoint - 1, 0);
            SkillPointNextTime = countDown;
            SkillPointSyncTime = Time.realtimeSinceStartup;
        }
    }

    public int GetMaxSP()
    {
        PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(Level);
        if (cfg != null) {
            return cfg.MaxStamina;
        }

        return 0;
    }

    public void OnAddUserExp(int exp)
    {
        PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(Level);
        if (cfg != null) {
            if (Exp + exp >= cfg.Exp) {
                // 升级的情况由服务端处理
            } else {
                Exp += exp;
            }
        }
    }

    public void AddItem(PComItemList data, bool isNewItem)
    {
        // 添加物品
        List<ItemInfo> list = new List<ItemInfo>();
        foreach (var item in data.item) {
            if (item.type == (eItem)ItemType.MONEY) {
                // 货币类型
                if (item.cfgId == GameConfig.ITEM_CONFIG_ID_MONEY) {
                    Money += item.num;
                } else if (item.cfgId == GameConfig.ITEM_CONFIG_ID_WOOD) {
                    Wood += item.num;
                } else if (item.cfgId == GameConfig.ITEM_CONFIG_ID_STONE) {
                    Stone += item.num;
                } else if (item.cfgId == GameConfig.ITEM_CONFIG_ID_GOLD) {
                    Gold += item.num;
                }
                continue;
            } else if (item.type == (eItem) ItemType.CARD) {
                continue;
            }

            ItemInfo itemInfo = new ItemInfo();
            itemInfo.Deserialize(item);
            list.Add(itemInfo);
        }

        AddItem(list, isNewItem);
    }

    public void AddItem(ItemInfo info, bool isNewItem)
    {
        bool hasNewItem = false;
        // 添加物品
        ItemInfo existItemInfo = GetItem(info.EntityID);
        if (existItemInfo != null) {
            // 物品已存在，只加数量
            existItemInfo.Number += info.Number;
            existItemInfo.AwardTime = Time.realtimeSinceStartup;
        } else {
            // 只有新增的物品才会被标识为新物品
            info.IsNewItem = isNewItem;
            info.AwardTime = Time.realtimeSinceStartup;
            ItemList.Add(info);
            if (isNewItem) {
                hasNewItem = true;
            }
        }
        // 有新物品，显示背包提示
        if (hasNewItem) {
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_NEW_FLAG, GameFunc.BAG, true);
        }
    }

    public void AddItem(List<ItemInfo> data, bool isNewItem)
    {
        bool hasNewItem = false;
        // 添加物品
        foreach (var itemInfo in data) {
            if (itemInfo.Cfg.Type == (int) ItemType.CARD) {
                continue;
            }

            ItemInfo existItemInfo = GetItem(itemInfo.EntityID);
            if (existItemInfo != null) {
                // 物品已存在，只加数量
                existItemInfo.Number += itemInfo.Number;
                existItemInfo.AwardTime = Time.realtimeSinceStartup;
            } else {
                // 只有新增的物品才会被标识为新物品
                itemInfo.IsNewItem = isNewItem;
                itemInfo.AwardTime = Time.realtimeSinceStartup;
                ItemList.Add(itemInfo);
                if (isNewItem) {
                    hasNewItem = true;
                }
            }
        }

        // 有新物品，显示背包提示
        if (hasNewItem) {
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_NEW_FLAG, GameFunc.BAG, true);
        }
    }

    // 有一个界面提示获得新物品
    public void AddItemWithUI(PComItemList ret)
    {
        List<ItemInfo> list = new List<ItemInfo>();

        foreach (var item in ret.item) {
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.Deserialize(item);
            if (!item.isFixGet) {
                list.Add(itemInfo);
            }
        }

        UIManager.Instance.OpenWindow<UIGetItemView>(list);

        // 添加物品
        AddItem(ret, true);
    }

    // 重置新物品标识
    public void ResetNewItemFlag()
    {
        foreach (var item in ItemList) {
            item.IsNewItem = false;
        }
    }

    // 是否有新物品
    public bool HasNewItem()
    {
        foreach (var item in ItemList) {
            if (item.IsNewItem) {
                return true;
            }
        }
        return false;
    }

    // 请求购买资源
    public void RequestBuyRes(ResourceType retType, int cost, int resValue)
    {
        PChance data = new PChance();
        data.num = cost;
        data.priceType = ePriceType.DIAMOND;

        switch (retType) {
            case ResourceType.MONEY:
                data.chance = eChance.CHAN_GOLD;
                break;
            case ResourceType.WOOD:
                data.chance = eChance.CHAN_WOOD;
                break;
            case ResourceType.STONE:
                data.chance = eChance.CHAN_STONE;
                break;
        }

        NetworkManager.Instance.Send(eCommand.BUY_CHANCE, data, (buffer) =>
        {
            PChance ret = Net.Deserialize<PChance>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.BUY_CHANCE)) return;

            switch (ret.chance) {
                case eChance.CHAN_GOLD:
                    // 购买银两
                    Money += ret.num;
                    CostMoney(cost, PriceType.GOLD);
                    break;
                case eChance.CHAN_WOOD:
                    // 购买木材
                    Wood += ret.num;
                    CostMoney(cost, PriceType.GOLD);
                    break;
                case eChance.CHAN_STONE:
                    // 购买石材
                    Stone += ret.num;
                    CostMoney(cost, PriceType.GOLD);
                    break;
            }

            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });
    }

    public HeroInfo GetHeroByItem(long itemID)
    {
        foreach (var item in HeroList) {
            if (item.GetItem(itemID) != null) {
                return item;
            }
        }
        return null;
    }

    // 设置阵型
    public void ReqSetFormation()
    {
        PSetLineUp data = new PSetLineUp();
        data.lineType = eLineUpType.BT_PVE;
        int index = 0;
        foreach (var item in PVEHeroList) {
            ++index;
            data.positionHero.Add(new PPosionInfo
            {
                pos = index,
                arg = item.EntityID,
            });
        }
        Net.Send(eCommand.SET_LINE_UP, data, (buffer) =>
        {
            CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
            if (!Net.CheckErrorCode(ret.err_code, eCommand.SET_LINE_UP)) return;
        });
    }

    // 获取随机名字
    public string GetRandomName()
    {
        List<string> prefixName = new List<string>();
        List<string> suffixName = new List<string>();
        Language language = LocalizationManager.Instance.Language;
        foreach (var item in RandomNameConfigLoader.Data) {
            List<string> list = null;
            if (item.Value.Type == 0) {
                // 名字前缀
                list = prefixName;
            } else if (item.Value.Type == 1) {
                // 名字后缀
                list = suffixName;
            }

            if (list != null) {
                switch (language) {
                    case Language.en:
                        list.Add(item.Value.Text_en);
                        break;
                    case Language.zhCN:
                        list.Add(item.Value.Text_zhCN);
                        break;
                    case Language.zhTW:
                        list.Add(item.Value.Text_zhCN);
                        break;
                }
            }
        }

        string prefix = prefixName[Random.Range(0, prefixName.Count - 1)];
        string suffix = suffixName[Random.Range(0, suffixName.Count - 1)];
        return prefix + suffix;
    }


    // 出售物品
    public void ReqSellItem(long itemID, int count)
    {
        PUseCommonItem data = new PUseCommonItem();
        data.comItem.Add(new PComItem
        {
            id = itemID,
            num = count,
        });

        Net.Send(eCommand.SALE_GOOD_TOSYS, data, (buffer) =>
        {
            CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
            if (!Net.CheckErrorCode(ret.err_code, eCommand.SALE_GOOD_TOSYS)) return;

            ItemInfo info = GetItem(itemID);
            if (info != null) {
                AddMoney(info.Cfg.Price * count, PriceType.MONEY);
            }
            UseItem(itemID, count);

            UIManager.Instance.RefreshWindow<UINewBagView>();
        });
    }

    // 使用物品
    public void ReqUseItem(long itemID)
    {
        PUseCommonItem data = new PUseCommonItem();
        data.comItem.Add(new PComItem {
            id = itemID,
            num = 1,
        });

        Net.Send(eCommand.USE_PROP_TO_GAINGOOD, data, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.USE_PROP_TO_GAINGOOD)) return;

            UseItem(itemID, 1);

            AddItem(ret, true);

            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
            UIManager.Instance.RefreshWindow<UINewBagView>();
        });
    }
}
