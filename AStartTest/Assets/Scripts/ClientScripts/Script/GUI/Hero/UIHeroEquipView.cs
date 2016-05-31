using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// 在背包中装备物品时选择英雄的界面
public class UIHeroEquipView : UIWindow
{
    public const string Name = "Hero/UIHeroEquipView";

    public UIListView _listView;

    private ItemInfo _itemInfo;
    private List<HeroInfo> _heroList = new List<HeroInfo>(); 

    public override void OnBindData(params object[] param)
    {
        _itemInfo = param[0] as ItemInfo;
    }

    public override void OnRefreshWindow()
    {
        foreach (var item in UserManager.Instance.HeroList) {
            ItemType type = (ItemType)_itemInfo.Cfg.Type;
            if (type == ItemType.DECORATION || type == ItemType.BOOK
                || type == (ItemType) item.Cfg.WeaponType || type == (ItemType) item.Cfg.ArmourType) {
                if (_itemInfo.Cfg.Level <= item.Level) {
                    _heroList.Add(item);
                }
            }
        }

        _listView.MaxCount = _heroList.Count;
        _listView.OnClickListItem = OnClickWidget;
        _listView.OnListItemAtIndex = (index) =>
        {
            var widget = _listView.CreateListItemWidget<HeroEquipWidget>(0);
            widget.SetInfo(_heroList[index], _itemInfo);
            return widget;
        };
        _listView.Refresh();
    }

    private void OnClickWidget(int index, ListItemWidget widget)
    {
        HeroInfo heroInfo = _heroList[index];
        UserManager.Instance.RequestEquipItem(heroInfo.EntityID, _itemInfo.EntityID);
        CloseWindow();
    }
}
