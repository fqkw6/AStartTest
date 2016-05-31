using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class EventID
{
    public const string EVENT_USE_ITEM = "EVENT_USE_ITEM";
}

// 从装备选择英雄
public class UISelectHeroView : UIWindow
{
    public const string Name = "Hero/UISelectHeroView";
    public UIListView _listView;

    private ItemInfo _itemInfo;
    private List<HeroInfo> _listHero = new List<HeroInfo>(); 

    public override void OnBindData(params object[] param)
    {
        _itemInfo = param[0] as ItemInfo;

        OnToggleAll(true);
    }


    public override void OnRefreshWindow()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        _listHero.Sort((a, b) =>
        {
            // 战斗力高的排前面
            return b.FightingScore.CompareTo(a.FightingScore);
        });

        _listView.OnClickListItem = OnClickItem;
        _listView.Data = _listHero.ToArray();
        _listView.Refresh();
    }

    private void OnClickItem(int index, ListItemWidget widget)
    {
        UserManager.Instance.RequestUseExpBook(_listHero[index].EntityID, _itemInfo.EntityID, 1);
    }

    public void OnToggleAll(bool value)
    {
        if (value == false) return;
        _listHero.Clear();
        _listHero.AddRange(UserManager.Instance.HeroList);
        UpdateList();
    }


    public void OnToggleAtk(bool value)
    {
        if (value == false) return;
        _listHero.Clear();
        foreach (var item in UserManager.Instance.HeroList) {
            if (item.Cfg.AttackType == 1) {
                _listHero.Add(item);
            }
        }
        UpdateList();
    }


    public void OnToggleDef(bool value)
    {
        if (value == false) return;
        _listHero.Clear();
        foreach (var item in UserManager.Instance.HeroList) {
            if (item.Cfg.AttackType == 2) {
                _listHero.Add(item);
            }
        }
        UpdateList();
    }

    public void OnToggleAux(bool value)
    {
        if (value == false) return;
        _listHero.Clear();
        foreach (var item in UserManager.Instance.HeroList) {
            if (item.Cfg.AttackType == 3) {
                _listHero.Add(item);
            }
        }
        UpdateList();
    }
}
