using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UINewBagView : UIWindow
{
    public const string Name = "Hero/UINewBagView";
    private enum ShowPage
    {
        ALL,
        EQUIP,
        STONE,
        ITEM,
    }
    
    public Button _btnAll;
    public Button _btnEquip;
    public Button _btnStone;
    public Button _btnItem;
    public UIListView _listView;
    public PanelItemTip _itemTip;

    private List<ItemInfo> _list; 
    private List<ItemWidget> _listWidget = new List<ItemWidget>(); 
    private ShowPage _currentShowPage;

    public override void OnOpenWindow()
    {
        _currentShowPage = ShowPage.ALL;

        // 打开背包时清理主界面上的标识
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_MAIN_NEW_FLAG, GameFunc.BAG, false);
    }

    public override void OnCloseWindow()
    {
        UIManager.Instance.CloseWindow<UIItemSellView>();

        // 关闭背包的时候清理新物品标识
        UserManager.Instance.ResetNewItemFlag();
    }

    public override void OnRefreshWindow()
    {
        UpdateList(_currentShowPage);
    }

    // 先按时间排序，新获得的物品排在前面
    private void SortListByTime(List<ItemInfo> list)
    {
        list.Sort((a, b) =>
        {
            if (a.IsNewItem != b.IsNewItem) {
                // 新获得的物品排在前面
                if (a.IsNewItem) {
                    return -1;
                } else {
                    return 1;
                }
            } else {
                if (Mathf.Abs(a.AwardTime - b.AwardTime) <= 0.1f) {
                    // 获取时间相同按照品阶排序
                    return b.Cfg.Quality.CompareTo(a.Cfg.Quality);
                } else {
                    // 否则按照获取时间排序(时间越大的排在越前面，代表越近获得)
                    return b.AwardTime.CompareTo(a.AwardTime);
                }
            }
        });
    }

    // 按品质排序
    private void SortListByQuality(List<ItemInfo> list)
    {
        list.Sort((a, b) => b.Cfg.Quality.CompareTo(a.Cfg.Quality));

    }

    private void UpdateList(ShowPage page)
    {
        _currentShowPage = page;
        UpdateTab();

        _list = new List<ItemInfo>();
        foreach (var item in UserManager.Instance.ItemList) {
            if (page == ShowPage.ALL
                || (page == ShowPage.EQUIP && item.IsEquip())
                || (page == ShowPage.ITEM && item.Cfg.Enable > 0)
                || (page == ShowPage.STONE && item.Cfg.Type == (int)ItemType.SOUL_PIECE)) {
                _list.Add(item);
            }
        }

        if (page == ShowPage.ALL) {
            SortListByTime(_list);
        } else {
            SortListByQuality(_list);
        }

        _listWidget.Clear();
        _listView.MaxCount = _list.Count;
        _listView.OnClickListItem = OnClickItemIcon;
        _listView.OnListItemAtIndex = (index) =>
        {
            BagItemWidget item = _listView.CreateListItemWidget<BagItemWidget>(0);
            item.SetInfo(_list[index]);
            _listWidget.Add(item);
            return item;
        };
        _listView.Refresh();

        SelectFirstItem();
    }

    private void UpdateTab()
    {
        _btnAll.interactable = true;
        _btnEquip.interactable = true;
        _btnItem.interactable = true;
        _btnStone.interactable = true;

        if (_currentShowPage == ShowPage.ALL) {
            _btnAll.interactable = false;
        } else if (_currentShowPage == ShowPage.EQUIP) {
            _btnEquip.interactable = false;
        } else if (_currentShowPage == ShowPage.STONE) {
            _btnStone.interactable = false;
        } else if (_currentShowPage == ShowPage.ITEM) {
            _btnItem.interactable = false;
        }
    }

    private void SelectFirstItem()
    {
        if (_list != null && _list.Count > 0) {
            _itemTip.gameObject.SetActive(true);
            _itemTip.SetItemInfo(_list[0]);
            SelectItemWidget(_list[0].ConfigID);
        } else {
            _itemTip.gameObject.SetActive(false);
            SelectItemWidget(0);
        }
    }

    public void OnClickAll()
    {
        UpdateList(ShowPage.ALL);
    }

    public void OnClickEquip()
    {
        UpdateList(ShowPage.EQUIP);
    }

    public void OnClickStone()
    {
        UpdateList(ShowPage.STONE);
    }

    public void OnClickItem()
    {
        UpdateList(ShowPage.ITEM);
    }

    public void OnClickItemIcon(int index, ListItemWidget widget)
    {
        ItemInfo info = _list[index];
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_UPDATE_ITEMTIP_INFO, info);
        SelectItemWidget(info.EntityID);

        var w = (widget as BagItemWidget);
        if (w) {
            w.ClearFlag();
        }
    }

    private void SelectItemWidget(long entityID)
    {
        foreach (var item in _listWidget) {
            if (item._info == null) {
                continue;
            }

            if (item._info.EntityID == entityID) {
                item.Select();
            } else {
                item.UnSelect();
            }
        }
    }
}
