using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UINewHeroListView : UIWindow
{
    public const string Name = "Hero/UINewHeroListView";
    public UIListView _listView;
    private int _currentType;
    private List<int> _heroList = new List<int>(); 

    public override void OnOpenWindow()
    {
        UserManager.Instance.SortHeroList();
    }

    public override void OnCloseWindow()
    {

    }

    public override void OnRefreshWindow()
    {
        UpdateList();

    }

    public void UpdateList()
    {
        _heroList.Clear();
        List<int> heroNotHaveList = new List<int>();

        foreach (var item in HeroConfigLoader.Data) {
            if (!UserManager.Instance.HaveHero(item.Key) && (_currentType == 0 || _currentType == item.Value.AttackType)) {
                heroNotHaveList.Add(item.Key);
            }
        }

        // 排序后的英雄列表
        foreach (var item in UserManager.Instance.HeroList) {
            if (_currentType == 0 || _currentType == item.Cfg.AttackType) {
                _heroList.Add(item.ConfigID);
            }
        }

        _listView.MaxCount = _heroList.Count + heroNotHaveList.Count;
        _listView.OnClickListItem = OnClickItem;
        _listView.OnListItemAtIndex = (index) =>
        {
            if (index >= _heroList.Count) {
                NewHeroListHeroNotHave widget = _listView.CreateListItemWidget<NewHeroListHeroNotHave>(1);
                widget.SetInfo(heroNotHaveList[index - _heroList.Count]);
                return widget;
            } else {
                NewHeroListHeroHave widget = _listView.CreateListItemWidget<NewHeroListHeroHave>(0);
                widget.SetInfo(UserManager.Instance.GetHeroInfoByUnitID(_heroList[index]));
                return widget;
            }
        };
        _listView.Refresh();
    }

    private void OnClickItem(int index, ListItemWidget widget)
    {
        if (index < _heroList.Count) {
            HeroInfo heroInfo = UserManager.Instance.GetHeroInfoByUnitID(_heroList[index]);
            UIManager.Instance.OpenWindow<UINewHeroView>(heroInfo);
        }
    }

    public void OnClickReturn()
    {
        UIManager.Instance.OpenWindow<UINewMainView>();
        CloseWindow();
    }

    public void OnClickAddMoney()
    {
        UIManager.Instance.OpenWindow<UICityBuyResourceView>(ResourceType.MONEY);
    }

    public void OnClickAddSp()
    {
        UIManager.Instance.OpenWindow<UICityBuyResourceView>();
    }

    public void OnClickAddGold()
    {
        UIManager.Instance.OpenWindow<UICityBuyResourceView>();
    }



    public void OnToggleAll(bool value)
    {
        if (value == false) return;

        _currentType = 0;
        UpdateList();
    }


    public void OnToggleAtk(bool value)
    {
        if (value == false) return;

        _currentType = 1;
        UpdateList();
    }


    public void OnToggleDef(bool value)
    {
        if (value == false) return;
        _currentType = 2;
        UpdateList();
    }

    public void OnToggleAux(bool value)
    {
        if (value == false) return;
        _currentType = 3;
        UpdateList();
    }
}
