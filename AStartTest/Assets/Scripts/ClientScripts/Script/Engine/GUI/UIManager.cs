using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityStandardAssets.ImageEffects;

public class UIManager : Singleton<UIManager>
{
    private GameObject _rootBg;
    private GameObject _root;
    private GameObject _rootTopmost;
    
    private List<UIWindow> _openedWindows = new List<UIWindow>();

    public delegate void OpenWindowHandler(UIWindow window);

    public static readonly int UI = LayerMask.NameToLayer("UI");
    public static readonly int UIMask = LayerMask.GetMask("UI");
    public static readonly int UITop = LayerMask.NameToLayer("UITop");
    public static readonly int UITopMask = LayerMask.GetMask("UITop");
    public static readonly int UIModel = LayerMask.NameToLayer("UIModel");
    public static readonly int UIModelMask = LayerMask.GetMask("UIModel");

    private Camera _uiCamera;
    private Camera _modelCamera;

    public Canvas Canvas
    {
        get
        {
            if (_root == null) {
                return null;
            }
            return _root.GetComponent<Canvas>();
        }
    }


    private void Awake()
    {
        // 初始化canvas
        InitUICanvas();
        InitBgCanvas();
        InitTopmostCanvas();
    }

    private void Start()
    {
        GameObject go = GameObject.Find("UICamera");
        if (go != null) {
            _uiCamera = go.GetComponent<Camera>();
        }
        GameObject go2 = GameObject.Find("ModelCamera");
        if (go2 != null) {
            _modelCamera = go2.GetComponent<Camera>();
        }

        EnableBlur(false);
    }

    private GameObject InitPanel(string objName)
    {
        GameObject go = GameObject.Find(objName);
        if (go == null) {
            go = new GameObject(objName);
            go.transform.SetParent(_root.transform, false);
            go.AddComponent<CanvasRenderer>();
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.sizeDelta = Vector3.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        return go;
    }

    private void InitBgCanvas()
    {
        // 地图的屏幕适配规则与ui不同，地图需要全屏拉伸不留黑边，ui需要保证所有元素都在屏幕内部不会被裁减
        _rootBg = GameObject.Find("CanvasBg");
        if (_rootBg != null) {
            return;
        }

        _rootBg = new GameObject("CanvasBg");
        Canvas canvas = _rootBg.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        CanvasScaler scaler = _rootBg.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(GameConfig.SCREEN_WIDTH, GameConfig.SCREEN_HEIGHT);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
        scaler.referencePixelsPerUnit = 100;

        _rootBg.AddComponent<GraphicRaycaster>();
    }

    private void InitUICanvas()
    {
        _root = GameObject.Find("CanvasNormal");
        if (_root != null) {
            return;
        }
        _root = new GameObject("CanvasNormal");
        Canvas canvas = _root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;

        CanvasScaler scaler = _root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(GameConfig.SCREEN_WIDTH, GameConfig.SCREEN_HEIGHT);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        scaler.referencePixelsPerUnit = 100;

        _root.AddComponent<GraphicRaycaster>();
    }

    private void InitTopmostCanvas()
    {
        _rootTopmost = GameObject.Find("CanvasTopmost");
        if (_rootTopmost != null) {
            return;
        }

        _rootTopmost = new GameObject("CanvasTopmost");
        Canvas canvas = _rootTopmost.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;

        CanvasScaler scaler = _rootTopmost.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(GameConfig.SCREEN_WIDTH, GameConfig.SCREEN_HEIGHT);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        scaler.referencePixelsPerUnit = 100;

        _rootTopmost.AddComponent<GraphicRaycaster>();
    }

    // 打开窗口 通过callback可以在回调中获取界面对象，param可以传递数据给OnBindData函数
    public void OpenWindow<T>(OpenWindowHandler callback) where T : UIWindow
    {
        string winName = GetWindowName<T>();
        if (string.IsNullOrEmpty(winName)) {
            return;
        }

        UIWindow openedWindow = FindWindow<T>();

        if (openedWindow != null) {
            if (!openedWindow.IsShown) {
                // 如果窗口有隐藏，则直接显示，如果已经处于显示状态，则直接忽略
                openedWindow.OnBindData(null);
                openedWindow.OnRefreshWindow();
                openedWindow.Show();
                openedWindow.PlayOpenAnimation();

                // TODO 看实际情况决定是否播放开启音效

                // 响应外部回调事件
                if (callback != null) {
                    callback(openedWindow);
                }
            }
        } else {
            string path = "GUI/" + winName;

            GameObject prefab = Resources.Load(path) as GameObject;

            GameObject go = Instantiate(prefab) as GameObject;
            InitWindow(winName, go, callback, null);
        }
    }

    // 不推荐使用
    public void OpenWindow(string winName, params object[] param)
    {
        UIWindow openedWindow = FindWindow(winName);
        if (openedWindow != null) {
            if (!openedWindow.IsShown) {
                // 如果窗口有隐藏，则直接显示，如果已经处于显示状态，则直接忽略
                openedWindow.OnBindData(param);  // 设置 UI 窗口的数据
                openedWindow.OnRefreshWindow();
                openedWindow.Show();
                openedWindow.PlayOpenAnimation();  // 窗口显示动画

                // TODO 看实际情况决定是否播放开启音效
            }
        } else {
            string path = "GUI/" + winName;

            GameObject prefab = Resources.Load(path) as GameObject;

            GameObject go = Instantiate(prefab) as GameObject;
            InitWindow(winName, go, null, param);
        }
    }

    // 推荐使用，更加清晰，合理利用编译器检查防止拼写错误
    public void OpenWindow<T>(params object[] param) where T : UIWindow
    {
        string winName = GetWindowName<T>();  // 类名与 UI 窗口同名
        if (string.IsNullOrEmpty(winName)) {
            return;
        }

        OpenWindow(winName, param);
    }

    private void InitWindow(string winName, GameObject go, OpenWindowHandler callback, params object[] param)
    {
        UIWindow window = go.GetComponent<UIWindow>();
        if (!window) return;

        Transform parent = _root.transform;

        // 根据层级配置决定附加在哪个层
        if (window.Layer == UIWindow.WindowLayer.NORMAL) {
            parent = _root.transform;
        } else if (window.Layer == UIWindow.WindowLayer.TOPMOST) {
            parent = _rootTopmost != null ? _rootTopmost.transform : _root.transform;
        } else if (window.Layer == UIWindow.WindowLayer.BG) {
            parent = _rootBg != null ? _rootBg.transform : _root.transform;
        }

        // 添加Selectable防止穿透
        if (!window.IgnoreRaycast) {
            Selectable selectable = go.GetComponent<Selectable>();
            if (selectable == null) {
                selectable = go.AddComponent<Selectable>();
            }

            selectable.transition = Selectable.Transition.None;
        }

        // TODO 看看如何统一局部坐标和全局坐标
        if (window.StartPosition.sqrMagnitude > 0) {
            window.transform.localPosition = window.StartPosition;
        } else {
            go.transform.localPosition = Vector3.zero;
        }

        if (window.IsShowBackground || window.BackgroundEvent != UIWindow.ClickEvent.NONE) {
            GameObject goBackground = new GameObject(winName + "_BG");
            RectTransform rt = goBackground.AddComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = Vector3.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            Image image = goBackground.AddComponent<Image>();

            if (window.IsShowBackground) {
                image.color = window.BackgroundColor;
            } else {
                image.color = new Color(0, 0, 0, 0);
            }

            Button btn = goBackground.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(window.OnClickBackground);

            go.transform.SetParent(goBackground.transform, false);
            goBackground.transform.SetParent(parent, false);

            window.WindowObject = goBackground;
        } else {
            go.transform.SetParent(parent, false);
            window.WindowObject = go;
        }

        // 如果设定为独立互斥窗口，则关闭其他界面
        if (window.IsMutexWindow) {
            CloseAllWindow(winName);
        }

        go.name = winName;
        go.transform.localScale = Vector3.one;
        go.SetActive(true);

        _openedWindows.Add(window);

        window.HasInit = true;
        window.OnOpenWindow();
        window.OnBindData(param);
        window.OnRefreshWindow();
        window.PlayOpenAnimation();

        if (!string.IsNullOrEmpty(window.OpenSound)) {
            AudioController.PlaySound(window.OpenSound);
        }

        // 响应外部回调事件
        if (callback != null) {
            callback(window);
        }

        if (window.Layer == UIWindow.WindowLayer.TOPMOST) {
            Utils.SetLayer(window.WindowObject.transform, UITop);
        }

        if (window.BackgroundBlur) {
            EnableBlur(true);
        }
    }

    // 查找界面
    public UIWindow FindWindow<T>() where T : UIWindow
    {
        string winName = GetWindowName<T>();
        if (string.IsNullOrEmpty(winName)) {
            return null;
        }
        return FindWindow(winName);
    }

    public UIWindow FindWindow(string winName)
    {
        if (string.IsNullOrEmpty(winName)) {
            return null;
        }

        for (int i = 0; i < _openedWindows.Count; ++i) {
            UIWindow window = _openedWindows[i];
            if (window != null && window.name == winName) {
                return window;
            }
        }

        return null;
    }

    private string GetWindowName<T>() where T : UIWindow
    {
        // 获取一个类的名字
        FieldInfo fieldInfo = typeof(T).GetField("Name");
        if (fieldInfo == null) {
            Log.Error("错误，每个继承自UIWindow的界面都应该有一个static string Name = 'GUI路径',    {0}", typeof(T).Name);
            return null;
        }

        string winName = fieldInfo.GetValue(null) as string;
        return winName;
    }

    // 关闭界面
    public void CloseWindow<T>() where T : UIWindow
    {
        UIWindow window = FindWindow<T>();
        if (window) {
            window.CloseWindow();
        }
    }

    // 将界面从队列中移除
    public void RemoveWindow(string winName)
    {
        _openedWindows.RemoveAll(x => x.name == winName);
    }

    // 关闭所有非MainWindow的界面
    public void CloseAllWindow(string excludeWindow = null)
    {
        List<UIWindow> temp = new List<UIWindow>();
        temp.AddRange(_openedWindows);

        for (int i = 0; i < temp.Count; ++i) {
            UIWindow window = temp[i];
            if (window != null && !window.IsMainWindow && window.name != excludeWindow) {
                window.CloseWindow();
            }
        }
    }

    // 切换场景，关闭所有界面
    public void OnChangeScene()
    {
        List<UIWindow> temp = new List<UIWindow>();
        temp.AddRange(_openedWindows);

        for (int i = 0; i < temp.Count; ++i) {
            UIWindow window = temp[i];
            window.CloseWindow();
        }
    }

    // 刷新界面
    public void RefreshWindow<T>() where T : UIWindow
    {
        UIWindow window = FindWindow<T>();
        if (window) {
            window.OnRefreshWindow();
        }
    }

    // 切换到战斗界面，隐藏自己的Canvas
    public void ShowCanvas(bool show)
    {
        _root.SetActive(show);
        _rootBg.SetActive(show);
        _rootTopmost.SetActive(show);
    }

    public void EnableBlur(bool enable)  // 背景模糊处理
    {
        BlurOptimized effect = _uiCamera.GetComponent<BlurOptimized>();
        if (effect != null) {
            effect.enabled = enable;
        }

        BlurOptimized effect2 = _modelCamera.GetComponent<BlurOptimized>();
        if (effect2 != null) {
            effect2.enabled = enable;
        }
    }
}