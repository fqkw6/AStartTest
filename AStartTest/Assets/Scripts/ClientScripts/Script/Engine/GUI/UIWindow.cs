using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// ui基础封装（TODO 根据实际手机运行效果决定是否需要缓存ui，理论上不需要，因为unity已经缓存了texture资源，所以ui关闭的时候直接销毁即可）
public class UIWindow : MonoBehaviour
{
    // 窗口层级(只影响显示顺序，不一定与实际功能string.Format("{0}/{1}", currentCount, needCount);相关联)
    public enum WindowLayer
    {
        NORMAL, // 正常窗口（可以设置是否有背景，以及点击背景是否消失）
        BG, // 底图层，显示在最下层，全屏拉伸，不留空白边
        TOPMOST, // 显示在最上层（系统消息提示）
    }

    // 点击背景的事件处理
    public enum ClickEvent
    {
        NONE, // 没有背景，即不拦截点击事件，除此之外的选项，无论是否显示背景，都会添加一个背景用于事件处理
        INTERCEPT_EVENT, // 拦截事件，即模态窗口，不能点击背景之后的ui
        CLICK_TO_CLOSE, // 点击关闭界面
    }

    // ui默认动画类型
    public enum AnimationType
    {
        NONE,
        POPUP,
        FADE_IN,
        FLY_IN_LEFT,
        FLY_IN_RIGHT,
    }

    // 关闭ui时的动画
    public enum CloseAnimationType
    {
        NONE,
        POPUP_CLOSE,
        FADE_OUT,
        FLY_OUT_LEFT,
        FLY_OUT_RIGHT,
    }

    // 是否已经初始化，一个标志，主要测试时用
    [NonSerialized]
    public bool HasInit;

    // 如果添加了背景，则为背景Go，否则为窗体Go
    [NonSerialized]
    public GameObject WindowObject;

    [Tooltip("界面显示的层级")]
    public WindowLayer Layer = WindowLayer.NORMAL;
    [Tooltip("是否显示背景，一般为半透明黑色背景")]
    public bool IsShowBackground = false; // 是否显示背景，仅与显示相关，如果设置了点击关闭界面或者是模态窗口，则自动添加一个不可见的背景
    [Tooltip("背景颜色")]
    public Color BackgroundColor = new Color(0, 0, 0, 0.4f);
    [Tooltip("背景是否模糊")]
    public bool BackgroundBlur = false; // 如果勾选，则此界面打开的时候，背景会模糊处理
    [Tooltip("点击背景的响应事件")]
    public ClickEvent BackgroundEvent = ClickEvent.NONE; // 注意如果需要相应背景点击事件，需要给界面添加Selectable组件，否则点击到界面Image上的事件会穿透到背景上。
    // 如果不具有父子关系的界面之间不需要添加此组件即可防止穿透

    [Tooltip("界面的初始坐标，默认居中")]
    public Vector3 StartPosition = Vector3.zero;

    [NonSerialized]
    public bool IsMainWindow = false; // MainWindow不会因为其他界面的打开而关闭，如主界面、系统消息界面

    [Tooltip("如果勾选，则界面可以被点击穿透")]
    public bool IgnoreRaycast = false;
    [Tooltip("如果勾选，则打开此界面时会关闭其他界面")]
    public bool IsMutexWindow = false;
    [Tooltip("打开界面时播放的声音")]
    public string OpenSound;
    [Tooltip("关闭界面时播放的声音")]
    public string CloseSound;
    [Tooltip("打开界面时的动画")]
    public AnimationType _openAnimationType = AnimationType.NONE; // 默认动画类型
    public float _openAnimationTime = 0.4f; // 动画时间

    [Tooltip("关闭界面时的动画")]
    public CloseAnimationType _closeAnimationType = CloseAnimationType.NONE; // 关闭时候的动画
    public float _closeAnimationTime = 0.4f;

    public void Start()
    {
        // 非代码创建的界面
        if (!HasInit) {
            OnOpenWindow();
            OnRefreshWindow();
        }
    }

    // 重载 界面显示时调用
    public virtual void OnOpenWindow()
    {
    }

    // 设置数据，参数一般通过UIManager.OpenWindow传进来
    public virtual void OnBindData(params object[] param)
    {
    }

    // 重载 当刷新界面时调用
    public virtual void OnRefreshWindow()
    {
    }

    // 重载 界面关闭时调用
    public virtual void OnCloseWindow()
    {
    }

    // 点击背景时调用
    public virtual void OnClickBackground()
    {
        if (BackgroundEvent == ClickEvent.CLICK_TO_CLOSE) {
            if (IsMainWindow) {
                // 如MsgBox界面，点击背景不销毁而仅仅是隐藏界面
                Hide(true);
            } else {
                CloseWindow(true);
            }
        }
    }

    // 点击关闭按钮（比较通用，统一到UIWindow来处理，关闭、取消按钮都可以绑定此事件）
    public virtual void OnClickClose()
    {
        CloseWindow(true);
    }

    // 关闭界面
    public void CloseWindow(bool withAnimation = false)
    {
        if (!string.IsNullOrEmpty(CloseSound)) {
            AudioController.PlaySound(CloseSound);
        }

        if (withAnimation && _closeAnimationType != CloseAnimationType.NONE) {
            PlayCloseAnimation(DoCloseWindow);
        } else {
            DoCloseWindow();
        }
    }

    private void DoCloseWindow()
    {
        if (BackgroundBlur) {
            UIManager.Instance.EnableBlur(false);
        }

        // 响应回调
        OnCloseWindow();
        Destroy(WindowObject);

        // 从管理者中移除
        UIManager.Instance.RemoveWindow(name);
    }

    // 显示界面
    public void Show()
    {
        WindowObject.SetActive(true);
    }

    // 隐藏界面（不是销毁）
    public void Hide(bool withAnimation = false)
    {
        if (withAnimation && _closeAnimationType != CloseAnimationType.NONE) {
            // 有关闭动画
            PlayCloseAnimation(() => { WindowObject.SetActive(false); });
        } else {
            WindowObject.SetActive(false);
        }
    }

    // 当前界面是否显示
    public bool IsShown
    {
        get { return WindowObject.activeInHierarchy; }  // activeSelf 为 true 和其父物体(以及祖先物体)为激活状态才为 true
    }

    // 窗口坐标(相对于父窗口的，并且不考虑背景)
    public Vector3 Position
    {
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }

    // 获取控件
    public T GetControl<T>(string cname) where T : Component
    {
        Transform control = Utils.FindChild(transform, cname);
        return control != null ? control.GetComponent<T>() : null;
    }

    // 设置当前界面在另一个界面之前(调整遮挡关系)
    public void SetOrderBefore(GameObject go)
    {
        int winIndex = go.transform.GetSiblingIndex();
        transform.SetSiblingIndex(Mathf.Max(0, go.transform.GetSiblingIndex()));
    }

    // 设置当前界面在另一个界面之后
    public void SetOrderAfter(GameObject go)
    {
        transform.SetSiblingIndex(go.transform.GetSiblingIndex() + 1);
    }

    // 显示ui时播放动画
    public virtual void PlayOpenAnimation(TweenCallback callback = null)
    {
        switch (_openAnimationType) {
            case AnimationType.POPUP:
                OnAnimationPopup(callback);
                break;
            case AnimationType.FADE_IN:
                OnAnimationFadein(callback);
                break;
            case AnimationType.FLY_IN_LEFT:
                OnAnimationFlyInLeft(callback);
                break;
            case AnimationType.FLY_IN_RIGHT:
                OnAnimationFlyInRight(callback);
                break;
        }
    }

    // 关闭ui时播放动画
    public virtual void PlayCloseAnimation(TweenCallback callback = null)
    {
        switch (_closeAnimationType) {
            case CloseAnimationType.POPUP_CLOSE:
                OnAnimationPopupClose(callback);
                break;
            case CloseAnimationType.FADE_OUT:
                OnAnimationFadeout(callback);
                break;
            case CloseAnimationType.FLY_OUT_LEFT:
                OnAnimationFlyOutLeft(callback);
                break;
            case CloseAnimationType.FLY_OUT_RIGHT:
                OnAnimationFlyOutRight(callback);
                break;
        }
    }

    private void OnAnimationPopup(TweenCallback callback)
    {
        transform.localScale = Vector3.zero;
        Tweener tweener = transform.DOScale(Vector3.one, _openAnimationTime).SetEase(Ease.OutBack);
        if (callback != null) {
            tweener.OnComplete(callback);
        }
    }

    private void OnAnimationPopupClose(TweenCallback callback)
    {
        Tweener tweener = transform.DOScale(Vector3.zero, _openAnimationTime).SetEase(Ease.InBack);
        if (callback != null) {
            tweener.OnComplete(callback);
        }
    }

    private void OnAnimationFadein(TweenCallback callback)
    {
        SetOpacity(0);
        FadeTo(1, _openAnimationTime);
        if (callback != null) {
            callback();
        }
    }

    private void OnAnimationFadeout(TweenCallback callback)
    {
        if (callback != null) {
            callback();
        }
    }

    private void OnAnimationFlyInLeft(TweenCallback callback)
    {
        //        gameObject.transform.localPosition = new Vector3(-Screen.width / 2 - 400, 0, 0);
        //        LTDescr tween = LeanTween.moveLocal(gameObject, StartPosition, _openAnimationTime);
        //        if (callback != null) {
        //            tween.setOnComplete(callback);
        //        }

        Tweener tweener = transform.DOLocalMove(StartPosition, _openAnimationTime);
        if (callback != null) {
            tweener.OnComplete(callback);
        }
    }

    private void OnAnimationFlyInRight(TweenCallback callback)
    {
        if (callback != null) {
            callback();
        }
    }

    private void OnAnimationFlyOutLeft(TweenCallback callback)
    {
        if (callback != null) {
            callback();
        }
    }

    private void OnAnimationFlyOutRight(TweenCallback callback)
    {
        if (callback != null) {
            callback();
        }
    }

    // 设置背景的透明度
    public void SetBackgroundOpacity(float value)
    {
        Image bgImage = WindowObject.GetComponent<Image>();
        if (bgImage != null) {
            Color color = bgImage.color;
            color.a = value;
            bgImage.color = color;
        }
    }

    // 设置界面的透明度（会影响所有的控件）
    public void SetOpacity(float value)
    {
        CanvasRenderer[] renderers = GetComponentsInChildren<CanvasRenderer>();
        foreach (var item in renderers) {
            item.SetAlpha(value);
        }
    }

    public void FadeTo(float alpha, float time)
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>();
        foreach (var item in graphics) {
            item.CrossFadeAlpha(alpha, time, false);
        }
    }
}