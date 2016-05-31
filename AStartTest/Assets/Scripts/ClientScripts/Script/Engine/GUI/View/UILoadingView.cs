using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 进度加载界面
public class UILoadingView : UIWindow
{
    public const string Name = "Common/UILoadingView";
    public Text _textTip;
    public UIProgress _prgLoading;
    
    public override void OnOpenWindow()
    {
        IsMainWindow = true;
        _prgLoading.Reset();
 	}

    void Update()
    {
        
    }

    public void SetText(string text)
    {
        _textTip.text = text;
    }

    public void SetProgressValue(float value)
    {
        _prgLoading.SetValue(value);
    }
}
