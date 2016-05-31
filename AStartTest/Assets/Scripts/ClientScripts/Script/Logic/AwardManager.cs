using UnityEngine;
using System.Collections.Generic;


// 奖励数据，这个主要是方便维护客户端逻辑，不需要从服务器获取
public class AwardInfo
{
    public int ItemID;
    public int ItemCount;

    public AwardInfo(int id, int count)
    {
        ItemID = id;
        ItemCount = count;
    }
}

// 奖励逻辑
public class AwardManager
{
    public static readonly AwardManager Instance = new AwardManager();

    public List<AwardInfo> GetAwardList(int awardID, bool onlyItem = false)
    {
        List<AwardInfo> ret = new List<AwardInfo>();
        AwardListConfig cfg = AwardListConfigLoader.GetConfig(awardID);
        if (cfg == null) return ret;

        ParseAwardID(cfg.Award1, ret, onlyItem);
        ParseAwardID(cfg.Award2, ret, onlyItem);
        ParseAwardID(cfg.Award3, ret, onlyItem);
        ParseAwardID(cfg.Award4, ret, onlyItem);
        ParseAwardID(cfg.Award5, ret, onlyItem);
        ParseAwardID(cfg.Award6, ret, onlyItem);
        ParseAwardID(cfg.Award7, ret, onlyItem);
        ParseAwardID(cfg.Award8, ret, onlyItem);

        return ret;
    }

    private void ParseAwardID(List<string> textList, List<AwardInfo> list, bool onlyItem)
    {
        if (textList.Count <= 0) return;

        string txt = textList[0];

        if (string.IsNullOrEmpty(txt)) return;

        int index = txt.IndexOf("-");
        int id = System.Convert.ToInt32(txt.Substring(0, index));
        int count = System.Convert.ToInt32(txt.Substring(index + 1));

        if (onlyItem) {
            if (id == GameConfig.ITEM_CONFIG_ID_MONEY
                || id == GameConfig.ITEM_CONFIG_ID_WOOD
                || id == GameConfig.ITEM_CONFIG_ID_STONE
                || id == GameConfig.ITEM_CONFIG_ID_GOLD) {
                return;
            }
            list.Add(new AwardInfo(id, count));
        } else {
            list.Add(new AwardInfo(id, count));
        }
    }
}
