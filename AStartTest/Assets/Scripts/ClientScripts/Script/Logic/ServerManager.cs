using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

// 服务器状态
public enum ServerStatusType
{
    UNKNOWN,
    COMMON,              // 普通
    RECOMMENT,           // 推荐
    NEW,                 // 最新
    FIRE_STORM,          // 火爆
}

// 服务器状态
public enum ServerBusyType
{
    UNKNOWN,
    RUN_WELL,            // 普通
    FULL,                // 爆满
    BUSY,                // 繁忙
    HALTED,              // 关闭
}

// 服务器信息
public class ServerInfo
{
    public int id;
    public string name;
    public string ip;
    public int port;
    public string url;
    public ServerStatusType statusType;
    public ServerBusyType busyType;
}

// 大区
public class GroupInfo
{
    public int index;
    public string name;
    public List<ServerInfo> servers = new List<ServerInfo>();
}

// 服务器信息管理
public partial class ServerManager
{
    public static readonly  ServerManager Instance = new ServerManager();
    public List<ServerInfo> LoginServerList = new List<ServerInfo>();
    public List<ServerInfo> GameServerList = new List<ServerInfo>();

    public string AccountName;         // 用户名
    public long AccountID;             // 用户id
    public string SessionKey;       // 会话密钥
    public List<int> RegisteredGameServerList = new List<int>();        // 注册过账号的游戏服务器id 
    public string LoginState;

    private string _currentEmail;

    public string CurrentRouteServerIP;
    public int CurrentRoutePort;
    public string CurrentLoginServerIP;
    public int CurrentLoginServerPort;
    public string CurrentGameServerIP;
    public int CurrentGameServerPort;

    // 连接路由服务器
    public void ConnectToRouteServer(string serverIP, int serverPort)
    {
        Log.Info("连接路由服务器");

        NetworkManager.Instance.Close();
        SetLoginState(Str.Get("UI_LOGIN_STATE_ROUTE"));
        NetworkManager.Instance.Connect(serverIP, serverPort, (ConnectState state) =>
        {
            CurrentRouteServerIP = serverIP;
            CurrentRoutePort = serverPort;

            // 登陆路由服务器
            Log.Info(">路由服务器: " + state);
            if (state == ConnectState.CONNECTED) {
                // 登陆路由服成功，获取登陆服列表
                SetLoginState(Str.Get("UI_LOGIN_STATE_ROUTE_OK"));
                Net.Send(eCommand.LOGIN_SERVER_LIST);
            } else {
                SetLoginState(Str.Get("UI_LOGIN_STATE_ROUTE_FAIL"));
            }
        });
    }

    private void SetLoginState(string txt)
    {
        LoginState = Str.Get("UI_LOGIN_STATE_ROUTE_FAIL");
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_LOGIN_STATE);
    }

    // 连接登录服务器
    public void ConnectToLoginServer()
    {
        NetworkManager.Instance.Close();
        if (LoginServerList.Count == 0) {
            Log.Error("登录服务器列表为空");
            return;
        }

        ServerInfo info = LoginServerList[0];
        Log.Info("连接登录服务器");
        SetLoginState(Str.Get("UI_LOGIN_STATE_LOGIN"));
        NetworkManager.Instance.Connect(info.ip, info.port, (state) =>
        {
            CurrentLoginServerIP = info.ip;
            CurrentLoginServerPort = info.port;

            Log.Info("登录服务器" + state);
            if (state == ConnectState.CONNECTED) {
                SetLoginState(Str.Get("UI_LOGIN_STATE_LOGIN_OK"));
                // 连接登录服务器成功，可以显示输入账号密码的界面了。玩家输入账号密码进行确认
                EventDispatcher.TriggerEvent(EventID.EVENT_UI_CONNECT_LOGIN_SERVER_OK);
            } else {
                SetLoginState(Str.Get("UI_LOGIN_STATE_LOGIN_FAIL"));
            }
        });
    }

    // 当像登录服务器发送WHICH_GAME_SERVER后，无论成功还是失败都会断开连接，此时要重新连接登录服务器
    public void ReConnectToLoginServer()
    {
        NetworkManager.Instance.Close();

        if (LoginServerList.Count == 0) {
            Log.Error("登录服务器列表为空");
            return;
        }

        ServerInfo info = LoginServerList[0];

        NetworkManager.Instance.Connect(info.ip, info.port, (state) => {
            Debug.Log("重连接登录服务器" + state.ToString());
            // 账号登录
            Login();
        });
    }

    // 请求注册账号
    public void RegisterAccount(string usrName, string password, string email)
    {
        CurrentAccount = usrName;
        CurrentPsw = password;
        _currentEmail = email;

        Account builder = new Account();
        builder.name = CurrentAccount;
        builder.password = CurrentPsw;
        builder.email = _currentEmail;
        Net.Send(eCommand.REGISTER, builder);
    }

    // 注册成功后，直接发送登录消息
    public void Login()
    {
        Account builder = new Account();
        builder.name = CurrentAccount;
        builder.password = CurrentPsw;
        Net.Send(eCommand.LOGIN, builder);
    }

    // 向登录服务器通知，选择游戏服务器
    public void SelectGameServer(int serverID)
    {
        CurrentGameServerID = serverID;

        GameServer gbuilder = new GameServer();
        gbuilder.id = serverID;

        PWhichGameSrv builder = new PWhichGameSrv();
        builder.userId = AccountID;
        builder.gameSrv = gbuilder;
        builder.userName = AccountName;
        Net.Send(eCommand.WHICH_GAME_SERVER, builder);
    }
    
    // 进入游戏
    public void ConnectGameServer()
    {
        Log.Info("连接游戏服务器");
        NetworkManager.Instance.Close();
        ServerInfo info = GameServerList.Find((x) => x.id == CurrentGameServerID);
        if (info == null) {
            Log.Error("所选的游戏服务器不存在： " + CurrentGameServerID);
            return;
        }

        NetworkManager.Instance.Connect(info.ip, info.port, (state) => {
            CurrentGameServerIP = info.ip;
            CurrentGameServerPort = info.port;

            Log.Info("游戏服务器" + state);
            // 连接游戏服务器成功，可以发送消息了
            RequestCheckSession(AccountID, SessionKey);
        });
    }

    // 是否记住账号
    public bool RememberAccount
    {
        get
        {
            return PlayerPrefs.GetInt("rts_remember_account", 0) > 0;
        }
        set
        {
            PlayerPrefs.SetInt("rts_remember_account", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // 记住的账号
    private string _currentAccount;
    public string CurrentAccount
    {
        get
        {
            if (string.IsNullOrEmpty(_currentAccount) && RememberAccount) {
                _currentAccount = PlayerPrefs.GetString("rts_account");
            }

            return _currentAccount;
        }
        set
        {
            _currentAccount = value;
            if (RememberAccount) {
                PlayerPrefs.SetString("rts_account", value);
            } else {
                PlayerPrefs.DeleteKey("rts_account");
            }
            PlayerPrefs.Save();
        }
    }

    private string _currentPsw;
    public string CurrentPsw
    {
        get
        {
            if (string.IsNullOrEmpty(_currentPsw) && RememberAccount) {
                _currentPsw = PlayerPrefs.GetString("rts_psw");
            }
            return _currentPsw;
        }
        set
        {
            _currentPsw = value;
            if (RememberAccount) {
                PlayerPrefs.SetString("rts_psw", value);
            } else {
                PlayerPrefs.DeleteKey("rts_psw");
            }
            PlayerPrefs.Save();
        }
    }

    private int _currentGameServerID = -1;
    public int CurrentGameServerID
    {
        get
        {
            if (_currentGameServerID == -1) {
                _currentGameServerID = PlayerPrefs.GetInt("rts_server_id", -1);
            }
            return _currentGameServerID;
        }

        set
        {
            _currentGameServerID = value;
            PlayerPrefs.SetInt("rts_server_id", _currentGameServerID);
            PlayerPrefs.Save();
        }
    }

    public ServerInfo GetServerInfoByID(int id)
    {
        return GameServerList.Find((x) => x.id == id );
    }

    // 获取一个推荐的游戏服务器id，如果有登录过的服务器，优先选取上次登录的服务器，否则选择有账号的服务器，否则选择推荐服务器
    public ServerInfo GetRecommendServerID()
    {
        ServerInfo ret = null;
        
        if (GameServerList.Count <= 0) {
            return null;
        }

        // 有登录过服务器
        if (CurrentGameServerID != -1) {
            ret = GameServerList.Find((x) => x.id == CurrentGameServerID);
            if (ret != null) return ret;
        }

        // 获取一个注册过账号的游戏服务器
        if (RegisteredGameServerList.Count > 0) {
            ret = GameServerList.Find((x) => x.id == RegisteredGameServerList[0]);
            if (ret != null) return ret;
        }

        // 获取一个推荐的游戏服务器
        ret = GameServerList.Find(x => x.statusType == ServerStatusType.RECOMMENT);
        if (ret != null) return ret;

        // 获取一个最新的游戏服务器
        ret = GameServerList.Find(x => x.statusType == ServerStatusType.NEW);
        if (ret != null) return ret;

        // 随便给一个游戏服务器
        return GameServerList[0];
    }

    // 检测账号名是否合法
    public bool IsAccountValid(string txt)
    {
        if (string.IsNullOrEmpty(txt)) {
            return false;
        }

        if (txt.Length < GameConfig.MIN_ACCOUNT_SIZE || txt.Length > GameConfig.MAX_ACCOUNT_SIZE) {
            return false;
        }
        return true;
    }

    // 检测密码是否合法
    public bool IsPswValid(string txt)
    {
        if (string.IsNullOrEmpty(txt)) {
            return false;
        }

        if (txt.Length < GameConfig.MIN_PASSWORD_SIZE || txt.Length > GameConfig.MAX_PASSWORD_SIZE) {
            return false;
        }

        return true;
    }
}
