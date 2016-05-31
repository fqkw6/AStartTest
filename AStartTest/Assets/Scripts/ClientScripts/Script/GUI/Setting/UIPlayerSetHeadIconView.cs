using UnityEngine;
using System.Collections;

// 修改人物头像界面
public class UIPlayerSetHeadIconView : UIWindow
{
    public const string Name = "Setting/UIPlayerSetHeadIconView";

    public UIListView _listView;

    public override void OnRefreshWindow()
    {
        // TODO 头像列表（包括英雄和自定义头像）

        _listView.OnClickListItem = OnClickListItem;
    }

    private void OnClickListItem(int index, ListItemWidget widget)
    {
        
    }
}
