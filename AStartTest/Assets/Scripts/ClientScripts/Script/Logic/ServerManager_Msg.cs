using UnityEngine;
using System.Collections;
using comrt.comnet;

public partial class ServerManager
{
    public void RegisterMsg()
    {
        Net.Register(eCommand.CHECK_SESSION, OnMsgCheckSession);
        Net.Register(eCommand.CREATE_NEW_ROLE, OnMsgCreateNewRole);
        Net.Register(eCommand.GET_PLAYER_INFO, OnMsgGetPlayerInfo);
        Net.Register(eCommand.JOIN, OnMsgJoin);
        Net.Register(eCommand.LOGIN, OnMsgLogin);
        Net.Register(eCommand.LOGIN_SERVER_LIST, OnMsgLoginServerList);
        Net.Register(eCommand.REGISTER, OnMsgRegister);
        Net.Register(eCommand.WHICH_GAME_SERVER, OnMsgWhichGameServer);
    }

    public void RequestCheckSession(long userID, string session)
    {
        PCheckSession builder = new PCheckSession();
        builder.userId = userID;
        builder.session_key = session;
        Net.Send(eCommand.CHECK_SESSION, builder);
    }

    private void OnMsgCheckSession(byte[] buffer)
    {
        PSessionAnswer ret = Net.Deserialize<PSessionAnswer>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.CHECK_SESSION)) return;

        // sessionkey检查成功，可以正常的跟游戏服务器进行通信了
        Log.Info("--登录游戏服务器成功");

        // 请求玩家数据
        UserManager.Instance.RequestUserInfo();
    }

    public void RequestCreateNewRole(string roleName, int roleConfigID, int sex)
    {
        PCreateRole builder = new PCreateRole();
        builder.userId = AccountID;
        builder.serverId = CurrentGameServerID;
        builder.sex = (eSex)sex;
        builder.playerName = roleName;
        builder.plyaerCfgId = roleConfigID;
        Net.Send(eCommand.CREATE_NEW_ROLE, builder);
    }

    private void OnMsgCreateNewRole(byte[] buffer)
    {
        PPlayerInfo ret = Net.Deserialize<PPlayerInfo>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.CREATE_NEW_ROLE)) return;

        UserManager.Instance.Deserialize(ret);

        // 加入游戏
        UserManager.Instance.JoinGame();

        UIManager.Instance.CloseWindow<UISelectServerView>();
    }

    private void OnMsgGetPlayerInfo(byte[] buffer)
    {
        PPlayerInfo ret = Net.Deserialize<PPlayerInfo>(buffer);
        if (ret.errorCode != 0) {
            // 创建新的角色
            if (ret.errorCode == (int)eErrorCode.NO_PLAYER_ERROR) {
                UIManager.Instance.OpenWindow<UICreateRoleView>();
                return;
            }
        }

        if (!Net.CheckErrorCode(ret.errorCode, eCommand.GET_PLAYER_INFO)) return;

        UserManager.Instance.Deserialize(ret);

        if (!Game.Instance.IsInGame) {
            // 加入游戏
            UserManager.Instance.JoinGame();
        }
    }

    private void OnMsgJoin(byte[] buffer)
    {
        CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
        if (!Net.CheckErrorCode(ret.err_code, eCommand.JOIN)) return;

        // 此时正式进入游戏
        Game.Instance.IsInGame = true;
        NetworkManager.Instance.NeedToSendHeartBeat = true;
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_LOGIN_OK);

        UserManager.Instance.RequestUserInfo();
    }

    private void OnMsgLogin(byte[] buffer)
    {
        AccountAnswer ret = Net.Deserialize<AccountAnswer>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.LOGIN)) return;

        // 账号信息
        ServerManager.Instance.AccountName = ret.name;
        ServerManager.Instance.AccountID = ret.user_id;
        ServerManager.Instance.SessionKey = ret.session_key;
        ServerManager.Instance.RegisteredGameServerList.Clear();
        ServerManager.Instance.RegisteredGameServerList.AddRange(ret.gmServerIds);

        // 游戏服务器列表
        ServerManager.Instance.GameServerList.Clear();
        foreach (var item in ret.servers) {
            ServerInfo info = new ServerInfo();
            info.id = item.id;
            info.name = item.name;
            info.ip = item.ipv4;
            info.port = item.port;
            info.statusType = (ServerStatusType)item.serverType;
            info.busyType = (ServerBusyType)item.busy_degree;
            info.url = item.domain_name;

            ServerManager.Instance.GameServerList.Add(info);
        }

        // TODO UI刷新游戏服务器列表
        Log.Info("登录成功");
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_ACCOUNT_LOGIN_OK);
    }

    private void OnMsgLoginServerList(byte[] buffer)
    {
        PRServerList ret = Net.Deserialize<PRServerList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.LOGIN_SERVER_LIST)) return;

        ServerManager.Instance.LoginServerList.Clear();

        foreach (var item in ret.servers) {
            Log.Info("登录服务器: " + item.ip);
            ServerInfo info = new ServerInfo();
            info.id = item.id;
            info.name = item.name;
            info.ip = item.ip;
            info.port = item.port;
            info.url = item.url;
            info.statusType = (ServerStatusType)item.type;
            ServerManager.Instance.LoginServerList.Add(info);
        }

        Log.Info("获取登陆服列表成功");

        // 获取登陆服列表成功，登录
        ServerManager.Instance.ConnectToLoginServer();
    }

    private void OnMsgRegister(byte[] buffer)
    {
        AccountAnswer ret = Net.Deserialize<AccountAnswer>(buffer);

        if (!Net.CheckErrorCode(ret.errorCode, eCommand.REGISTER)) return;

        // 注册成功
        ServerManager.Instance.Login();
    }

    private void OnMsgWhichGameServer(byte[] buffer)
    {
        CommonAnswer ret = Net.Deserialize<CommonAnswer>(buffer);
        if (!Net.CheckErrorCode(ret.err_code, eCommand.WHICH_GAME_SERVER)) {
            // 重新连接登录服务器(此时登录服务器已经断开连接)
            ServerManager.Instance.ReConnectToLoginServer();
            return;
        };

        // 选择游戏服务器成功，可以直接连接游戏服务器
        ServerManager.Instance.ConnectGameServer();
    }
}