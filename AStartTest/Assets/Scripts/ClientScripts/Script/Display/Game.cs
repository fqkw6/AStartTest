using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏主逻辑入口
/// </summary>
public class Game : Singleton<Game>
{
    private FSMStateMachine _fsm;
    public bool IsInGame = false; // 是否进入游戏，即可以正常收发消息
    public bool IsOffline = false;
    public bool IsDevelop = false;
    public GameObject MainCity = null;
    public Camera Camera = null;    // 主城的摄像机

    [NonSerialized]
    public bool HasInit = false;
    [NonSerialized]
    public bool BackFromBattle = false;

    public float _cityScaleFactor = 0.7f; // 当前缩放比例，暂时主城和大地图用一个，根据需求可以分开
    public float _worldScaleFactor = 0.7f;

    public string TestRouteServer;

    public float CityScaleFactor
    {
        get { return _cityScaleFactor; }

        set { _cityScaleFactor = Mathf.Clamp(value, 0.7f, 1.5f); }
    }

    public float WorldScaleFactor
    {
        get { return _worldScaleFactor; }

        set { _worldScaleFactor = Mathf.Clamp(value, 0.7f, 1.5f); }
    }

    private void Start()
    {
    }

    public void Init()
    {
        if (!HasInit) {
            // 初始化资源
            ResourceManager.Instance.Init(InitGame);  // 参数为资源初始化之后的回调（资源可能网络下载）
            return;
        }

        if (BackFromBattle) {
            // 从战斗中返回，切换到主界面
            EnterNormal();
        }

        BackFromBattle = false;
    }

    private void InitGame()
    {
        HasInit = true;

        _fsm = new FSMStateMachine(this);
        // 添加游戏的状态
        _fsm.AddState(StateID.GAME_STATE_LOGO, new GameStateLogo());
        _fsm.AddState(StateID.GAME_STATE_LOGIN, new GameStateLogin());
        _fsm.AddState(StateID.GAME_STATE_NORMAL, new GameStateNormal());
        _fsm.AddState(StateID.GAME_STATE_BATTLE, new GameStateBattle());

        InitConfig();
        RegisterMsg();  // 注册消息

        // 进入登录界面
        EnterLogin();

        NetworkManager.Instance.OnDisConnectCallback += OnDisConnect;
        BattleNetworkManager.Instance.OnDisConnectCallback += OnDisConnectBattleServer;
    }

    // 显示、隐藏主城
    public void ShowMainCity(bool show)
    {
        if (MainCity == null) {
            MainCity = GameObject.Find("MainCity");
        }

        if (MainCity != null) {
            MainCity.SetActive(show);
        }
    }

    // 控制主城摄像机的开启和关闭
    public void EnableGameCamera(bool enable)
    {
        if (Camera == null) {
            GameObject go = GameObject.Find("GameCamera");
            if (go != null) {
                Camera = go.GetComponent<Camera>();
            }
        }

        if (Camera != null) {
            Camera.gameObject.SetActive(enable);
        }
    }

    // 销毁主城资源
    public void DestroyMainCity()
    {
        if (MainCity != null) {
            Destroy(MainCity);
        }

        MainCity = null;
    }

    private void InitConfig()
    {
    }

    // 注册消息（注册用來响应服务器调用）
    private void RegisterMsg()
    {
        ServerManager.Instance.RegisterMsg();
        UserManager.Instance.RegisterMsg();
        GMManager.Instance.RegisterMsg();
        CityManager.Instance.RegisterMsg();
        WorldManager.Instance.RegisterMsg();
        PVEManager.Instance.RegisterMsg();
        PVPManager.Instance.RegisterMsg();
        MailManager.Instance.RegisterMsg();
        ShopManager.Instance.RegisterMsg();
    }

    // 当程序退出的时候
    public void OnApplicationQuit()
    {
        // 断开网络连接
        NetworkManager.Instance.Close();
        BattleNetworkManager.Instance.Close();
    }

    private void OnDisConnect()
    {
        UIUtil.ShowMsgBox(Str.Get("UI_MSG_NETWORK_DISCONNECT"), null, () => { ServerManager.Instance.ReConnectToLoginServer(); }, false, true);
    }

    private void OnDisConnectBattleServer()
    {
        UIUtil.ShowMsgBox(Str.Get("UI_MSG_NETWORK_DISCONNECT"), null, () => { }, false, true);
    }

    // 进入登陆界面
    public void EnterLogin()
    {
        _fsm.ChangeState(StateID.GAME_STATE_LOGIN);
    }

    // 进入主界面
    public void EnterNormal()
    {
        _fsm.ChangeState(StateID.GAME_STATE_NORMAL);
    }

    // 开始战斗
    public void EnterBattle()
    {
        _fsm.ChangeState(StateID.GAME_STATE_BATTLE);
    }

    private void Update()
    {
        // 同步网络消息
        float dt = Time.deltaTime;

        // 网络消息实时接收
        NetworkManager.Instance.OnUpdate(dt);
        BattleNetworkManager.Instance.OnUpdate(dt);

        // 这里不需要考虑同步与帧锁定，主要是外部非战斗逻辑使用，为保持时间概念一致，统一使用毫秒计时
        Timer.OnTick((int)(dt * 1000));

        // 非战斗的逻辑随意处理
        if (_fsm != null) _fsm.UpdateFSM(dt);

        // 返回键
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        // Home键
        if (Input.GetKeyDown(KeyCode.Home)) {
            Application.Quit();
        }
    }

    // 反作弊检测
    private void OnSpeedHackDetected()
    {
        Log.Warning("Speed hack detected!");
    }

    private void OnInjectionDetected()
    {
        Log.Warning("Injection detected!");
    }

    private void OnObscuredTypeCheatingDetected()
    {
        Log.Warning("Obscured type cheating detected!");
    }
}