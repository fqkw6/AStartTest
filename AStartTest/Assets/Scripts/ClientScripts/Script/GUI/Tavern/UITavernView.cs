using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 酒馆界面（抽卡）
public class UITavernView : UIWindow
{
    public const string Name = "Tavern/UITavernView";
    public UITavernLeft2Panel _left2Panel;
    public UITavernRight2Panel _right2Panel;
    
    public override void OnOpenWindow()
    {
        ShopManager.Instance.RequestTavernBuyInfo();    // 获取酒馆抽奖的信息
    }
    
    public override void OnCloseWindow()
    {
    }

    public override void OnRefreshWindow()
    {
        _left2Panel.Refresh();
        _right2Panel.Refresh();
    }
}

