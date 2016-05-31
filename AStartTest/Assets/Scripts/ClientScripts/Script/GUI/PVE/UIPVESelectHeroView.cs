using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// pve关卡选择英雄出站的界面
public class UIPVESelectHeroView : UIWindow
{
    public const string Name = "PVE/UIPVESelectHeroView";
    public Button _btnAll;
    public Button _btnAttack;
    public Button _btnDefend;
    public Button _btnAux;
    public UIListView _listView;

    public override void OnOpenWindow()
    {   
        UserManager.Instance.SortHeroList();
    }

    public override void OnRefreshWindow()
    {
        UpdateList();
    }

    private void UpdateList()
    {
        _listView.Data = UserManager.Instance.HeroList.ToArray();
        _listView.OnListItemAtIndex = (index) =>
        {
            PVESelectHeroWidget go = _listView.CreateListItemWidget<PVESelectHeroWidget>(0);
            go.OnClickItem += OnClickItem;
            go.SetInfo(UserManager.Instance.HeroList[index]);
            return go;
        };
        _listView.Refresh();
    }

    private void UpdateHero()
    {
        
    }

    private void OnClickItem(int heroConfigID)
    {
        HeroInfo info = UserManager.Instance.GetHeroInfoByUnitID(heroConfigID);
        if (info == null) return;

        if (info.IsOnPVE()) {
            UserManager.Instance.PVEHeroList.Remove(info);
        } else {
            UserManager.Instance.PVEHeroList.Add(info);
        }

        // 只刷新点击的英雄
        foreach (Transform item in _listView._listContainer) {
            PVESelectHeroWidget widget = item.GetComponent<PVESelectHeroWidget>();
            if (widget != null && widget.IsWidget(heroConfigID)) {
                widget.SetInfo(info);
            }
        }

        // TODO 刷新英雄
    }

    // 开始战斗，只可能是普通战斗，扫荡不会选择英雄
    public void OnClickFight()
    {
        CloseWindow();
        PVEManager.Instance.RequestFight(PVEManager.Instance.CurrentSelectLevelID);
    }

    public void OnClickAll()
    {
        // TODO 配置英雄类别
    }

    public void OnClickAttack()
    {
        
    }

    public void OnClickDefend()
    {
        
    }

    public void OnClickAux()
    {
        
    }
}
