using UnityEngine;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using comrt.comnet;

// 商店类型
public enum ShopType
{
    SUNDRY = 1, // 杂货店需要金币
    BLACK_MARKET = 2,   // 黑市需要钻石
    PVP = 3,    // pvp商店需要积分
    GUILD = 4,  // 公会商店需要贡献值
}

// 商店中的物品信息
public class ShopItemInfo
{
    public int EntityID;               // 无用
    public int ConfigID;                   // 刷新表的配置ID
    public int ItemConfigID;            // 物品的配置ID
    public int ContainNum;              // 单位数量
    public int Price;                   // 购买价格
    public ShopType PriceType;         // 价格类型
    public bool IsSoldOut;              //是否售空
    public string Name;
    public int Quality;
    public bool IsFixGet;   // 传递给服务器，用来区分两个配表中的物品

    public void Deserialize(PGeneralGood data)
    {
        EntityID = data.id;
        ConfigID = data.id;
        ContainNum = data.num;
        IsSoldOut = !data.isCanBuy;
        IsFixGet = data.isFixGet;

        if (data.isFixGet) {
            ShopFixGoodConfig cfg = ShopFixGoodConfigLoader.GetConfig(ConfigID);
            ItemConfigID = cfg.ItemId;
            ItemsConfig itemCfg = ItemsConfigLoader.GetConfig(ItemConfigID);
            if (itemCfg != null) {
                Name = itemCfg.Name;
                Quality = itemCfg.Quality;
                Price = cfg.Price * ContainNum;
            }
        } else {
            ShopConstConfig cfg = ShopConstConfigLoader.GetConfig(ConfigID);
            ItemConfigID = cfg.ItemId;
            ItemsConfig itemCfg = ItemsConfigLoader.GetConfig(ItemConfigID);
            if (itemCfg != null) {
                Name = itemCfg.Name;
                Quality = itemCfg.Quality;
                Price = cfg.Price * ContainNum;
            }
        }
    }
}


public class ShopInfo
{
    public ShopType shopType;
    public int refreshCount = 0;    // 手动刷新次数，根据这个值计算元宝消耗
    public RemainTime refreshCD;
    public List<ShopItemInfo> itemList = new List<ShopItemInfo>();

    public bool hasRequest = false; // 是否向服务器请求过数据，防止切换界面的时候频繁请求数据

    // 获取此商店刷新消耗的元宝
    public int GetRefreshCost()
    {
        return (int)(GameConfig.SHOP_REFRESH_COST * Mathf.Pow(GameConfig.SHOP_REFRESH_MULTIPLE, refreshCount));
    }
}

// 商店数据管理
public class ShopManager
{
    public static readonly ShopManager Instance = new ShopManager();

    public Dictionary<ShopType, ShopInfo> ShopInfoData = new Dictionary<ShopType, ShopInfo>();

    public int MoneyFreeCount;  // 银两免费次数
    public float MoneyNextFreeTime; // 银两免费倒计时
    public float MoneyNextFreeSyncTime; // 同步时间

    public int GoldLeftCount;   // 元宝还有几次获得英雄
    public float GoldNextFreeTime;  // 元宝下次免费倒计时
    public float GoldNextFreeSyncTime;  // 同步时间

    // 请求商店信息
    public void RequestShopInfo(ShopType shopType)
    {
        PCMInt data = new PCMInt();
        data.arg = (int)shopType;

        NetworkManager.Instance.Send(eCommand.GET_PLAYER_SHOP_LIST, data, (buffer) => {
            PGeneralGoodList ret = Net.Deserialize<PGeneralGoodList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_PLAYER_SHOP_LIST)) return;

            OnRefreshShopInfo(ret, shopType);
        });
    }

    private void OnRefreshShopInfo(PGeneralGoodList ret, ShopType shopType)
    {
        ShopInfo shopInfo = null;
        if (!ShopInfoData.TryGetValue(shopType, out shopInfo)) {
            shopInfo = new ShopInfo();
            ShopInfoData[shopType] = shopInfo;
        }

        shopInfo.refreshCD.SetTimeMilliseconds(ret.nextAutoRefreshTime);
        shopInfo.refreshCount = ret.refreshTime;

        shopInfo.itemList.Clear();
        foreach (var item in ret.GeneralGoodList) {
            ShopItemInfo info = new ShopItemInfo();
            info.Deserialize(item);
            info.PriceType = shopType;
            shopInfo.itemList.Add(info);
        }

        EventDispatcher.TriggerEvent(EventID.EVENT_SHOP_REFRESH_SHOP, shopType);
    }

    // 获取商店数据
    public ShopInfo GetShopInfo(ShopType shopType)
    {
        ShopInfo info = null;
        ShopInfoData.TryGetValue(shopType, out info);
        return info;
    }

    public ShopItemInfo ShopItemInfo(ShopType shopType, int shopItemID, bool isFixGet)
    {
        ShopInfo info = null;
        if (ShopInfoData.TryGetValue(shopType, out info)) {
            return info.itemList.Find(x => { return x.ConfigID == shopItemID && x.IsFixGet == isFixGet; });
        }

        return null;
    }

    public int GetMoney(ShopType shopType)
    {
        switch (shopType) {
            case ShopType.SUNDRY:
                return UserManager.Instance.Money;
            case ShopType.BLACK_MARKET:
                return UserManager.Instance.Gold;
            case ShopType.PVP:
                return UserManager.Instance.Money;
            case ShopType.GUILD:
                return UserManager.Instance.Money;
        }
        return 0;
    }

    public void CostMoney(int price, ShopType shopType)
    {
        switch (shopType) {
            case ShopType.SUNDRY:
                UserManager.Instance.CostMoney(price, PriceType.MONEY);
                break;
            case ShopType.BLACK_MARKET:
                UserManager.Instance.CostMoney(price, PriceType.GOLD);
                break;
            case ShopType.PVP:
                break;
            case ShopType.GUILD:
                break;
        }
    }

    // 购买物品
    public void RequestBuyItem(ShopType shopType, int shopItemID, bool isFixGet)
    {
        ShopItemInfo info = ShopItemInfo(shopType, shopItemID, isFixGet);
        if (info == null) return;

        if (GetMoney(info.PriceType) < info.Price) {
            UIUtil.ShowMsgFormat("MSG_SHOP_MONEY_LIMIT");
            return;
        }

        PBuyGood data = new PBuyGood();
        data.shopType = (int)shopType;
        data.goodId = shopItemID;
        data.isFix = isFixGet;

        NetworkManager.Instance.Send(eCommand.BUY_SHOP_GOOD, data, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.BUY_SHOP_GOOD)) return;

            info.IsSoldOut = true;
            UserManager.Instance.AddItemWithUI(ret);
            CostMoney(info.Price, info.PriceType);
            UIManager.Instance.RefreshWindow<UINewBagView>();
            UIManager.Instance.RefreshWindow<UIShopView>();
            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);

        });
    }

    // 请求手动刷新商店
    public void RequestRefreshShop(ShopType shopType)
    {
        ShopInfo info = GetShopInfo(shopType);
        if (info == null) return;

        int costGold = info.GetRefreshCost();
        if (UserManager.Instance.Gold < costGold) {
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_GOLD_LIMIT");
            return;
        }

        PCMInt data = new PCMInt();
        data.arg = (int)shopType;

        NetworkManager.Instance.Send(eCommand.MANAUL_REFRESH_SHOP, data, (buffer) => {
            PGeneralGoodList ret = Net.Deserialize<PGeneralGoodList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.MANAUL_REFRESH_SHOP)) return;

            // 减少金钱
            UserManager.Instance.CostMoney(costGold, PriceType.GOLD);

            // 增加刷新次数
            ++info.refreshCount;

            OnRefreshShopInfo(ret, shopType);
        });
    }

    // 获取酒馆抽奖的信息
    public void RequestTavernBuyInfo()
    {
        Net.Send(eCommand.GET_LUCK_DRAW_INFO);
    }

    // 获取免费银两抽奖的倒计时
    public float GetMoneyFreeCD()
    {
        if (MoneyNextFreeTime <= 0) return 0;

        return Mathf.Max(0, MoneyNextFreeTime - (Time.realtimeSinceStartup - MoneyNextFreeSyncTime));
    }

    // 获取元宝抽奖的倒计时
    public float GetGoldFreeCD()
    {
        if (GoldNextFreeTime <= 0) return 0;

        return Mathf.Max(0, GoldNextFreeTime - (Time.realtimeSinceStartup - GoldNextFreeSyncTime));
    }

    private void OnTavernBuyOK(PComItemList ret, int itemID, int itemCount, TavernBuyAction buyAction)
    {
        // ui显示
        Debug.Log("OnTavernBuyOK");

        List<ItemInfo> list = new List<ItemInfo>();

        bool needRequestBagList = false;
        bool needRequestHeroList = false;

        foreach (var item in ret.item) {
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.Deserialize(item);


            if (itemInfo.Cfg.Type == (int)ItemType.CARD) {
                // 如果是英雄卡的话
                HeroInfo heroInfo = UserManager.Instance.GetHeroInfoByUnitID(itemInfo.Cfg.MatchHero);
                if (heroInfo != null) {
                    // 转换为灵魂石
                    //                    needRequestBagList = true;
                    //                    ItemInfo souldInfo = new ItemInfo();
                    //                    souldInfo.ConfigID = heroInfo.Cfg.Cost;
                    //                    souldInfo.Number = itemInfo.Cfg.SoulStoneNum;
                    //                    list.Add(itemInfo);
                } else {
                    list.Add(itemInfo);

                    // 获得新英雄
                    needRequestHeroList = true;
                }
            } else {
                if (!item.isFixGet) {
                    list.Add(itemInfo);
                }
            }
        }

        UIManager.Instance.OpenWindow<UITavernGetItemView>(list, itemID, itemCount, buyAction);

        // 添加物品
        UserManager.Instance.AddItem(ret, true);

        if (needRequestHeroList) {
            UserManager.Instance.RequestUserInfo();
        }
    }

    private void RefreshUI()
    {
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        UIManager.Instance.RefreshWindow<UITavernView>();
    }

    // 酒馆抽卡，银两免费
    public void RequestTavernMoneyFreeBuy()
    {
        NetworkManager.Instance.Send(eCommand.FREE_GOLD_LUCK_DRAW, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.FREE_GOLD_LUCK_DRAW)) return;

            OnTavernBuyOK(ret, GameConfig.LUCK_DRAW_MONEY_ITEM_ID, 1, TavernBuyAction.BUY_MONEY_1);

            // 更新免费次数和倒计时
            MoneyFreeCount = Mathf.Max(0, MoneyFreeCount - 1);
            MoneyNextFreeTime = GameConfig.LUCK_DRAW_MONEY_FREE_CD;
            MoneyNextFreeSyncTime = Time.realtimeSinceStartup;

            RefreshUI();
        });
    }

    // 酒馆抽卡，银两1次
    public void RequestTavernMoneyBuy1()
    {
        NetworkManager.Instance.Send(eCommand.SIGLE_GOLD_LUCK_DRAW, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.SIGLE_GOLD_LUCK_DRAW)) return;

            OnTavernBuyOK(ret, GameConfig.LUCK_DRAW_MONEY_ITEM_ID, 1, TavernBuyAction.BUY_MONEY_1);

            UserManager.Instance.CostMoney(GameConfig.LUCK_DRAW_MONEY_1_COST, PriceType.MONEY);

            RefreshUI();
        });
    }

    // 酒馆抽卡，银两10次
    public void RequestTavernMoneyBuy10()
    {
        NetworkManager.Instance.Send(eCommand.TEN_CONTINUE_GOLD_LUCK_DRAW, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.TEN_CONTINUE_GOLD_LUCK_DRAW)) return;

            OnTavernBuyOK(ret, GameConfig.LUCK_DRAW_MONEY_ITEM_ID, 10, TavernBuyAction.BUY_MONEY_10);
            UserManager.Instance.CostMoney(GameConfig.LUCK_DRAW_MONEY_10_COST, PriceType.MONEY);

            RefreshUI();
        });
    }

    // 酒馆抽卡，元宝免费
    public void RequestTavernGoldFreeBuy()
    {
        NetworkManager.Instance.Send(eCommand.FREE_DIAMOND_LUCK_DRAW, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.FREE_DIAMOND_LUCK_DRAW)) return;

            OnTavernBuyOK(ret, GameConfig.LUCK_DRAW_GOLD_ITEM_ID, 1, TavernBuyAction.BUY_GOLD_1);

            GoldLeftCount = Mathf.Max(0, GoldLeftCount - 1);
            GoldNextFreeTime = GameConfig.LUCK_DRAW_GOLD_FREE_CD;
            GoldNextFreeSyncTime = Time.realtimeSinceStartup;

            RefreshUI();
        });
    }

    // 酒馆抽卡，元宝1次
    public void RequestTavernGoldBuy1()
    {
        NetworkManager.Instance.Send(eCommand.SIGLE_DIAMOND_LUCK_DRAW, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.SIGLE_DIAMOND_LUCK_DRAW)) return;

            OnTavernBuyOK(ret, GameConfig.LUCK_DRAW_GOLD_ITEM_ID, 1, TavernBuyAction.BUY_GOLD_1);
            UserManager.Instance.CostMoney(GameConfig.LUCK_DRAW_GOLD_1_COST, PriceType.GOLD);

            GoldLeftCount = Mathf.Max(0, GoldLeftCount - 1);

            RefreshUI();
        });
    }

    // 酒馆抽卡，元宝10次
    public void RequestTavernGoldBuy10()
    {
        NetworkManager.Instance.Send(eCommand.TEN_CONTINUE_DIAMOND_LUCK_DRAW, (buffer) => {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.TEN_CONTINUE_DIAMOND_LUCK_DRAW)) return;

            OnTavernBuyOK(ret, GameConfig.LUCK_DRAW_GOLD_ITEM_ID, 10, TavernBuyAction.BUY_GOLD_10);
            UserManager.Instance.CostMoney(GameConfig.LUCK_DRAW_GOLD_10_COST, PriceType.GOLD);

            RefreshUI();
        });
    }

    public void RegisterMsg()
    {
        Net.Register(eCommand.GET_LUCK_DRAW_INFO, OnMsgGetLuckDrawInfo);
    }

    private void OnMsgGetLuckDrawInfo(byte[] buffer)
    {
        PLuckDrawList ret = Net.Deserialize<PLuckDrawList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_LUCK_DRAW_INFO)) return;

        PLuckDraw infoMoney = null;
        PLuckDraw infoGold = null;

        foreach (var item in ret.luckDraw) {
            if (item.type == eLuckDraw.GOLD_LUCKDRAW) {
                infoMoney = item;
            } else if (item.type == eLuckDraw.DIAMOND_LUCKDRAW) {
                infoGold = item;
            }
        }

        if (infoMoney == null || infoGold == null) return;

        ShopManager.Instance.MoneyFreeCount = GameConfig.LUCK_DRAW_MAX_FREE_COUNT - infoMoney.usedCount;
        ShopManager.Instance.MoneyNextFreeTime = Utils.GetSeconds(infoMoney.nextFreeTime);
        ShopManager.Instance.MoneyNextFreeSyncTime = Time.realtimeSinceStartup;

        ShopManager.Instance.GoldLeftCount = GameConfig.LUCK_DRAW_GOLD_COUNT - infoGold.totalCount;
        ShopManager.Instance.GoldNextFreeTime = Utils.GetSeconds(infoGold.nextFreeTime);
        ShopManager.Instance.GoldNextFreeSyncTime = Time.realtimeSinceStartup;

        UIManager.Instance.RefreshWindow<UITavernView>();
    }
}
