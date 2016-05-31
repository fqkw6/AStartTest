using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 给英雄选择装备
public class UISelectEquipView : UIWindow
{
    public const string Name = "Hero/UISelectEquipView";
    public UIListView _listView;

    private HeroInfo _info;
    private ItemType _itemType;
    private List<ItemInfo> _itemList = new List<ItemInfo>(); 

    public override void OnBindData(params object[] param)
    {
        _info = param[0] as HeroInfo;
        _itemType = (ItemType)param[1];
    }

    public override void OnRefreshWindow()
    {
        foreach (var item in UserManager.Instance.ItemList) {
            if (item.Cfg.Type == (int) _itemType && item.Cfg.Level > 0 && item.Cfg.Level <= _info.Level) {
                _itemList.Add(item);
            }
        }

		_itemList.Sort ((a, b)=>{
			return b.GetScore().CompareTo(a.GetScore());
		});

        _listView.Data = _itemList.ToArray();
        _listView.OnClickListItem = OnClickItem;
        _listView.Refresh();
    }

    private void OnClickItem(int index, ListItemWidget widget)
    {
        if (UserManager.Instance.RequestEquipItem(_info.EntityID, _itemList[index].EntityID)) {
            CloseWindow();
        }
    }
}
