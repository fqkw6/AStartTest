using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MsgBoxParam
{
    public string text;
    public Color textColor;
    public string textTitle;
    public Color titleColor;
    public string textOK;
    public string textCancel;
    public System.Action okCallback;
    public System.Action cancelCallback;
    public bool clickToHide;
    public bool showBackground;
    public bool showCancelButton = true;
}

public class UIMsgBoxView : UIWindow
{
    public const string Name = "Common/UIMsgBoxView";
    public Text _txtTitle;
    public Text _txtText;
    public Button _btnOK;
    public Button _btnCancel;
    public Text _txtOK;
    public Text _txtCancel;

    private System.Action _onClickOKCallback;
    private System.Action _okClickCancelCallback;

    private Vector3 _okPosition;
    private MsgBoxParam _param;

    public override void OnOpenWindow()
    {
        gameObject.transform.localScale = Vector3.zero;

        _okPosition = _btnOK.transform.localPosition;
    }

    public override void OnBindData(params object[] param)
    {
        _param = (MsgBoxParam) param[0];
    }

    public override void OnRefreshWindow()
    {
        OnShowMsgBox(_param);
    }

    public override void OnCloseWindow()
    {
    }

    private void OnShowMsgBox(MsgBoxParam param)
    {
        // 标题
        if (string.IsNullOrEmpty(param.textTitle)) {
            _txtTitle.gameObject.SetActive(false);
        } else {
            _txtTitle.gameObject.SetActive(true);
            _txtTitle.text = param.textTitle;
        }

        _txtText.text = param.text;

        // 按钮文字
        if (!string.IsNullOrEmpty(param.textOK)) {
            _txtOK.text = param.textOK;
        }

        if (!string.IsNullOrEmpty(param.textCancel)) {
            _txtCancel.text = param.textCancel;
        }

        _btnOK.gameObject.SetActive(true);

        if (param.showCancelButton) {
            // 有确定和取消
            _btnCancel.gameObject.SetActive(true);
            _btnOK.transform.localPosition = _okPosition;
        } else {
            // 只有一个ok按钮
            _btnCancel.gameObject.SetActive(false);

            Vector3 pos = _btnOK.transform.localPosition;
            pos.x = 0;
            _btnOK.transform.localPosition = pos;
        }

        _onClickOKCallback = param.okCallback;
        _okClickCancelCallback = param.cancelCallback;

        if (param.clickToHide) {
            BackgroundEvent = ClickEvent.CLICK_TO_CLOSE;
        } else {
            BackgroundEvent = ClickEvent.INTERCEPT_EVENT;
        }

        if (!param.showBackground) {
            SetBackgroundOpacity(0);
        } else {
            SetBackgroundOpacity(BackgroundColor.a);
        }

        Show();
        PlayOpenAnimation();
    }
    
    public void OnClickOK()
    {
        if (_onClickOKCallback != null) {
            _onClickOKCallback();
        }

        CloseWindow();
    }

    public void OnClickCancel()
    {
        if (_okClickCancelCallback != null) {
            _okClickCancelCallback();
        }

        CloseWindow();
    }
}
