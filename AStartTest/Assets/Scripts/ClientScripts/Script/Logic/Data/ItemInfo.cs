using UnityEngine;
using System.Collections.Generic;
using System.Text;
using comrt.comnet;

// 物品类型
public enum ItemType
{
    NONE = 0,
    SWORD = 1,
    SPEAR = 2,
    BOW = 3,
    FAN = 4,
    CLOTH = 5,
    CHEST = 6,
    DECORATION = 7,
    BOOK = 8,
    SOUL_PIECE = 9, // 灵魂石
    MONEY = 10, // 货币
    CARD = 11,  // 英雄卡
    OTHER = 1000,
}

// 商品状态标签
public enum ShopItemFlag
{
    UNKNOW14 = 60,
    GOOD_COMMON = 1,        // 普通
    GOOD_HOT = 2,           // 热门
    GOOD_VIP = 3,           // vip
    GOOD_NEW = 4,           // 新品
    GOOD_LIMIT_TIME = 5,    // 限时
    GOOD_LIMIT_NUM = 6,     // 限量
    GOOD_DISCOUNT = 7,      // 折扣
}

// 货币类型
public enum PriceType {
   UNKNOW12                       = 50,
   GOLD                        = 1, //钻石 (Gold 元宝)
   MONEY                           = 2, //黄金 (Money 银两)
   WOOD                           = 3, //木材
   STONE                          = 4, //石头
   PROPS                          = 5, //道具 放入背包的东东
   ARENACURRENCY                  = 6, //竞技场货币
   EXPEDITION                     = 7, //远征货币
}

// 装备数据
public class ItemInfo
{
    public long EntityID;
    public int Number;
    public int ConfigID;
    public float AwardTime;
    public bool IsNewItem;
    public int Quality;


    // 一级属性（兵法增加）
    public int Strength;    // 力量
    public int Intelligence;    // 智力
    public int LeaderShip;  // 统帅

    // 二级属性
    public int Attack;  // 攻击
    public int MagicAttack; // 法术伤害
    public int Hp;  // 生命
    public float Def; // 伤害减免
    public float Critical;    // 暴击
    public float HpSorb;  // 吸血
    public float Stum;    // 重击
    public int AttackSpeed; // 攻击速度
    public float Cooldown;   // 冷却缩减

    public void Deserialize(PComItem data)
    {
        EntityID = data.id;
        Number = data.num;
        ConfigID = data.cfgId;
        Quality = data.quality;
        
        // 一级属性
        if (data.basicAttr != null) {
            Strength = data.basicAttr.strength;
            Intelligence = data.basicAttr.Intelligence;
            LeaderShip = data.basicAttr.LeaderShip;
        }

        // 二级属性
        if (data.battleAttr != null) {
            Attack = data.battleAttr.physAtk;
            MagicAttack = data.battleAttr.magicAtk;
            Hp = data.battleAttr.hp;
            Def = data.battleAttr.damReduce;
            Critical = data.battleAttr.criticStrike;
            HpSorb = data.battleAttr.suckBlood;
            Stum = data.battleAttr.damDeepen;
            AttackSpeed = data.battleAttr.atkSpeed;
            Cooldown = data.battleAttr.coolDown;
        }
    }

    private ItemsConfig _config;
    public ItemsConfig Cfg
    {
        get
        {
            if (_config == null) {
                _config = ItemsConfigLoader.GetConfig(ConfigID);
            }
            return _config;
        }
    }

    public ItemInfo Clone()
    {
        ItemInfo info = new ItemInfo();
        info.EntityID = EntityID;
        info.Number = Number;
        info.ConfigID = ConfigID;
        return info;
    }

    // 获取物品品质名字
    public static string GetQualityName(int quality)
    {
        switch (quality) {
            case 1:
                return Str.Get("QUALITY_WHITE");
            case 2:
                return Str.Get("QUALITY_GREEN");
            case 3:
                return Str.Get("QUALITY_BLUE");
            case 4:
                return Str.Get("QUALITY_PINK");
            case 5:
                return Str.Get("QUALITY_ORANGE");
        }
        return "";
    }

    // 获取物品类型名字
    public static string GetItemTypeName(int itemType)
    {
        return Str.Get("ITEM_TYPE_" + itemType);
    }

    // 获取属性的名字（根据配置中的数字）
    public static string GetAttrName(int attr)
    {
        switch (attr) {
            case 1:
                return Str.Get("ATTR_ATTACK");
            case 2:
                return Str.Get("ATTR_MAGIC_ATTACK");
            case 3:
                return Str.Get("ATTR_HP");
            case 4:
                return Str.Get("ATTR_DEF");
            case 5:
                return Str.Get("ATTR_CRIT");
            case 6:
                return Str.Get("ATTR_HP_SORB");
            case 7:
                return Str.Get("ATTR_STUM");
            case 8:
                return Str.Get("ATTR_ATTACK_SPEED");
            case 9:
                return Str.Get("ATTR_CD");
        }
        return "";
    }
    
    private bool CheckAttr(int baseAttr, int attr, bool showBase)
    {
        return showBase ? baseAttr == attr : baseAttr != attr;
    }
    
    public string GetAttrDesc(int index, int baseAttr, bool showBase)
    {
        int curIndex = 0;
        if (Strength > 0 && CheckAttr(baseAttr, 0, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}", Str.Get("ATTR_STR"), Strength);
        }

        if (Intelligence > 0 && CheckAttr(baseAttr, 0, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}", Str.Get("ATTR_INT"), Intelligence);
        }

        if (LeaderShip > 0 && CheckAttr(baseAttr, 0, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}", Str.Get("ATTR_LEADER"), LeaderShip);
        }

        if (Attack > 0 && CheckAttr(baseAttr, 1, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}", Str.Get("ATTR_ATTACK"), Attack);
        }

        if (MagicAttack > 0 && CheckAttr(baseAttr, 2, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}", Str.Get("ATTR_MAGIC_ATTACK"), MagicAttack);
        }

        if (Hp > 0 && CheckAttr(baseAttr, 3, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}", Str.Get("ATTR_HP"), Hp);
        }

        if (Def > 0 && CheckAttr(baseAttr, 4, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}%", Str.Get("ATTR_DEF"), Def);
        }

        if (Critical > 0 && CheckAttr(baseAttr, 5, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}%", Str.Get("ATTR_CRIT"), Critical);
        }

        if (HpSorb > 0 && CheckAttr(baseAttr, 6, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}%", Str.Get("ATTR_HP_SORB"), HpSorb);
        }

        if (Stum > 0 && CheckAttr(baseAttr, 7, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}%", Str.Get("ATTR_STUM"), Stum);
        }

        if (AttackSpeed > 0 && CheckAttr(baseAttr, 8, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}%", Str.Get("ATTR_ATTACK_SPEED"), AttackSpeed);
        }

        if (Cooldown > 0 && CheckAttr(baseAttr, 9, showBase) && curIndex++ == index) {
            return string.Format("{0}+{1}%", Str.Get("ATTR_CD"), Cooldown);
        }

        return "";
    }

    // 获取装备的评分
    public int GetScore()
    {
        return (int) (Strength + Intelligence + LeaderShip + Attack + MagicAttack + Hp + Def + Critical + HpSorb + Stum + AttackSpeed + AttackSpeed + Cooldown);
    }

    // 是否是经验球
    public bool IsExpBall()
    {
        return ConfigID == GameConfig.ITEM_CONFIG_ID_EXP_1
            || ConfigID == GameConfig.ITEM_CONFIG_ID_EXP_2
            || ConfigID == GameConfig.ITEM_CONFIG_ID_EXP_3
            || ConfigID == GameConfig.ITEM_CONFIG_ID_EXP_4
            || ConfigID == GameConfig.ITEM_CONFIG_ID_EXP_5;
    }

    // 是否是装备
    public bool IsEquip()
    {
        if (Cfg.Type <= (int)ItemType.BOOK) {
            return true;
        } else {
            return false;
        }
    }

    // 是否是兵法
    public bool IsBook()
    {

        ItemType itemType = (ItemType)Cfg.Type;
        return itemType == ItemType.BOOK;
    }

    // 是否是英雄卡
    public bool IsCard()
    {
        return Cfg.Type == (int)ItemType.CARD;
    }

    // 是否能够使用
    public bool CouldUse()
    {
        return Cfg.Enable == 1;
    }
}
