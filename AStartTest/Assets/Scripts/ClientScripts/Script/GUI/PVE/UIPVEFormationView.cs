using UnityEngine;
using System.Collections.Generic;

// 设置阵型界面
public class UIPVEFormationView : UIWindow
{
    public const string Name = "PVE/UIPVEFormationView";
    public UIListView _listView;

    private bool _hasChanged = false;

    private List<HeroInfo> _listHero = new List<HeroInfo>();

    public override void OnCloseWindow()
    {
        if (_hasChanged) {
            UserManager.Instance.ReqSetFormation();
        }
    }

    public override void OnRefreshWindow()
    {
        _listHero.Clear();

        List<HeroInfo> list = new List<HeroInfo>();

        foreach (var item in UserManager.Instance.HeroList) {
            if (UserManager.Instance.IsHeroInPVEFormation(item.ConfigID)) {
                _listHero.Add(item);
            } else {
                list.Add(item);
            }
        }

        UserManager.Instance.SortHeroList(list);
        _listHero.AddRange(list);

        _listView.MaxCount = _listHero.Count;

        _listView.OnClickListItem = OnClickListItem;
        _listView.OnListItemAtIndex = (index) =>
        {
            FormationHeroItemWidget go = _listView.CreateListItemWidget<FormationHeroItemWidget>(0);
            go.SetHeroInfo(_listHero[index]);
            return go;
        };
        _listView.Refresh();
    }

    private void OnClickListItem(int index, ListItemWidget widget)
    {
        FormationHeroItemWidget w = widget as FormationHeroItemWidget;
        if (w == null) return;

        
        _hasChanged = true;
        if (w.IsSelect) {
            w.OnUnselect();
        } else {
            if (UserManager.Instance.PVEHeroList.Count >= GameConfig.MAX_PVE_HERO_COUNT) {
                return;
            }

            w.OnSelect();
        }
    }

}

