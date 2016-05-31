using UnityEngine;
using System.Collections.Generic;

// 锻造铺中装备 材料的panel
public class SmithyItemPanel : MonoBehaviour
{
    public SmithyItemWidget _itemFinal;         // 最终生成的装备
    public SmithyItemWidget[] _itemList;   // 物品栏

    public void SetInfo(int equipCfgID, List<ItemInfo> mold, List<SmithyManager.SmithyItemInfo> stuff, int moldNeedCount)
    {
        _itemFinal.SetInfo(equipCfgID, SmithyItemWidget.WidgetType.EQUIP,  1, 0);

        ItemsConfig cfg = ItemsConfigLoader.GetConfig(equipCfgID);

        int level = 0;

        if (cfg.Type < (int)ItemType.BOOK) {
            EquipmentConfig cfgDest = EquipmentConfigLoader.GetConfig(equipCfgID);
            level = cfgDest.Level - 1;
        } else {
            BingfaConfig cfgDest = BingfaConfigLoader.GetConfig(equipCfgID);
            level = cfgDest.Level - 1;
        }


        for (int i = 0; i < _itemList.Length; ++i) {
            if (i < moldNeedCount) {
                // 排模具
                if (i < mold.Count) {
                    // 有足够的模具
                    _itemList[i].SetInfo(mold[i], SmithyItemWidget.WidgetType.MOLD, 1, 1);
                } else {
                    // 模具不足
                    _itemList[i].SetInfo(null, SmithyItemWidget.WidgetType.MOLD, 1, 1);
                }
                _itemList[i].SetMoldInfo(i, (ItemType)cfg.Type, level);
            } else {
                // 排材料
                int index = i - moldNeedCount;
                if (index < stuff.Count) {
                    // 材料不足
                    SmithyManager.SmithyItemInfo itemInfo = stuff[index];
                    if (itemInfo.CfgID == GameConfig.ITEM_CONFIG_ID_WOOD) {
                        // 木材
                        _itemList[i].SetInfo(itemInfo.CfgID, SmithyItemWidget.WidgetType.MATERIAL, UserManager.Instance.Wood, itemInfo.Count);
                    } else if (itemInfo.CfgID == GameConfig.ITEM_CONFIG_ID_STONE) {
                        // 石材
                        _itemList[i].SetInfo(itemInfo.CfgID, SmithyItemWidget.WidgetType.MATERIAL, UserManager.Instance.Stone, itemInfo.Count);
                    } else {
                        // 普通材料
                        ItemInfo info = UserManager.Instance.GetItemByConfigID(itemInfo.CfgID);
                        if (info != null) {
                            _itemList[i].SetInfo(itemInfo.CfgID, SmithyItemWidget.WidgetType.MATERIAL, info.Number, itemInfo.Count);
                        } else {
                            _itemList[i].SetInfo(itemInfo.CfgID, SmithyItemWidget.WidgetType.MATERIAL, 0, itemInfo.Count);
                        }
                    }
                } else {
                    // 材料种类充足（并不确定具体每个材料是否充足）
                    _itemList[i].SetInfo(0, SmithyItemWidget.WidgetType.MATERIAL, 0, 0);
                }
            }   
        }
    }

    public void SetMold(int index, ItemInfo item)
    {
        _itemList[index].SetInfo(item, SmithyItemWidget.WidgetType.MOLD, 1, 1);
    }
}
