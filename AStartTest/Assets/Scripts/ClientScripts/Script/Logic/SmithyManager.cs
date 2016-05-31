using System;
using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

// 铁匠铺管理，铁匠铺是城镇建设的一部分
public class SmithyManager
{
    public static readonly SmithyManager Instance = new SmithyManager();

    public class SmithyItemInfo
    {
        public int CfgID;
        public int Count;

        public SmithyItemInfo(int cfgID, int count)
        {
            CfgID = cfgID;
            Count = count;
        }
    }

    public class SmithyCost
    {
        public int Money;
        public int Gold;
        public List<SmithyItemInfo> Material = new List<SmithyItemInfo>();
        public List<ItemInfo> Mold = new List<ItemInfo>();
    }

    //  请求锻造装备
    public void RequestCombineEquip(int destCfgID, List<ItemInfo> itemList, bool isLuck, SmithyCost cost)
    {
        List<long> list = new List<long>();
        foreach (var item in itemList) {
            list.Add(item.EntityID);
        }
        PCombineEquip data = new PCombineEquip();
        data.destId = destCfgID;
        data.modelId.AddRange(list);
        data.type = isLuck ? eCombineType.COMBINE_LUCK : eCombineType.COMBINE_GOLD;

        Net.Send(eCommand.COMBINE_EQUIP, data, (buffer) =>
        {
            PComItem ret = Net.Deserialize<PComItem>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.COMBINE_EQUIP)) return;

            ItemInfo info = new ItemInfo();
            info.Deserialize(ret);
            UserManager.Instance.AddItem(info, true);

            // 刷新锻造铺
            UIManager.Instance.RefreshWindow<UISmithyView>();
            UIManager.Instance.OpenWindow<UISmithyGetArmsView>(info);

            UserManager.Instance.Money -= cost.Money;
            UserManager.Instance.Gold -= cost.Gold;
            foreach (var item in cost.Mold) {
                ItemInfo old = UserManager.Instance.GetItem(item.EntityID);
                if (old != null) {
                    UserManager.Instance.RemoveItem(old);
                }
            }

            foreach (var item in cost.Material) {
                if (item.CfgID == GameConfig.ITEM_CONFIG_ID_WOOD) {
                    UserManager.Instance.Wood -= item.Count;
                } else if (item.CfgID == GameConfig.ITEM_CONFIG_ID_STONE) {
                    UserManager.Instance.Stone -= item.Count;
                } else {
                    UserManager.Instance.UseItemByConfigID(item.CfgID, item.Count);
                }
            }

            EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_REFRESH_VALUE);
        });
    }
    
    // 选择模具
    public List<ItemInfo> SelectModelEquip(int equipCfgID)
    {
        List<ItemInfo> ret = new List<ItemInfo>();

        ItemsConfig cfg = ItemsConfigLoader.GetConfig(equipCfgID);
        int demandCount = 0;

        if (cfg.Type < (int) ItemType.BOOK) {
            EquipmentConfig cfgEquip = EquipmentConfigLoader.GetConfig(equipCfgID);
            demandCount = cfgEquip.MoldDemand;

            // 遍历所有装备，获取比当前装备低一级的所有装备(低一级是底线，可以等级更高)
            foreach (var item in UserManager.Instance.ItemList) {
                if (item.Cfg.Type < (int)ItemType.BOOK) {
                    EquipmentConfig itemCfg = EquipmentConfigLoader.GetConfig(item.ConfigID);
                    if (itemCfg.Level >= cfgEquip.Level - 1 && itemCfg.Type == cfgEquip.Type) {
                        ret.Add(item);
                    }
                }
            }
        } else {
            BingfaConfig cfgBook = BingfaConfigLoader.GetConfig(equipCfgID);
            demandCount = cfgBook.MoldDemand;

            // 遍历所有装备，获取比当前装备低一级的所有装备(低一级是底线，可以等级更高)
            foreach (var item in UserManager.Instance.ItemList) {
                if (item.Cfg.Type == (int)ItemType.BOOK) {
                    BingfaConfig itemCfg = BingfaConfigLoader.GetConfig(item.ConfigID);
                    if (itemCfg.Level >= cfgBook.Level - 1 && itemCfg.Type == cfgBook.Type) {
                        ret.Add(item);
                    }
                }
            }
        }
        

        SortItem(ret);

        if (demandCount < ret.Count) {
            return ret.GetRange(0, demandCount);
        } else {
            return ret;
        }
    }

    // 选择模具兵法
    public List<ItemInfo> SelectModelBook(int bookCfgID)
    {
        List<ItemInfo> ret = new List<ItemInfo>();
        BingfaConfig cfg = BingfaConfigLoader.GetConfig(bookCfgID);
        foreach (var item in UserManager.Instance.ItemList) {
            if (item.IsBook()) {
                BingfaConfig itemCfg = BingfaConfigLoader.GetConfig(item.ConfigID);
                if (itemCfg.Level >= cfg.Level - 1) {
                    ret.Add(item);
                }
            }
        }

        SortItem(ret);

        if (cfg.MoldDemand < ret.Count) {
            return ret.GetRange(0, cfg.MoldDemand);
        } else {
            return ret;
        }
    }

    public void SortItem(List<ItemInfo> list)
    {
        list.Sort((a, b) => {
            if (a.Cfg.Level != b.Cfg.Level) {
                // 先比等级
                return a.Cfg.Level.CompareTo(b.Cfg.Level);
            } else if (a.Cfg.Quality != b.Cfg.Quality) {
                // 再比品质
                return a.Cfg.Quality.CompareTo(b.Cfg.Quality);
            } else {
                // 再比属性
                return a.GetScore().CompareTo(b.GetScore());
            }
        });
    }

    // 解析需要的材料
    public List<SmithyItemInfo> ParseItems(string text)
    {
        List<SmithyItemInfo> ret = new List<SmithyItemInfo>();
        if (string.IsNullOrEmpty(text)) return ret;

        string[] splitText = text.Split(';');
        foreach (var item in splitText) {
            int index = item.IndexOf("-");
            int id = System.Convert.ToInt32(item.Substring(0, index));
            int count = System.Convert.ToInt32(item.Substring(index + 1));

            ret.Add(new SmithyItemInfo(id, count));
        }

        return ret;
    }

    // 基础属性的锻造范围
    public string GetBaseAttr(int cfgID)
    {
        EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(cfgID);
        List<int> list = new List<int>();

        ParseAttr(list, cfg.WhiteLow);
        ParseAttr(list, cfg.GreenLower);
        ParseAttr(list, cfg.BlueLower);
        ParseAttr(list, cfg.PurpleLower);
        ParseAttr(list, cfg.OrangeLower);

        list.Sort();
        return string.Format("{0}+ {1}-{2}", ItemInfo.GetAttrName(cfg.BasicType), list[0], list[list.Count - 1]);
    }

    private void ParseAttr(List<int> list, string txt)
    {
        string[] values = txt.Split('-');
        foreach (var item in values) {
            list.Add(Convert.ToInt32(item));
        }
    }

    public string GetAddAttr(string txt)
    {
        string[] values = txt.Split('-');
        int attrType = Convert.ToInt32(values[0]);
        int attrValue = Convert.ToInt32(values[1]);

        if (attrType == 1 || attrType == 2 || attrType == 3) {
            // 这几个是数值
            return string.Format("{0}+ {1}", ItemInfo.GetAttrName(attrType), attrValue);
        } else {
            // 这几个是百分比
            return string.Format("{0}+ {1}%", ItemInfo.GetAttrName(attrType), attrValue);
        }
    }
}
