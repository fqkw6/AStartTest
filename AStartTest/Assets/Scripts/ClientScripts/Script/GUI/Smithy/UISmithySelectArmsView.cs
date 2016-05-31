using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 选择武器的界面
public class UISmithySelectArmsView : UIWindow
{
    public const string Name = "Smithy/UISmithySelectArmsView";

    public UIListView _listView;

    private List<ItemInfo> _listItem = new List<ItemInfo>();

    private int _slotIndex;
    private ItemType _equipType;
    private int _limitLevel;

    public override void OnBindData(params object[] param)
    {
        _slotIndex = (int) param[0];
        _equipType = (ItemType) param[1];
        _limitLevel = (int) param[2];
    }

    public override void OnRefreshWindow()
    {
        _listItem.Clear();

        foreach (var item in UserManager.Instance.ItemList) {
            if (item.IsEquip() && item.Cfg.Type == (int)_equipType) {
                if (item.IsBook()) {
                    BingfaConfig cfg = BingfaConfigLoader.GetConfig(item.ConfigID);
                    if (cfg.Level >= _limitLevel) {
                        _listItem.Add(item);
                    }
                } else {
                    EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(item.ConfigID);
                    if (cfg.Level >= _limitLevel) {
                        _listItem.Add(item);
                    }
                }
            }
        }
        
        SmithyManager.Instance.SortItem(_listItem);

        _listView.Data = _listItem.ToArray();
        _listView.OnClickListItem = OnSelectItem;
        _listView.Refresh();
    }

    private void OnSelectItem(int index, ListItemWidget widget)
    {
        ItemInfo info = _listItem[index];
        EventDispatcher.TriggerEvent(EventID.SMITHY_SELECT_EQUIP, _slotIndex, info);
        CloseWindow();
    }
}
