using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 加载场景
public class UILoadingSceneView : UIWindow
{
    public const string Name = "Common/UILoadingSceneView";
    public Text _textTip;
    public UIProgress _prgLoading;

    private AsyncOperation _ap;

    public override void OnOpenWindow()
    {
        IsMainWindow = true;
        _prgLoading.Reset();
    }

    public override void OnBindData(params object[] param)
    {
        _ap = (AsyncOperation) param[0];
    }

    void Update()
    {
        if (_ap == null) {
            return;
        }

        if (!_ap.isDone) {
            Debug.Log(_ap.progress);
            _prgLoading.SetValue(_ap.progress);
        } else {
            _ap = null;
            CloseWindow();
        }
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
