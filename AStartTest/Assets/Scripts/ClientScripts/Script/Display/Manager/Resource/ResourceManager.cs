using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

// 资源管理，资源加载
public class ResourceManager : Singleton<ResourceManager>
{
    public GameObject _shadowProjectorPrefab;
    public GameObject _simpleShadowProjectorPrefab;

    public List<Sprite> _resIconList;       // 资源图标列表
    public List<Sprite> _priceIconList;       // 金钱图标列表
    public List<Sprite> _sprHeroType;   // 英雄类型
    public List<Sprite> _sprItemType;   // 物品类型底图 
    public List<Sprite> _sprSoldierType;    // 兵种图标 
    public List<Sprite> _sprItemAttrFlag;   // 物品属性标签 

    public delegate void OnLoadOverHandler(Object obj);

    // 初始化，在游戏一开始的时候调用
    public void Init(AssetBundleMananger.OnInitOverHandler callback)
    {
        if (callback != null) {
            callback();
        }
        //AssetBundleMananger.Instance.Init(callback);
    }

    // 加载一个物件
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public void Load(string path, OnLoadOverHandler callback)
    {
        Object obj = Resources.Load(path);
        if (callback != null) {
            callback(obj);
        }
    }

    public Sprite GetSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);
        return sprite;
    }

    // 获取英雄头像
    public Sprite GetHeroIcon(int heroID)
    {
        HeroConfig cfg = HeroConfigLoader.GetConfig(heroID);
        if (cfg == null || string.IsNullOrEmpty(cfg.HeroIcon)) {
            return GetSprite("Texture/HeroIcon/guanyutouxiang1");
        }
        return GetSprite("Texture/HeroIcon/" + cfg.HeroIcon);
    }

    // 获取物品图标
    public Sprite GetItemIcon(int itemID)
    {
        ItemsConfig cfg = ItemsConfigLoader.GetConfig(itemID);
        if (cfg == null || string.IsNullOrEmpty(cfg.Icon)) {
            return GetSprite("Texture/ItemIcon/19");
        }
        return GetSprite("Texture/ItemIcon/" + cfg.Icon);
    }

    // 获取物品大图
    public Sprite GetEquipImage(int itemID)
    {
        EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(itemID);
        if (cfg == null || string.IsNullOrEmpty(cfg.CreateIcon)) {
            return GetSprite("Texture/ItemIcon2/1");
        }

        return GetSprite("Texture/ItemIcon2/" + cfg.CreateIcon);
    }

    // 获取兵法大图
    public Sprite GetBookImage(int itemID)
    {
        BingfaConfig cfg = BingfaConfigLoader.GetConfig(itemID);
        if (cfg == null || string.IsNullOrEmpty(cfg.CreateIcon)) {
            return GetSprite("Texture/ItemIcon2/1");
        }

        return GetSprite("Texture/ItemIcon2/" + cfg.CreateIcon);
    }

    // 获取技能图标
    public Sprite GetSkillIcon(int skillID)
    {
        SkillSettingsConfig cfg = SkillSettingsConfigLoader.GetConfig(skillID);
        if (cfg == null || string.IsNullOrEmpty(cfg.Icon)) {
            return GetSprite("Texture/SkillIcon/UI_jinengtubiao_1");
        }
        return GetSprite("Texture/SkillIcon/" + cfg.Icon);
    }

    // 获取士兵头像
    public Sprite GetSoldierIcon(int soldierID)
    {
        SoldierConfig cfg = SoldierConfigLoader.GetConfig(soldierID);
        if (cfg == null || string.IsNullOrEmpty(cfg.SoldierIcon)) {
            return GetSprite("Texture/ItemIcon/UI_zhuangbeitubiao_lan_1");
        }

        return GetSprite("Texture/SoldierIcon/" + cfg.SoldierIcon);
    }

    // 获取士兵半身像
    public Sprite GetSoldierImage(int soldierID)
    {
        //SoldierConfig cfg = SoldierConfigLoader.GetConfig(soldierID);
        //if (cfg == null || string.IsNullOrEmpty(cfg.SoldierIcon.ToString())) {
        //    return GetSprite("Texture/ItemIcon/UI_zhuangbeitubiao_lan_1");
        //}

        //return GetSprite(cfg.SoldierIcon.ToString());
        return GetSprite("Texture/SoldierImage/Soldier1");
    }

    // 获取建筑图标
    public Sprite GetBuildingIcon(int cfgID)
    {
        BuildingConstConfig cfg = BuildingConstConfigLoader.GetConfig(cfgID);
        if (cfg != null) {
            return GetSprite("Texture/BuildingIcon/" + cfg.BuildingIcon);
        }
        return GetSprite("Texture/BuildingIcon/UI_bingying");
    }

    // 获取建筑图像
    public Sprite GetBuildingrImage(int cfgID)
    {
        BuildingConstConfig cfg = BuildingConstConfigLoader.GetConfig(cfgID);
        if (cfg != null) {
            return GetSprite("Texture/BuildingIcon/" + cfg.BuildingIcon);
        }
        return GetSprite("Texture/BuildingIcon/UI_bingying");
    }

    // 根据品阶，获取背景框
    public Sprite GetIconBgByQuality(int quality)
    {
        string prefix = "Texture/IconBg/";
        switch (quality) {
            case 1:
                return GetSprite(prefix + "icon_putong_dis");
            case 2:
                return GetSprite(prefix + "icon_lvse_dis");
            case 3:
                return GetSprite(prefix + "icon_lanse_dis");
            case 4:
                return GetSprite(prefix + "icon_zise_dis");
            case 5:
                return GetSprite(prefix + "icon_chengse_dis");
        }
        return GetSprite(prefix + "icon_putong_dis");
    }


    public Sprite GetIconBgCoverByQuality(int quality)
    {
        string prefix = "Texture/IconBg/";
        switch (quality) {
            case 1:
                return GetSprite(prefix + "fanpin");
            case 2:
                return GetSprite(prefix + "changpin");
            case 3:
                return GetSprite(prefix + "liangpin");
            case 4:
                return GetSprite(prefix + "jingpin");
            case 5:
                return GetSprite(prefix + "huangpin");
        }
        return GetSprite(prefix + "fanpin");
    }

    //根据品阶，获取武将列表底框
    public Sprite GetHeroBgByStar(int star)
    {
        string prefix = "Texture/IconBg/";
        switch (star) {
            case 1:
                return GetSprite(prefix + "wujiangkuangbai");
            case 2:
                return GetSprite(prefix + "wujiangkuanglv");
            case 3:
                return GetSprite(prefix + "wujiangkuanglan");
            case 4:
                return GetSprite(prefix + "wujiangkuang1");
            case 5:
                return GetSprite(prefix + "wujiangkuangcheng");
        }
        return GetSprite(prefix + "wujiangkuangbai");
    }

    public Sprite GetHeroIconBgByStar(int star)
    {
        string prefix = "Texture/IconBg/";
        switch (star) {
            case 1:
                return GetSprite(prefix + "wujiangkuangbai");
            case 2:
                return GetSprite(prefix + "wujiangdilv");
            case 3:
                return GetSprite(prefix + "wujiangdilan");
            case 4:
                return GetSprite(prefix + "wujiangkuangzi2");
            case 5:
                return GetSprite(prefix + "wujiangkuangbai2");
        }
        return GetSprite(prefix + "wujiangkuangbai");
    }

    //根据品阶，获取武将物品底框颜色
    public Sprite GetItemBgByStar(int quality)
    {
        string prefix = "Texture/IconBg/";
        switch (quality) {
            case 1:
                return GetSprite(prefix + "xiaozhuangbeikuanghuangse1");
            case 2:
                return GetSprite(prefix + "xiaozhuangbeikuanglvse1");
            case 3:
                return GetSprite(prefix + "xiaozhuangbeikuanglanse1");
            case 4:
                return GetSprite(prefix + "xiaozhuangbeikuangzise1");
        }
        return GetSprite(prefix + "xiaozhuangbeikuanghuangse1");
    }

    public Sprite GetHeroImage(int heroID)
    {
        HeroConfig cfg = HeroConfigLoader.GetConfig(heroID);
        if (cfg != null) {
            return GetSprite("Texture/HeroImage/" + cfg.HeroImage);
        }
        return GetSprite("Texture/HeroImage/huangzhongkapaihui");
    }

    public Sprite GetHeroNameImage(int heroID)
    {
        HeroConfig cfg = HeroConfigLoader.GetConfig(heroID);
        if (cfg != null) {
            return GetSprite("Texture/HeroNameImage/" + cfg.HeroNamePath);
        }
        return GetSprite("Texture/HeroNameImage/ico_huangzhongmingzi");
    }

    // 获取不同品阶下的名字颜色
    public Color GetColorByQuality(int quality)
    {
        switch (quality) {
            case 1:
                return Color.white;
            case 2:
                return new Color(190.0f / 255, 187.0f / 255, 90.0f / 255);
            case 3:
                return new Color(12.0f / 255, 136.0f / 255, 207.0f / 255);
            case 4:
                return new Color(197.0f / 255, 26f / 255, 222.0f / 255);
            case 5:
                return new Color(218.0f / 255, 193f / 255, 103.0f / 255);
        }
        return Color.white;
    }

    // 获取资源图标
    public Sprite GetResIcon(ResourceType resType)
    {
        switch (resType) {
            case ResourceType.MONEY:
                return _resIconList[1];
            case ResourceType.WOOD:
                return _resIconList[2];
            case ResourceType.STONE:
                return _resIconList[3];
            case ResourceType.GOLD:
                return _resIconList[4];
            case ResourceType.SOLDIER:
                return _resIconList[5];
        }

        return null;
    }

    // 获取价格图标
    public Sprite GetPriceIcon(PriceType priceType)
    {
        switch (priceType) {
            case PriceType.MONEY:
                return _priceIconList[0];
        }

        return _priceIconList[0];
    }

    // 根据商店类型获取价格图标
    public Sprite GetPriceIcon(ShopType shopType)
    {
        switch (shopType) {
            case ShopType.SUNDRY:
                return _priceIconList[0];
            case ShopType.BLACK_MARKET:
                return _priceIconList[1];
        }

        return _priceIconList[0];
    }

    // 获取士兵类型图标
    public Sprite GetSoldierTypeIcon(int cfgID)
    {
        if (cfgID == 1) {
            return _sprSoldierType[0];
        } else if (cfgID == 2) {
            return _sprSoldierType[1];
        } else if (cfgID == 3) {
            return _sprSoldierType[2];
        } else if (cfgID == 4) {
            return _sprSoldierType[3];
        }
        return _sprSoldierType[0];
    }

    // 获取英雄类型图标
    public Sprite GetHeroTypeIcon(int heroConfigID)
    {
        HeroConfig cfg = HeroConfigLoader.GetConfig(heroConfigID);
        return _sprHeroType[cfg.AttackType - 1];
    }

    public Sprite GetPlayerIcon(int iconID)
    {
        foreach (var item in HeroConfigLoader.Data) {
            return GetHeroIcon(item.Key);
        }
        return null;
    }

    // 获取物品类型底图
    public Sprite GetItemTypeIcon(ItemType itemType)
    {
        switch (itemType) {
            case ItemType.BOW:
            case ItemType.FAN:
            case ItemType.SPEAR:
            case ItemType.SWORD:
                return _sprItemType[0];
            case ItemType.CHEST:
            case ItemType.CLOTH:
                return _sprItemType[1];
            case ItemType.DECORATION:
                return _sprItemType[2];
            case ItemType.BOOK:
                return _sprItemType[3];
        }
        return null;
    }

    // 物品属性
    public Sprite GetItemAttrFlag(int qualityLevel)
    {
        return _sprItemAttrFlag[qualityLevel - 1];
    }


    // 获取投影，实时投影，效果好，效率低
    public GameObject GetShadowProjector()
    {
        return Instantiate(_shadowProjectorPrefab);
    }

    // 获取简单投影（一个圆圈），效率高
    public GameObject GetSimpleShadowProjector()
    {
        return Instantiate(_simpleShadowProjectorPrefab);
    }
}
