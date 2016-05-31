
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ConfigDataBase {
    public ConfigDataBase() {}
    public ConfigDataBase(JsonData jdata) { fromJson(jdata);  }
    public void fromJson(JsonData jdata) {
        System.Reflection.FieldInfo[] fieldInfo = GetType().GetFields();
        foreach (var p in fieldInfo) {
            if (!JsonUtil.HasJsonKey(jdata, p.Name) || JsonUtil.IsValueEmpty(jdata[p.Name])) {
                continue;
            }

            if (p.FieldType == typeof(System.Single)) {
                p.SetValue(this, JsonUtil.Json2Float(jdata, p.Name));
            } else if (p.FieldType == typeof(System.Int32)) {
                p.SetValue(this, JsonUtil.Json2Int(jdata, p.Name));
            } else if (p.FieldType == typeof(System.String)) {
                p.SetValue(this, JsonUtil.Json2String(jdata, p.Name));
            } else if (p.FieldType == typeof(List<int>)) {
                p.SetValue(this, JsonUtil.Json2IntList(jdata, p.Name));
            } else if (p.FieldType == typeof(List<float>)) {
                p.SetValue(this, JsonUtil.Json2FloatList(jdata, p.Name));
            } else if (p.FieldType == typeof(List<string>)) {
                p.SetValue(this, JsonUtil.Json2StringList(jdata, p.Name));
            } else if (p.FieldType == typeof(Color)) {
                p.SetValue(this, JsonUtil.Json2Color(jdata, p.Name, Color.white));
            } else if (p.FieldType == typeof(bool)) {
                p.SetValue(this, JsonUtil.Json2Boolean(jdata, p.Name));
            } else if (p.FieldType == typeof(long)) {
                p.SetValue(this, JsonUtil.Json2Long(jdata, p.Name));
            } else if (p.FieldType == typeof(Translation)) {
                p.SetValue(this, JsonUtil.Json2Translation(jdata, p.Name));
            } else {
                Debug.LogError("未知的属性: " + p.Name);
            }
        }
    }
}

public class ConfigDataLoaderHelper
{
    public static string GetText(string name)
    {
        UnityEngine.Object text = Resources.Load(name);
        if (text == null) {
            Debug.LogError(string.Format("config file not found: {0}", name));
            return "";
        }
        
        TextAsset txt = (TextAsset)GameObject.Instantiate(text);
        string data = txt.text;
        Resources.UnloadAsset(text);
        txt = null;
        return data;
    }
}

// 玩家初始化常量数据以及其他配置常量
public class ComConstConfig : ConfigDataBase
{
	public int initDiamond;					//初始钻石
	public int initWood;					//初始木材
	public int initStone;					//初始石头
	public int initGold;					//初始黄金
	public int intStamina;					//初始体力
	public int StaminaRecoverTime;					//每点体力恢复时间（ms）
	public int unlockPassiveSkill;					//星级解锁天赋技能
	public int unlockSkill2;					//等级解锁技能2
	public int unlockSkill3;					//等级解锁技能3
	public int unlockSkill4;					//等级解锁技能4
	public float intAttack;					//攻击力系数
	public float intAbilityPower;					//法术伤害系数
	public float intHp;					//HP系数
	public int intSkillPoint;					//初始技能点
	public int intSkillPointInterval;					//每点技能点恢复时间（ms）
	public int intProduceTime;					//建筑生产时间（ms）
	public int intProduceInterval;					//产出间隔时间（ms）
	public int ArenaChallengeTimes;					//竞技场初始挑战次数
	public int ArenaChallengeInterval;					//竞技场挑战间隔（ms）
	public int BuyResRatio;					//元宝购买资源比例
	public int OriginalHero;					//玩家初始英雄
	public int MissionBuyTimesCost;					//玩家购买精英副本次数花费元宝
	public ComConstConfig(JsonData jdata) : base(jdata) {}
}

public class ComConstConfigLoader
{
    static public Dictionary<int, ComConstConfig> Data = new Dictionary<int, ComConstConfig>();
    static ComConstConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ComConst"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ComConstConfig info = new ComConstConfig(jsondata[i]);
            Data[info.initDiamond] = info;
        }
    }

    static public ComConstConfig GetConfig(int key, bool logError = true)
    {
        ComConstConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ComConstConfig has no key: {0}", key));
        }
        return null;
    }
}


// 玩家等级
public class PlayerLevelConfig : ConfigDataBase
{
	public int Level;					//等级
	public int Exp;					//升级经验
	public int MaxStamina;					//体力上限
	public int HeroLevelLimit;					//英雄等级上限
	public int GiveStamina;					//升级赠送体力
	public int AttackCost;					//攻打消耗
	public int ManaLine;					//统御值上限
	public PlayerLevelConfig(JsonData jdata) : base(jdata) {}
}

public class PlayerLevelConfigLoader
{
    static public Dictionary<int, PlayerLevelConfig> Data = new Dictionary<int, PlayerLevelConfig>();
    static PlayerLevelConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/PlayerLevel"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            PlayerLevelConfig info = new PlayerLevelConfig(jsondata[i]);
            Data[info.Level] = info;
        }
    }

    static public PlayerLevelConfig GetConfig(int key, bool logError = true)
    {
        PlayerLevelConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("PlayerLevelConfig has no key: {0}", key));
        }
        return null;
    }
}


// 英雄升级表
public class HeroLevelConfig : ConfigDataBase
{
	public int Level;					//等级
	public int ExpRequire;					//升级所需经验
	public int TotalExp;					//累积等级经验
	public HeroLevelConfig(JsonData jdata) : base(jdata) {}
}

public class HeroLevelConfigLoader
{
    static public Dictionary<int, HeroLevelConfig> Data = new Dictionary<int, HeroLevelConfig>();
    static HeroLevelConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/HeroLevel"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            HeroLevelConfig info = new HeroLevelConfig(jsondata[i]);
            Data[info.Level] = info;
        }
    }

    static public HeroLevelConfig GetConfig(int key, bool logError = true)
    {
        HeroLevelConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("HeroLevelConfig has no key: {0}", key));
        }
        return null;
    }
}


// 技能升级消耗
public class UpgradeSkillConfig : ConfigDataBase
{
	public int SkillType;					//技能类型
	public int Cost;					//初始消耗金钱
	public int CostPerLevel;					//每级消耗金钱
	public UpgradeSkillConfig(JsonData jdata) : base(jdata) {}
}

public class UpgradeSkillConfigLoader
{
    static public Dictionary<int, UpgradeSkillConfig> Data = new Dictionary<int, UpgradeSkillConfig>();
    static UpgradeSkillConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/UpgradeSkill"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            UpgradeSkillConfig info = new UpgradeSkillConfig(jsondata[i]);
            Data[info.SkillType] = info;
        }
    }

    static public UpgradeSkillConfig GetConfig(int key, bool logError = true)
    {
        UpgradeSkillConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("UpgradeSkillConfig has no key: {0}", key));
        }
        return null;
    }
}


// 英雄配置表
public class HeroConfig : ConfigDataBase
{
	public int CfgId;					//英雄配置id
	public string HeroName;					//英雄名称
	public string HeroNamePath;					//英雄名称路径
	public string HeroIcon;					//英雄图标
	public string HeroImage;					//英雄头像
	public string HeroModel;					//英雄模型
	public int Skill1;					//技能1
	public int Skill2;					//技能2
	public int Skill3;					//技能3
	public int Skill4;					//技能4
	public int PassiveSkill;					//天赋技能
	public int AttackType;					//英雄类型  1 武  2 智   3 辅
	public int WeaponType;					//武器类型  1 刀  2 枪  3 弓  4 扇
	public int ArmourType;					//防具类型  5 布甲  6 重甲
	public int Star;					//初始星级
	public int Strength;					//武力
	public int Intelligence;					//智力
	public int Leadership;					//统帅
	public float Attack;					//物理攻击
	public float AbilityPower;					//法术攻击
	public int HP;					//血量
	public int Range;					//普攻范围
	public float MoveSpeed;					//移动速度
	public float AtkSpeed;					//攻击速度
	public float Crtical;					//暴击
	public int Cost;					//招募/升星  消耗
	public int CallCost;					//招募所需灵魂石数
	public float ComprehensiveAbility;					//综合能力
	public int PhysicalDamage;					//物理伤害
	public int AbilityDamage;					//法术伤害
	public int ViabilityPower;					//生存能力
	public int AuxiliaryPower;					//辅助能力
	public List<int> HeroLabel;					//英雄标签  1 近战  2 高伤害  3 肉盾  4 控制  5 治疗  6 脆皮  7 远程  8 英雄杀手  9辅助
	public HeroConfig(JsonData jdata) : base(jdata) {}
}

public class HeroConfigLoader
{
    static public Dictionary<int, HeroConfig> Data = new Dictionary<int, HeroConfig>();
    static HeroConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Hero"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            HeroConfig info = new HeroConfig(jsondata[i]);
            Data[info.CfgId] = info;
        }
    }

    static public HeroConfig GetConfig(int key, bool logError = true)
    {
        HeroConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("HeroConfig has no key: {0}", key));
        }
        return null;
    }
}


// 技能表
public class SkillSettingsConfig : ConfigDataBase
{
	public int SkillId;					//技能ID
	public string Name;					//技能名称
	public string Description;					//技能描述
	public string Icon;					//技能图标
	public float Level1Param1;					//一级参数1
	public float Level2Param1;					//二级参数1
	public float Level3Param1;					//三级参数1
	public float Level4Param1;					//四级参数1
	public float Level1Param2;					//一级参数2
	public float Level2Param2;					//二级参数2
	public float Level3Param2;					//三级参数2
	public float Level4Param2;					//四级参数2
	public SkillSettingsConfig(JsonData jdata) : base(jdata) {}
}

public class SkillSettingsConfigLoader
{
    static public Dictionary<int, SkillSettingsConfig> Data = new Dictionary<int, SkillSettingsConfig>();
    static SkillSettingsConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/SkillSettings"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            SkillSettingsConfig info = new SkillSettingsConfig(jsondata[i]);
            Data[info.SkillId] = info;
        }
    }

    static public SkillSettingsConfig GetConfig(int key, bool logError = true)
    {
        SkillSettingsConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("SkillSettingsConfig has no key: {0}", key));
        }
        return null;
    }
}


// 英雄升星
public class HeroStarUpgradeConfig : ConfigDataBase
{
	public int CfgId;					//英雄ID
	public int Star;					//星级
	public int Strength;					//武力值
	public int Intelligence;					//智力值
	public int Leadership;					//统帅值
	public int StrengthImprove;					//提升武力
	public int IntelligenceImprove;					//提升智力
	public int LeadershipImprove;					//提升统帅
	public int Cost;					//升星消耗
	public HeroStarUpgradeConfig(JsonData jdata) : base(jdata) {}
}

public class HeroStarUpgradeConfigLoader
{
    static public Dictionary<int, Dictionary<int, HeroStarUpgradeConfig>> Data = new Dictionary<int, Dictionary<int, HeroStarUpgradeConfig>>();
    static HeroStarUpgradeConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/HeroStarUpgrade"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            HeroStarUpgradeConfig info = new HeroStarUpgradeConfig(jsondata[i]);
            Dictionary<int, HeroStarUpgradeConfig> dict;
            if (!Data.TryGetValue(info.CfgId, out dict)) {
                dict = new Dictionary<int, HeroStarUpgradeConfig>();
                Data[info.CfgId] = dict;
            }
            dict[info.Star] = info;
        }
    }

    static public HeroStarUpgradeConfig GetConfig(int key, int key2, bool logError = true)
    {
        Dictionary<int, HeroStarUpgradeConfig> dict;
        if (Data.TryGetValue(key, out dict)) {
            HeroStarUpgradeConfig data;
            if (dict.TryGetValue(key2, out data)) {
                return data;
            }
        }

        if (logError) {
            Debug.LogError(string.Format("HeroStarUpgradeConfig has no key: {0}, {1}", key, key2));
        }
        return null;
    }
}


// 物品表
public class ItemsConfig : ConfigDataBase
{
	public int CfgId;					//物品配置
	public string Name;					//物品名称
	public string Icon;					//物品图标
	public int Type;					//物品类型  1 刀剑  2 枪矛  3 弓弩      4 羽扇      5 布甲     6 重甲   7 饰品  8 兵书  9 灵魂石  10 货币  11 英雄卡  1000 其他
	public int Enable;					//能否使用  0 否  1 是
	public string AddItem;					//使用后增加物品
	public int Combined;					//能否合成  0 否  1 是
	public string CombineNum;					//合成目标及所需数量
	public int ExpIncrease;					//增加经验
	public int ForSale;					//能否出售  0 否   1 是
	public int PriceType;					//价格类  1 元宝   2 黄金   3 木材   4 石材   5 其他
	public int Price;					//出售价格
	public int Quality;					//物品品质  1 白色   2 绿色   3 蓝色   4 紫色  5 橙色
	public int EquipLevel;					//装备等级
	public int Level;					//穿戴等级
	public int Attack;					//攻击力
	public int Hp;					//生命值
	public int DamageDecrease;					//伤害减免%
	public int Strength;					//武力
	public string Decription;					//物品描述
	public int WeightClass;					//权重分类  (金币抽)  1 60%  2 25%  3 9.9%  4 4%  5 1%  6 0.1%
	public int DropWeight;					//掉落权重
	public int WeightClass2;					//权重分类(钻石抽)  1 0%  2 0%  3 60%  4 25%  5 10%  6 5%
	public int DropWeight2;					//掉落权重
	public int MatchHero;					//对应英雄
	public int SoulStoneNum;					//转换灵魂石数量
	public ItemsConfig(JsonData jdata) : base(jdata) {}
}

public class ItemsConfigLoader
{
    static public Dictionary<int, ItemsConfig> Data = new Dictionary<int, ItemsConfig>();
    static ItemsConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Items"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ItemsConfig info = new ItemsConfig(jsondata[i]);
            Data[info.CfgId] = info;
        }
    }

    static public ItemsConfig GetConfig(int key, bool logError = true)
    {
        ItemsConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ItemsConfig has no key: {0}", key));
        }
        return null;
    }
}


// 装备锻造
public class EquipmentConfig : ConfigDataBase
{
	public int CfgId;					//配置ID
	public string Name;					//装备名称
	public string CreateIcon;					//锻造图标
	public int Level;					//装备等级
	public int EquipLevel;					//穿戴等级
	public int GoldDemand;					//普通锻造所需银两
	public int LuckyBuild;					//幸运锻造所需数量
	public int Type;					//1 刀  2 枪  3 弓  4 扇  5 布甲  6 重甲  7 饰品  8 兵法  装备类型
	public int BasicType;					//1 攻击力  2 法术伤害  3 生命值  4 伤害减免%  5 暴击%  6 吸血%  7 重击%  8 攻击速度%  9 冷却缩减%  基础属性类型
	public string WhiteLow;					//  白色属性
	public string GreenLower;					//绿色属性
	public string BlueLower;					//蓝色属性
	public string PurpleLower;					//紫色属性
	public string PurpleAttribute1;					//1 攻击力  2 法术伤害  3 生命值  4 伤害减免%  5 暴击%  6 吸血%  7 重击%  8 攻击速度%  9 冷却缩减%  紫色附加属性
	public string OrangeLower;					//橙色属性
	public string OrangeAttribute1;					//1 攻击力  2 法术伤害  3 生命值  4 伤害减免%  5 暴击%  6 吸血%  7 重击%  8 攻击速度%  9 冷却缩减%  橙色附加属性1
	public string OrangeAttribute2;					//1 攻击力  2 法术伤害  3 生命值  4 伤害减免%  5 暴击%  6 吸血%  7 重击%  8 攻击速度%  9 冷却缩减%  橙色附加属性2
	public int WhiteProb;					//白色概率
	public int GreenProb;					//绿色概率
	public int BlueProb;					//蓝色概率
	public int PurpleProb;					//紫色概率
	public int OrangeProb;					//橙色概率
	public int WhiteProb2;					//白色概率2
	public int GreenProb2;					//绿色概率2
	public int BlueProb2;					//蓝色概率2
	public int PurpleProb2;					//紫色概率2
	public int OrangeProb2;					//橙色概率2
	public int MoldDemand;					//锻造所需模具数量
	public string MaterialDemand;					//锻造所需  材料数量
	public int BuildingLevelDemand;					//锻造所需锻造铺等级
	public int WhiteProb3;					//副本掉落白色权重  (万分比)
	public int GreenProb3;					//副本掉落绿色权重(万分比)
	public int BlueProb3;					//副本掉落蓝色权重(万分比)
	public int PurpleProb3;					//副本掉落紫色权重(万分比)
	public int OrangeProb3;					//副本掉落橙色权重(万分比)
	public int WhiteProb4;					//商店刷出白色权重(万分比)
	public int GreenProb4;					//商店刷出绿色权重(万分比)
	public int BlueProb4;					//商店刷出蓝色权重(万分比)
	public int PurpleProb4;					//商店刷出紫色权重(万分比)
	public int OrangeProb4;					//商店刷出橙色权重(万分比)
	public EquipmentConfig(JsonData jdata) : base(jdata) {}
}

public class EquipmentConfigLoader
{
    static public Dictionary<int, EquipmentConfig> Data = new Dictionary<int, EquipmentConfig>();
    static EquipmentConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Equipment"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            EquipmentConfig info = new EquipmentConfig(jsondata[i]);
            Data[info.CfgId] = info;
        }
    }

    static public EquipmentConfig GetConfig(int key, bool logError = true)
    {
        EquipmentConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("EquipmentConfig has no key: {0}", key));
        }
        return null;
    }
}


// 兵法锻造
public class BingfaConfig : ConfigDataBase
{
	public int CfgId;					//配置ID
	public string Name;					//装备名称
	public string CreateIcon;					//锻造图标
	public int Level;					//装备等级
	public int EquipLevel;					//穿戴等级
	public int GoldDemand;					//普通锻造所需银两
	public int LuckyBuild;					//幸运锻造所需数量
	public int Type;					//1 刀  2 枪  3 弓  4 扇  5 布甲  6 重甲  7 饰品  8 兵法  装备类型
	public int WhiteLow;					//  白色属性
	public int GreenLower;					//绿色属性
	public int BlueLower;					//蓝色属性
	public int PurpleLower;					//紫色属性
	public int OrangeLower;					//橙色属性
	public int WhiteProb;					//白色概率
	public int GreenProb;					//绿色概率
	public int BlueProb;					//蓝色概率
	public int PurpleProb;					//紫色概率
	public int OrangeProb;					//橙色概率
	public int WhiteProb2;					//白色概率2
	public int GreenProb2;					//绿色概率2
	public int BlueProb2;					//蓝色概率2
	public int PurpleProb2;					//紫色概率2
	public int OrangeProb2;					//橙色概率2
	public int MoldDemand;					//锻造所需模具数量
	public string MaterialDemand;					//锻造所需  材料数量
	public int BuildingLevelDemand;					//锻造所需锻造铺等级
	public int WhiteProb3;					//副本掉落白色权重
	public int GreenProb3;					//副本掉落绿色权重
	public int BlueProb3;					//副本掉落蓝色权重
	public int PurpleProb3;					//副本掉落紫色权重
	public int OrangeProb3;					//副本掉落橙色权重
	public int WhiteProb4;					//商店刷出白色权重
	public int GreenProb4;					//商店刷出绿色权重
	public int BlueProb4;					//商店刷出蓝色权重
	public int PurpleProb4;					//商店刷出紫色权重
	public int OrangeProb4;					//商店刷出橙色权重
	public BingfaConfig(JsonData jdata) : base(jdata) {}
}

public class BingfaConfigLoader
{
    static public Dictionary<int, BingfaConfig> Data = new Dictionary<int, BingfaConfig>();
    static BingfaConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Bingfa"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            BingfaConfig info = new BingfaConfig(jsondata[i]);
            Data[info.CfgId] = info;
        }
    }

    static public BingfaConfig GetConfig(int key, bool logError = true)
    {
        BingfaConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("BingfaConfig has no key: {0}", key));
        }
        return null;
    }
}


// 品质权重分配表
public class ItemWeightClassConfig : ConfigDataBase
{
	public int WeightClassId;					//权重分类
	public int WeightNum;					//权重系数
	public int WeightNum2;					//权重系数2
	public ItemWeightClassConfig(JsonData jdata) : base(jdata) {}
}

public class ItemWeightClassConfigLoader
{
    static public Dictionary<int, ItemWeightClassConfig> Data = new Dictionary<int, ItemWeightClassConfig>();
    static ItemWeightClassConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ItemWeightClass"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ItemWeightClassConfig info = new ItemWeightClassConfig(jsondata[i]);
            Data[info.WeightClassId] = info;
        }
    }

    static public ItemWeightClassConfig GetConfig(int key, bool logError = true)
    {
        ItemWeightClassConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ItemWeightClassConfig has no key: {0}", key));
        }
        return null;
    }
}


// 兵种设定
public class SoldierConfig : ConfigDataBase
{
	public int SoldierId;					//兵种ID
	public string SoldierName;					//兵种名称
	public string SoldierIcon;					//兵种图标
	public string SoldierDescription;					//兵种描述
	public int MaxNumber;					//最大数量
	public int Attack;					//单位攻击力
	public int Hp;					//单位HP
	public float AttackSpeed;					//攻击速度
	public int MoveSpeed;					//移动速度
	public int States;					//占用人口
	public int AttackSpeedFlag;					//攻击速度  1 慢  2 普通  3 快
	public int MoveSpeedFlag;					//移动速度  1 慢  2 普通  3 快
	public int Producetime;					//单位生产时间（ms）
	public int UnlockMilitaryDemand;					//解锁所需校场等级
	public SoldierConfig(JsonData jdata) : base(jdata) {}
}

public class SoldierConfigLoader
{
    static public Dictionary<int, SoldierConfig> Data = new Dictionary<int, SoldierConfig>();
    static SoldierConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Soldier"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            SoldierConfig info = new SoldierConfig(jsondata[i]);
            Data[info.SoldierId] = info;
        }
    }

    static public SoldierConfig GetConfig(int key, bool logError = true)
    {
        SoldierConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("SoldierConfig has no key: {0}", key));
        }
        return null;
    }
}


// 兵种等级
public class SoldierLevelConfig : ConfigDataBase
{
	public int SoldierId;					//兵种ID
	public int SoldierLevel;					//兵种等级
	public int SoldierAttack;					//攻击
	public int SoldierHp;					//生命值
	public int ProduceCost;					//生产消耗金币
	public int UpgradeCost;					//升级消耗金币
	public int UpgradeTime;					//升级所需时间(ms)
	public int UpgradeMilitaryLevelDemand;					//升级所需校场等级
	public SoldierLevelConfig(JsonData jdata) : base(jdata) {}
}

public class SoldierLevelConfigLoader
{
    static public Dictionary<int, Dictionary<int, SoldierLevelConfig>> Data = new Dictionary<int, Dictionary<int, SoldierLevelConfig>>();
    static SoldierLevelConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/SoldierLevel"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            SoldierLevelConfig info = new SoldierLevelConfig(jsondata[i]);
            Dictionary<int, SoldierLevelConfig> dict;
            if (!Data.TryGetValue(info.SoldierId, out dict)) {
                dict = new Dictionary<int, SoldierLevelConfig>();
                Data[info.SoldierId] = dict;
            }
            dict[info.SoldierLevel] = info;
        }
    }

    static public SoldierLevelConfig GetConfig(int key, int key2, bool logError = true)
    {
        Dictionary<int, SoldierLevelConfig> dict;
        if (Data.TryGetValue(key, out dict)) {
            SoldierLevelConfig data;
            if (dict.TryGetValue(key2, out data)) {
                return data;
            }
        }

        if (logError) {
            Debug.LogError(string.Format("SoldierLevelConfig has no key: {0}, {1}", key, key2));
        }
        return null;
    }
}


// 兵种克制关系
public class SoldierRestraintConfig : ConfigDataBase
{
	public int Id;					//兵种ID(//行单位为攻击方，列单位为受攻击方)
	public int Soldier1;					//1.0
	public int Soldier2;					//2.0
	public int Soldier3;					//3.0
	public int Soldier4;					//4.0
	public SoldierRestraintConfig(JsonData jdata) : base(jdata) {}
}

public class SoldierRestraintConfigLoader
{
    static public Dictionary<int, SoldierRestraintConfig> Data = new Dictionary<int, SoldierRestraintConfig>();
    static SoldierRestraintConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/SoldierRestraint"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            SoldierRestraintConfig info = new SoldierRestraintConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public SoldierRestraintConfig GetConfig(int key, bool logError = true)
    {
        SoldierRestraintConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("SoldierRestraintConfig has no key: {0}", key));
        }
        return null;
    }
}


// 主城建筑设置
public class BuildingConstConfig : ConfigDataBase
{
	public int BuildingId;					//建筑ID
	public string BuildingName;					//建筑名称
	public string BuildingPinyin;					//建筑拼音
	public string BuildingDescription;					//建筑描述
	public int BuildingType;					//建筑类型  1 兵营  2 民房  3 采石场  4 伐木场  5 石材库  6 木材库  7 银库  8 校场  9 主城  10 铁匠铺  11 书院
	public string BuildingIcon;					//建筑图标
	public int UnlockHomeLevelDemand;					//解锁所需主城等级
	public BuildingConstConfig(JsonData jdata) : base(jdata) {}
}

public class BuildingConstConfigLoader
{
    static public Dictionary<int, BuildingConstConfig> Data = new Dictionary<int, BuildingConstConfig>();
    static BuildingConstConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/BuildingConst"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            BuildingConstConfig info = new BuildingConstConfig(jsondata[i]);
            Data[info.BuildingId] = info;
        }
    }

    static public BuildingConstConfig GetConfig(int key, bool logError = true)
    {
        BuildingConstConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("BuildingConstConfig has no key: {0}", key));
        }
        return null;
    }
}


// 建筑等级设置
public class BuildingLevelConfig : ConfigDataBase
{
	public int BuildingId;					//建筑ID
	public int BuildingLevel;					//建筑等级
	public int CostWood;					//木材消耗
	public int CostStone;					//石材消耗
	public int OutputType;					//产出类型  0 不产出  1 士兵  2 金钱  3 木材  4 石材  5 钻石
	public int OutputNumber;					//产出数量
	public int MaxStorage;					//最大储量
	public int UpgradeTime;					//升级时间（ms）
	public int HomeLevelDemand;					//升级所需主城等级
	public int PlayerLevelDemand;					//升级所需玩家等级
	public BuildingLevelConfig(JsonData jdata) : base(jdata) {}
}

public class BuildingLevelConfigLoader
{
    static public Dictionary<int, Dictionary<int, BuildingLevelConfig>> Data = new Dictionary<int, Dictionary<int, BuildingLevelConfig>>();
    static BuildingLevelConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/BuildingLevel"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            BuildingLevelConfig info = new BuildingLevelConfig(jsondata[i]);
            Dictionary<int, BuildingLevelConfig> dict;
            if (!Data.TryGetValue(info.BuildingId, out dict)) {
                dict = new Dictionary<int, BuildingLevelConfig>();
                Data[info.BuildingId] = dict;
            }
            dict[info.BuildingLevel] = info;
        }
    }

    static public BuildingLevelConfig GetConfig(int key, int key2, bool logError = true)
    {
        Dictionary<int, BuildingLevelConfig> dict;
        if (Data.TryGetValue(key, out dict)) {
            BuildingLevelConfig data;
            if (dict.TryGetValue(key2, out data)) {
                return data;
            }
        }

        if (logError) {
            Debug.LogError(string.Format("BuildingLevelConfig has no key: {0}, {1}", key, key2));
        }
        return null;
    }
}


// 大地图NPC城池配置
public class WorldMapConfig : ConfigDataBase
{
	public int Id;					//ID
	public string CityName;					//城池名称
	public int Difficulty;					//难度设定  1 简单  2 中等  3 困难
	public int PlayerLevel;					//玩家等级
	public int CityLevel;					//城池等级
	public int ResourceAdd;					//系统资源  奖励加成（%）
	public int DefensiveSetting;					//城防布置
	public int LineupSetting;					//布阵配置
	public int AppearProbability;					//刷出概率%
	public int DefenseHero1;					//守城英雄1
	public int DefenseHero2;					//守城英雄2
	public int DefenseHero3;					//守城英雄3
	public int BattlePower;					//战斗力
	public WorldMapConfig(JsonData jdata) : base(jdata) {}
}

public class WorldMapConfigLoader
{
    static public Dictionary<int, WorldMapConfig> Data = new Dictionary<int, WorldMapConfig>();
    static WorldMapConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/WorldMap"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            WorldMapConfig info = new WorldMapConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public WorldMapConfig GetConfig(int key, bool logError = true)
    {
        WorldMapConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("WorldMapConfig has no key: {0}", key));
        }
        return null;
    }
}


// 大地图资源抢夺比例
public class WorldMapResourceConfig : ConfigDataBase
{
	public int Id;					//ID
	public int LowerLevel;					//等级下限
	public int UpperLevel;					//等级上限
	public int RobPercent;					//抢夺比例（%）
	public int ExtraGold;					//额外获取银两
	public int ExtraWood;					//额外获取木材
	public int ExtraStone;					//额外获取石材
	public int FloatValue;					//浮动值%
	public int DiamondAward;					//元宝奖励
	public WorldMapResourceConfig(JsonData jdata) : base(jdata) {}
}

public class WorldMapResourceConfigLoader
{
    static public Dictionary<int, WorldMapResourceConfig> Data = new Dictionary<int, WorldMapResourceConfig>();
    static WorldMapResourceConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/WorldMapResource"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            WorldMapResourceConfig info = new WorldMapResourceConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public WorldMapResourceConfig GetConfig(int key, bool logError = true)
    {
        WorldMapResourceConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("WorldMapResourceConfig has no key: {0}", key));
        }
        return null;
    }
}


// 大地图匹配AI英雄配置表
public class WorldMapHeroConfig : ConfigDataBase
{
	public int CfgId;					//英雄配置id
	public string HeroName;					//英雄名称
	public string HeroIcon;					//英雄图标
	public int Skill1;					//技能1
	public int AttackType;					//攻击类型  （1近战 2 远战）
	public int Star;					//初始星级
	public int Strength;					//武力
	public int Intelligence;					//智力
	public int Leadership;					//统帅
	public int Range;					//普攻范围
	public float MoveSpeed;					//移动速度
	public float AtkSpeed;					//攻击速度
	public float Crtical;					//暴击
	public WorldMapHeroConfig(JsonData jdata) : base(jdata) {}
}

public class WorldMapHeroConfigLoader
{
    static public Dictionary<int, WorldMapHeroConfig> Data = new Dictionary<int, WorldMapHeroConfig>();
    static WorldMapHeroConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/WorldMapHero"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            WorldMapHeroConfig info = new WorldMapHeroConfig(jsondata[i]);
            Data[info.CfgId] = info;
        }
    }

    static public WorldMapHeroConfig GetConfig(int key, bool logError = true)
    {
        WorldMapHeroConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("WorldMapHeroConfig has no key: {0}", key));
        }
        return null;
    }
}


// 战斗力分段
public class FightSegConfig : ConfigDataBase
{
	public int Segment;					//段数 
	public int Lower;					//下限
	public int Upper;					//上限
	public FightSegConfig(JsonData jdata) : base(jdata) {}
}

public class FightSegConfigLoader
{
    static public Dictionary<int, FightSegConfig> Data = new Dictionary<int, FightSegConfig>();
    static FightSegConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/FightSeg"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            FightSegConfig info = new FightSegConfig(jsondata[i]);
            Data[info.Segment] = info;
        }
    }

    static public FightSegConfig GetConfig(int key, bool logError = true)
    {
        FightSegConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("FightSegConfig has no key: {0}", key));
        }
        return null;
    }
}


// 战功牌分段
public class BattleFeatSegConfig : ConfigDataBase
{
	public int Segment;					//段数 
	public int Lower;					//下限
	public int Upper;					//上限
	public BattleFeatSegConfig(JsonData jdata) : base(jdata) {}
}

public class BattleFeatSegConfigLoader
{
    static public Dictionary<int, BattleFeatSegConfig> Data = new Dictionary<int, BattleFeatSegConfig>();
    static BattleFeatSegConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/BattleFeatSeg"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            BattleFeatSegConfig info = new BattleFeatSegConfig(jsondata[i]);
            Data[info.Segment] = info;
        }
    }

    static public BattleFeatSegConfig GetConfig(int key, bool logError = true)
    {
        BattleFeatSegConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("BattleFeatSegConfig has no key: {0}", key));
        }
        return null;
    }
}


// 章节设定
public class ChapterConfig : ConfigDataBase
{
	public int Id;					//章节ID
	public string ChapterName;					//章节名称
	public string ChapterPicture;					//章节配图
	public int CompleteAward;					//章节通关奖励
	public ChapterConfig(JsonData jdata) : base(jdata) {}
}

public class ChapterConfigLoader
{
    static public Dictionary<int, ChapterConfig> Data = new Dictionary<int, ChapterConfig>();
    static ChapterConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Chapter"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ChapterConfig info = new ChapterConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ChapterConfig GetConfig(int key, bool logError = true)
    {
        ChapterConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ChapterConfig has no key: {0}", key));
        }
        return null;
    }
}


// 副本设定
public class MissionConstConfig : ConfigDataBase
{
	public int Id;					//ID
	public int Chapter;					//章节
	public string MissionName;					//关卡名称
	public int MissionDegree;					//0 普通副本  1 精英副本
	public int LevelRequire;					//等级要求
	public int FrontMission;					//前置关卡
	public string MissionDescription;					//关卡描述
	public int BattleConfig;					//战斗配置
	public int StaminaCost;					//消耗体力
	public int TimesLimit;					//限制次数
	public int RecommendStrength;					//推荐战斗力
	public int CompleteAward;					//通关奖励
	public int FirstAward;					//首次通关奖励
	public int ExpAward;					//通关英雄经验
	public int PlayExpAward;					//通关玩家经验
	public MissionConstConfig(JsonData jdata) : base(jdata) {}
}

public class MissionConstConfigLoader
{
    static public Dictionary<int, MissionConstConfig> Data = new Dictionary<int, MissionConstConfig>();
    static MissionConstConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/MissionConst"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            MissionConstConfig info = new MissionConstConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public MissionConstConfig GetConfig(int key, bool logError = true)
    {
        MissionConstConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("MissionConstConfig has no key: {0}", key));
        }
        return null;
    }
}


// 怪物配置表
public class MonsterConfigConfig : ConfigDataBase
{
	public int Id;					//ID
	public string Name;					//名称
	public string Description;					//描述
	public string ModelPath;					//模型路径
	public string ActionPath;					//动作路径
	public string Icon;					//图标
	public int Quality;					//品质
	public int Level;					//等级
	public int Attack;					//攻击力
	public int AbilityPower;					//法术强度
	public int HP;					//最大生命
	public int Mana;					//最大魔法
	public int AtkSpeed;					//攻击速度
	public int MoveSpeed;					//移动速度
	public int Skill1;					//技能1
	public int Skill2;					//技能2
	public int Skill3;					//技能3
	public MonsterConfigConfig(JsonData jdata) : base(jdata) {}
}

public class MonsterConfigConfigLoader
{
    static public Dictionary<int, MonsterConfigConfig> Data = new Dictionary<int, MonsterConfigConfig>();
    static MonsterConfigConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/MonsterConfig"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            MonsterConfigConfig info = new MonsterConfigConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public MonsterConfigConfig GetConfig(int key, bool logError = true)
    {
        MonsterConfigConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("MonsterConfigConfig has no key: {0}", key));
        }
        return null;
    }
}


// 奖励列表
public class AwardListConfig : ConfigDataBase
{
	public int Id;					//ID
	public string AwardName;					//奖励名称
	public string AwardProb;					//奖励概率%
	public List<string> Award1;					//奖励1
	public List<string> Award2;					//奖励2
	public List<string> Award3;					//奖励3
	public List<string> Award4;					//奖励4
	public List<string> Award5;					//奖励5
	public List<string> Award6;					//奖励6
	public List<string> Award7;					//奖励7
	public List<string> Award8;					//奖励8
	public List<string> Award9;					//奖励9
	public List<string> Award10;					//奖励10
	public List<string> Award11;					//奖励11
	public List<string> Award12;					//奖励12
	public AwardListConfig(JsonData jdata) : base(jdata) {}
}

public class AwardListConfigLoader
{
    static public Dictionary<int, AwardListConfig> Data = new Dictionary<int, AwardListConfig>();
    static AwardListConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/AwardList"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            AwardListConfig info = new AwardListConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public AwardListConfig GetConfig(int key, bool logError = true)
    {
        AwardListConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("AwardListConfig has no key: {0}", key));
        }
        return null;
    }
}


// 章节奖励表
public class ChapterAwardConfig : ConfigDataBase
{
	public int Id;
	public int ChapterID;					//章节ID
	public int Degree;					//是否精英副本
	public int Star;					//星级
	public int Award;					//奖励ID
	public ChapterAwardConfig(JsonData jdata) : base(jdata) {}
}

public class ChapterAwardConfigLoader
{
    static public Dictionary<int, ChapterAwardConfig> Data = new Dictionary<int, ChapterAwardConfig>();
    static ChapterAwardConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ChapterAward"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ChapterAwardConfig info = new ChapterAwardConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ChapterAwardConfig GetConfig(int key, bool logError = true)
    {
        ChapterAwardConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ChapterAwardConfig has no key: {0}", key));
        }
        return null;
    }
}


// 商店配置
public class ShopConstConfig : ConfigDataBase
{
	public int Id;					//物品id
	public int ItemId;					//物品配置id
	public int Price;					//购买单价
	public string AppearProb;					//物品数量刷出概率
	public int ShopType;					//商店类型  1 杂货商店  2 公会商店  3 竞技场商店  4 黑市商店
	public int RefreshProb;					//刷新概率  （万分）
	public int RefreshLowerLevel;					//刷新玩家等级下限
	public int RefreshUpperLevel;					//刷新玩家等级上限
	public ShopConstConfig(JsonData jdata) : base(jdata) {}
}

public class ShopConstConfigLoader
{
    static public Dictionary<int, ShopConstConfig> Data = new Dictionary<int, ShopConstConfig>();
    static ShopConstConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ShopConst"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ShopConstConfig info = new ShopConstConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ShopConstConfig GetConfig(int key, bool logError = true)
    {
        ShopConstConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ShopConstConfig has no key: {0}", key));
        }
        return null;
    }
}


// 商店必刷道具表
public class ShopFixGoodConfig : ConfigDataBase
{
	public int Id;					//ID
	public int ItemId;					//物品ID
	public string AmountProb;					//数量概率
	public int RefreshProb;					//刷新概率  （万分）
	public int Price;					//购买单价
	public int ShopType;					//商店类型  1 杂货商店  2 公会商店  3 竞技场商店  4 黑市商店
	public int RefreshLowerLevel;					//刷新玩家等级下限
	public int RefreshUpperLevel;					//刷新玩家等级上限
	public int RefreshSort;					//刷新类型
	public ShopFixGoodConfig(JsonData jdata) : base(jdata) {}
}

public class ShopFixGoodConfigLoader
{
    static public Dictionary<int, ShopFixGoodConfig> Data = new Dictionary<int, ShopFixGoodConfig>();
    static ShopFixGoodConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ShopFixGood"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ShopFixGoodConfig info = new ShopFixGoodConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ShopFixGoodConfig GetConfig(int key, bool logError = true)
    {
        ShopFixGoodConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ShopFixGoodConfig has no key: {0}", key));
        }
        return null;
    }
}


// PVP排名匹配
public class PVPMatchingConfig : ConfigDataBase
{
	public int Id;					//ID
	public int LowerRanking;					//排名下限
	public int UpperRanking;					//排名上限
	public int UpperStep;					//高步长
	public int LowerStep;					//低步长
	public int WinBonus;					//胜利积分奖励
	public int LoseBonus;					//失败积分奖励
	public int UpperNumber;					//高推人数
	public int LowerNumber;					//低推人数
	public PVPMatchingConfig(JsonData jdata) : base(jdata) {}
}

public class PVPMatchingConfigLoader
{
    static public Dictionary<int, PVPMatchingConfig> Data = new Dictionary<int, PVPMatchingConfig>();
    static PVPMatchingConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/PVPMatching"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            PVPMatchingConfig info = new PVPMatchingConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public PVPMatchingConfig GetConfig(int key, bool logError = true)
    {
        PVPMatchingConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("PVPMatchingConfig has no key: {0}", key));
        }
        return null;
    }
}


// 竞技场积分奖励
public class ArenaScoreConfig : ConfigDataBase
{
	public int Id;					//ID
	public int PointsDemand;					//所需积分
	public int AwardId;					//奖励ID
	public ArenaScoreConfig(JsonData jdata) : base(jdata) {}
}

public class ArenaScoreConfigLoader
{
    static public Dictionary<int, ArenaScoreConfig> Data = new Dictionary<int, ArenaScoreConfig>();
    static ArenaScoreConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ArenaScore"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ArenaScoreConfig info = new ArenaScoreConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ArenaScoreConfig GetConfig(int key, bool logError = true)
    {
        ArenaScoreConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ArenaScoreConfig has no key: {0}", key));
        }
        return null;
    }
}


// 竞技场历史排名奖励
public class ArenaHistoryRankConfig : ConfigDataBase
{
	public int Id;					//ID
	public int UpperHistoryRank;					//历史排名上限
	public int LowerHistoryRank;					//历史排名下限
	public int AwardId;					//奖励ID
	public ArenaHistoryRankConfig(JsonData jdata) : base(jdata) {}
}

public class ArenaHistoryRankConfigLoader
{
    static public Dictionary<int, ArenaHistoryRankConfig> Data = new Dictionary<int, ArenaHistoryRankConfig>();
    static ArenaHistoryRankConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ArenaHistoryRank"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ArenaHistoryRankConfig info = new ArenaHistoryRankConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ArenaHistoryRankConfig GetConfig(int key, bool logError = true)
    {
        ArenaHistoryRankConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ArenaHistoryRankConfig has no key: {0}", key));
        }
        return null;
    }
}


// 竞技场每日排名奖励
public class ArenaDailyRankConfig : ConfigDataBase
{
	public int Id;					//ID
	public int UpperRank;					//每日排名上限
	public int LowerRank;					//每日排名下限
	public int AwardId;					//奖励ID
	public ArenaDailyRankConfig(JsonData jdata) : base(jdata) {}
}

public class ArenaDailyRankConfigLoader
{
    static public Dictionary<int, ArenaDailyRankConfig> Data = new Dictionary<int, ArenaDailyRankConfig>();
    static ArenaDailyRankConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/ArenaDailyRank"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ArenaDailyRankConfig info = new ArenaDailyRankConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ArenaDailyRankConfig GetConfig(int key, bool logError = true)
    {
        ArenaDailyRankConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ArenaDailyRankConfig has no key: {0}", key));
        }
        return null;
    }
}


// VIP等级表
public class VIPLevelConfig : ConfigDataBase
{
	public int VipLevel;					//VIP等级
	public int BuyStaminaChance;					//购买体力次数
	public int BuyChallengeChance;					//竞技场可购买挑战次数
	public int ExtraMissionChance;					//单个精英副本可挑战次数
	public int BuyMissionChance;					//购买单个精英副本挑战次数
	public int ExtraResourceAward;					//额外攻城略地资源奖励%
	public int ExtraDiamandAward;					//攻城略地元宝奖励
	public int ResourceProtectChance;					//攻城略地资源保护次数
	public int ExtraSkillPoints;					//额外技能点上限
	public int ExtraFreeTime;					//额外建筑、升级科技免费时间（ms）
	public int DailySoulStoneAward;					//每日可领取万能灵魂石
	public int ExtraSignChance;					//签到可补签次数
	public VIPLevelConfig(JsonData jdata) : base(jdata) {}
}

public class VIPLevelConfigLoader
{
    static public Dictionary<int, VIPLevelConfig> Data = new Dictionary<int, VIPLevelConfig>();
    static VIPLevelConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/VIPLevel"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            VIPLevelConfig info = new VIPLevelConfig(jsondata[i]);
            Data[info.VipLevel] = info;
        }
    }

    static public VIPLevelConfig GetConfig(int key, bool logError = true)
    {
        VIPLevelConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("VIPLevelConfig has no key: {0}", key));
        }
        return null;
    }
}


// 竞技场（地图）配置
public class ArenaConfig : ConfigDataBase
{
	public int Id;
	public int Level;					//限制等级
	public string Name;					//竞技场名字
	public string Desc;					//描述
	public string Icon;					//竞技场图标
	public string Map;					//场景文件
	public string TowerLeft1;					//我方塔1的坐标
	public string TowerLeft2;
	public string CastleLeft;					//我方城堡坐标
	public string TowerRight1;
	public string TowerRight2;
	public string CastleRight;
	public string TowerAreaLeft1;					//敌方塔1击毁后解锁区域
	public string TowerAreaLeft2;
	public string TowerAreaRight1;					//敌方塔1击毁后解锁区域
	public string TowerAreaRight2;
	public int TrophyLimit;					//需要奖杯
	public int DemoteTrophyLimit;					//低于多少奖杯则降级
	public int ChestRewardMultiplier;					//宝箱加成
	public int ChestShopPriceMultiplier;					//商店商品售价加成
	public int RequestSize;
	public int MaxDonationCount;					//最多可捐献
	public int MatchmakingMinTrophyDelta;					//最低匹配的奖杯修正
	public int MatchmakingMaxTrophyDelta;					//最高匹配的奖杯修正
	public int MatchmakingMaxSeconds;					//最大匹配时间
	public int DailyDonationCapacityLimit;
	public string Music;
	public string ExtraTimeMusic;					//加时赛音乐
	public ArenaConfig(JsonData jdata) : base(jdata) {}
}

public class ArenaConfigLoader
{
    static public Dictionary<int, ArenaConfig> Data = new Dictionary<int, ArenaConfig>();
    static ArenaConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Arena"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            ArenaConfig info = new ArenaConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public ArenaConfig GetConfig(int key, bool logError = true)
    {
        ArenaConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("ArenaConfig has no key: {0}", key));
        }
        return null;
    }
}


// 卡牌属性
public class CardsAttributeConfig : ConfigDataBase
{
	public int Id;					//ID
	public string Name;					//名称
	public string Desc;					//描述
	public int NotInUse;					//召唤来的，无对应卡牌
	public string Icon;					//图标
	public string Image;					//大图
	public int UnlockArena;					//解锁竞技场
	public string Script;					//脚本
	public int Type;					//类型
	public int TurnToTarget;					//攻击时是否转向目标
	public int Quality;					//品质
	public int Cost;					//圣水消耗
	public int Mass;					//质量 影响角色推挤
	public int Health;					//生命值
	public int AtkRange;					//攻击距离
	public int MinimumRange;					//最小距离，小于此距离不进行攻击
	public int SightRange;					//警戒距离
	public int AtkInterval;					//攻击间隔
	public int AtkLoad;					//攻击前摇
	public int AtkTarget;					//攻击目标
	public int Damage;					//伤害
	public int DamageRadius;					//作用范围
	public int CrownTowerDamagePercent;					//对塔伤害减免
	public int MoveSpeed;					//移动速度
	public int SetTime;					//部署时间
	public int ArmyNumber;					//部队数量
	public int Formation;					//阵型配置
	public int FloorArea;					//建筑大小
	public int LifeDuration;					//持续时间
	public int LifeDurationIncreasePerLevel;					//每级增加的时间
	public int SummonID;					//召唤的单位的卡牌id  
	public int SummonLevel;					//召唤物相对于单位增加的等级
	public int SummonNumber;					//每次召唤数量
	public int SummonPauseTime;					//一次召唤后停止一段时间
	public int IgnorePushback;					//不会被击退
	public int CollisionRadius;					//碰撞范围
	public int ShieldHitpoints;					//护盾值
	public CardsAttributeConfig(JsonData jdata) : base(jdata) {}
}

public class CardsAttributeConfigLoader
{
    static public Dictionary<int, CardsAttributeConfig> Data = new Dictionary<int, CardsAttributeConfig>();
    static CardsAttributeConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/CardsAttribute"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            CardsAttributeConfig info = new CardsAttributeConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public CardsAttributeConfig GetConfig(int key, bool logError = true)
    {
        CardsAttributeConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("CardsAttributeConfig has no key: {0}", key));
        }
        return null;
    }
}


// 阵型属性
public class FormationConfig : ConfigDataBase
{
	public int Id;					//阵型配置ID
	public int Count;					//部队实例
	public string Positions;					//坐标
	public FormationConfig(JsonData jdata) : base(jdata) {}
}

public class FormationConfigLoader
{
    static public Dictionary<int, FormationConfig> Data = new Dictionary<int, FormationConfig>();
    static FormationConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Formation"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            FormationConfig info = new FormationConfig(jsondata[i]);
            Data[info.Id] = info;
        }
    }

    static public FormationConfig GetConfig(int key, bool logError = true)
    {
        FormationConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("FormationConfig has no key: {0}", key));
        }
        return null;
    }
}


// 文本翻译
public class TranslationConfig : ConfigDataBase
{
	public string Key;
	public string Text_zhCN;
	public string Text_en;
	public TranslationConfig(JsonData jdata) : base(jdata) {}
}

public class TranslationConfigLoader
{
    static public Dictionary<string, TranslationConfig> Data = new Dictionary<string, TranslationConfig>();
    static TranslationConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/Translation"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            TranslationConfig info = new TranslationConfig(jsondata[i]);
            Data[info.Key] = info;
        }
    }

    static public TranslationConfig GetConfig(string key, bool logError = true)
    {
        TranslationConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("TranslationConfig has no key: {0}", key));
        }
        return null;
    }
}


// 随机名字
public class RandomNameConfig : ConfigDataBase
{
	public int id;
	public int Type;					//0=prefix  1=suffix
	public string Text_en;
	public string Text_zhCN;
	public RandomNameConfig(JsonData jdata) : base(jdata) {}
}

public class RandomNameConfigLoader
{
    static public Dictionary<int, RandomNameConfig> Data = new Dictionary<int, RandomNameConfig>();
    static RandomNameConfigLoader()
    {
        JsonData jsondata = JsonMapper.ToObject(ConfigDataLoaderHelper.GetText("config/RandomName"));
        int count = jsondata.Count;
        for (int i = 0; i < count; ++i) {
            RandomNameConfig info = new RandomNameConfig(jsondata[i]);
            Data[info.id] = info;
        }
    }

    static public RandomNameConfig GetConfig(int key, bool logError = true)
    {
        RandomNameConfig value;
        if (Data.TryGetValue(key, out value)) {
            return value;
        }

        if (logError) {
            Debug.LogError(string.Format("RandomNameConfig has no key: {0}", key));
        }
        return null;
    }
}

