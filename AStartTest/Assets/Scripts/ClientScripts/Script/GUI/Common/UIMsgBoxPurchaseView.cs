using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 元宝不足的充值提示
public class UIMsgBoxPurchaseView : UIWindow
{
    public const string Name = "Common/UIMsgBoxPurchaseView";
    public override void OnOpenWindow()
    {
    }
    
    public void OnClickOKPurchase()
    {
        // TODO 打开商城界面，进行充值
        CloseWindow();
    }

    public void OnClickCancel()
    {
        CloseWindow();
    }
}
