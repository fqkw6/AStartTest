using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UIItemGetWayView : UIWindow
{
    public const string Name = "Hero/UIItemGetWayView";
    public float _animationOffset = 200;
    public float _animationTime = 0.5f;

    public override void OnOpenWindow()
    {
        transform.DOLocalMoveX(_animationOffset, _animationTime);
        
        SetOpacity(0);
        FadeTo(1, _animationTime);
    }

    public override void OnBindData(params object[] param)
    {
        SetItemInfo(param[0] as ItemInfo);
    }

    public void OnClickBack()
    {
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_ITEM_EQUIP_SUBVIEW_CLOSE);
        CloseWindow();
    }

    public void SetItem(int cfgID)
    {
        
    }

    public void SetItemInfo(ItemInfo info)
    {
        
    }
}
