using UnityEngine;
using System.Collections.Generic;

// 获得新物品的界面
public class UIGetItemView : UIWindow
{
    public const string Name = "Hero/UIGetItemView";
    public UIListView _listView;
    private List<ItemInfo> _itemList;
     
    public override void OnBindData(params object[] param)
    {
        _itemList = param[0] as List<ItemInfo>;
    }

    public override void OnRefreshWindow()
    {
        _listView.Data = _itemList.ToArray();
        _listView.Refresh();
    }
}
