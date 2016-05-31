using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 系统设置界面
public class UIPlayerSetSystemView : UIWindow
{
    public const string Name = "Setting/UIPlayerSetSystemView";

    public Text _txtNotifyTime1;
    public Text _txtNotify1;
    public Text _txtNotifyTime2;
    public Text _txtNotify2;
    public Text _txtNotifyTime3;
    public Text _txtNotify3;
    public Text _txtNotifyTime4;
    public Text _txtNotify4;

    public override void OnRefreshWindow()
    {
    }

    public void OnToggleMusic(bool value)
    {
        
    }

    public void OnToggleEffect(bool value)
    {
        
    }

    public void OnToggleNotify1(bool value)
    {
        
    }

    public void OnToggleNotify2(bool value)
    {

    }

    public void OnToggleNotify3(bool value)
    {

    }

    public void OnToggleNotify4(bool value)
    {

    }
}
