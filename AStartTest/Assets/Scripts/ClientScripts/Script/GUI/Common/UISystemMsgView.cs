using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;

// 屏幕中央的系统消息提示区域
public class UISystemMsgView : UIWindow
{
    public const string Name = "Common/UISystemMsgView";
    private class MsgData
    {
        public string text;

    }

    private class FloatingMsgWidget
    {
        public Text text;
        public float startTime;
        public bool removeFlag = false;
    }

    public Transform _floatingStartPoint;
    public Transform _floatingEndPoint;
    public Text _scrollMsgText;
    public Text _centerMsgText;
    public Text _floatingMsgPrefab;

    public float _animationTime = 1;
    public float _centerMsgShowTime = 2f;
    public float _centerMsgFadeoutTime = 0.5f;

    private List<Text> _recycleList = new List<Text>();

    private List<string> _systemMsgList = new List<string>();

    public override void OnOpenWindow()
    {
        IsMainWindow = true;

        _scrollMsgText.gameObject.SetActive(false);
        _centerMsgText.gameObject.SetActive(false);
        _floatingMsgPrefab.gameObject.SetActive(false);

        EventDispatcher.AddEventListener<string>(EventID.EVENT_UI_SHOW_SYSTEM_MSG, AddSystemMsg);
        EventDispatcher.AddEventListener<string, Color>(EventID.EVENT_UI_SHOW_CENTER_MSG, AddCenterMsg);
        EventDispatcher.AddEventListener<string, Color, float>(EventID.EVENT_UI_SHOW_FLOATING_MSG, AddFloatingMsg);
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener<string>(EventID.EVENT_UI_SHOW_SYSTEM_MSG, AddSystemMsg);
        EventDispatcher.RemoveEventListener<string, Color>(EventID.EVENT_UI_SHOW_CENTER_MSG, AddCenterMsg);
        EventDispatcher.RemoveEventListener<string, Color, float>(EventID.EVENT_UI_SHOW_FLOATING_MSG, AddFloatingMsg);

        CleanUp();
    }

    private void CleanUp()
    {
        foreach (var item in _recycleList) {
            Destroy(item.gameObject);
        }

        _recycleList.Clear();
    }

    // 添加系统滚动消息
    public void AddSystemMsg(string text)
    {
        _scrollMsgText.gameObject.SetActive(true);
        _scrollMsgText.text = text;
    }

    // 在中央提示一条消息，时间到后消失
    public void AddCenterMsg(string text, Color color)
    {
        _centerMsgText.text = text;
        Color c = _centerMsgText.color;
        c.a = 1;
        _centerMsgText.color = c;
        _centerMsgText.gameObject.SetActive(true);

        // 停止原来的动画
        DOTween.Kill(_centerMsgText);

        // 开启新的动画，显示一秒钟，然后渐隐消失
        _centerMsgText.DOFade(0, _centerMsgFadeoutTime).SetDelay(_centerMsgShowTime).OnComplete(() => {
            _centerMsgText.gameObject.SetActive(false);
        });
    }

    // 在中央提示一条消息，有从下到上运动，并渐隐消失
    public void AddFloatingMsg(string text, Color color, float delayTime)
    {
        DOVirtual.DelayedCall(delayTime, () => {
            Text floatingMsg = GetFloatingMsg();
            floatingMsg.text = text;
            Color c = color;
            c.a = 1;
            floatingMsg.color = c;
            floatingMsg.transform.localPosition = _floatingStartPoint.localPosition;

            floatingMsg.transform.DOLocalMoveY(_floatingEndPoint.localPosition.y, _animationTime).OnComplete(() => {
                floatingMsg.gameObject.SetActive(false);
                _recycleList.Add(floatingMsg);
            });

            floatingMsg.DOFade(0, 0.3f).SetDelay(_animationTime - 0.3f);

        });
    }

    private Text GetFloatingMsg()
    {
        Text floatingMsg = null;

        if (_recycleList.Count > 0) {
            floatingMsg = _recycleList[0];
            _recycleList.RemoveAt(0);
        } else {
            floatingMsg = Instantiate(_floatingMsgPrefab);
            floatingMsg.transform.SetParent(transform, false);
        }

        floatingMsg.gameObject.SetActive(true);
        return floatingMsg;
    }
}
