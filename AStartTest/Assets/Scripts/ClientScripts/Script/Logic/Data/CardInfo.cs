using UnityEngine;
using System.Collections;

// 卡牌数据
public class CardInfo
{
    public long EntityID;    // 实例ID，全服唯一，用于升级、捐赠等逻辑，主要是与服务器通信用
    public int ConfigID;    // 配置ID
    public int Level;   // 等级
    public int Index;   // 在哪个手牌位置 未放置手牌时是-1，只有正确设置了手牌索引的牌才能正确出牌

    private CardsAttributeConfig _cfg;
    public CardsAttributeConfig Cfg
    {
        get
        {
            if (_cfg == null) {
                _cfg = CardsAttributeConfigLoader.GetConfig(ConfigID);
            }
            return _cfg;
        }
    }

    // 复制一个卡牌数据，同一张牌可能出现多次
    public CardInfo Clone()
    {
        var info = new CardInfo();
        info.EntityID = EntityID;
        info.ConfigID = ConfigID;
        info.Level = Level;
        info.Index = Index;
        return info;
    }

    public bool IsSpell()
    {
        return Cfg.Type == (int) CardType.SPELL;
    }

    public bool IsBuilding()
    {
        return Cfg.Type == (int) CardType.BUILDING;
    }

    public bool IsSoldier()
    {
        return Cfg.Type == (int) CardType.SOLDIER;
    }
}
