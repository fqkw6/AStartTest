using UnityEngine;
using UnityEngine.EventSystems;

public class UIUtil
{
    public static void ShowMsg(string text)
    {
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_SHOW_CENTER_MSG, text, Color.green);
    }

    public static void ShowMsgFormat(string tkey, params object[] param)
    {
        if (param.Length > 0) {
            string text = string.Format(Str.Get(tkey), param);
            ShowMsg(text);
        } else {
            ShowMsg(Str.Get(tkey));
        }
    }

    public static void ShowErrMsg(string text)
    {
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_SHOW_CENTER_MSG, text, Color.red);
    }

    public static void ShowErrMsgFormat(string tkey, params object[] param)
    {
        if (param.Length > 0) {
            string text = string.Format(Str.Get(tkey), param);
            ShowErrMsg(text);
        } else {
            ShowErrMsg(Str.Get(tkey));
        }
    }

    public static void AddFloatingMsg(string text, Color color, float delayTime)
    {
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_SHOW_FLOATING_MSG, text, color, delayTime);
    }

    public static void AddFloatingMsg(string text, float delayTime = 0)
    {
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_SHOW_FLOATING_MSG, text, Color.green, delayTime);
    }

    public static void AddFloatingMsgFormat(string tkey, Color color, float delayTime, params object[] param)
    {
        if (param.Length > 0) {
            string text = string.Format(Str.Get(tkey), param);
            AddFloatingMsg(text, color, delayTime);
        } else {
            AddFloatingMsg(Str.Get(tkey), color, delayTime);
        }
    }

    // 显示确定框，有确定按钮，默认不显示背景，点击隐藏界面
    public static void ShowMsgBox(string text, string textTitle, System.Action okCallback, bool clickToHide = true, bool showBackground = false)
    {
        MsgBoxParam param = new MsgBoxParam();
        param.text = text;
        param.textTitle = textTitle;
        param.okCallback = okCallback;
        param.cancelCallback = null;
        param.clickToHide = clickToHide;
        param.showBackground = showBackground;
        param.showCancelButton = false;

        ShowMsgBox(param);
    }

    // 显示确认框，有确定和取消，默认显示半透明背景，并且点击不可取消界面
    public static void ShowConfirm(string text, string textTitle, System.Action okCallback, System.Action cancelCallback = null, bool clickToHide = false, bool showBackground = true)
    {
        MsgBoxParam param = new MsgBoxParam();
        param.text = text;
        param.textTitle = textTitle;
        param.okCallback = okCallback;
        param.cancelCallback = cancelCallback;
        param.clickToHide = clickToHide;
        param.showBackground = showBackground;
        param.showCancelButton = true;

        ShowMsgBox(param);
    }

    public static void ShowMsgBox(MsgBoxParam param)
    {
        UIManager.Instance.OpenWindow<UIMsgBoxView>(param);
    }

    // 是否有点到ui上面
    public static bool IsTouchUI()
    {
        GameObject go = EventSystem.current.currentSelectedGameObject;
        if (go != null) {
            return true;
        }

        return false;
    }
}
