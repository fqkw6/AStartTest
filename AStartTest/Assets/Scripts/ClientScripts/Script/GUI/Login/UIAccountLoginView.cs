using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class EventID
{
    public const string EVENT_UI_LOGIN_STATE = "EVENT_UI_LOGIN_STATE";
}

// 账号登录界面
public class UIAccountLoginView : UIWindow
{
    public const string Name = "Login/UIAccountLoginView";
    public InputField _edtAccount;
    public InputField _edtPsw;
    public Toggle _rememberAccount;
    public RectTransform _panelLogin;
    public Text _txtLoginState;

    private bool _isConnectOK = false;

    public override void OnOpenWindow()
    {
        EventDispatcher.AddEventListener(EventID.EVENT_UI_CONNECT_LOGIN_SERVER_OK, OnConnectLoginServerOK);
        EventDispatcher.AddEventListener(EventID.EVENT_UI_ACCOUNT_LOGIN_OK, OnAccountLoginOK);
        EventDispatcher.AddEventListener(EventID.EVENT_UI_LOGIN_STATE, OnRefreshLoginState);

        if (Game.Instance.IsDevelop && !string.IsNullOrEmpty(Game.Instance.TestRouteServer)) {
            // 在测试的时候可以直接指定路由服
            ServerManager.Instance.ConnectToRouteServer(Game.Instance.TestRouteServer, GameConfig.SERVER_PORT);
        } else {
            ServerManager.Instance.ConnectToRouteServer(GameConfig.SERVER_IP, GameConfig.SERVER_PORT);
        }

        _panelLogin.gameObject.SetActive(false);
        _txtLoginState.gameObject.SetActive(true);
    }

    public override void OnRefreshWindow()
    {
        _rememberAccount.isOn = ServerManager.Instance.RememberAccount;

        if (ServerManager.Instance.RememberAccount) {
            _edtAccount.text = ServerManager.Instance.CurrentAccount;
            _edtPsw.text = ServerManager.Instance.CurrentPsw;
        }
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_CONNECT_LOGIN_SERVER_OK, OnConnectLoginServerOK);
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_ACCOUNT_LOGIN_OK, OnAccountLoginOK);
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_LOGIN_STATE, OnRefreshLoginState);
    }

    void Update()
    {
        if (_isConnectOK) {
            _isConnectOK = false;
            _panelLogin.gameObject.SetActive(true);
            _txtLoginState.gameObject.SetActive(false);
            OnRefreshWindow();
        }
    }
    // 连接登录服务器成功，显示登录界面
    private void OnConnectLoginServerOK()
    {
        _isConnectOK = true;
    }

    // 账号验证成功，显示登录界面
    private void OnAccountLoginOK()
    {
        UIManager.Instance.OpenWindow<UILoginView>();
        CloseWindow();
    }

    private void OnRefreshLoginState()
    {
        _txtLoginState.text = ServerManager.Instance.LoginState;
    }

    public void OnToggleRememberAccount(bool value)
    {
        ServerManager.Instance.RememberAccount = _rememberAccount.isOn;
    }

    public void OnClickQuickLogin()
    {
        // 游客登录
    }

    // 注册账号
    public void OnClickRegister()
    {
        if (_edtAccount.text.Length < GameConfig.MIN_ACCOUNT_SIZE || _edtAccount.text.Length > GameConfig.MAX_ACCOUNT_SIZE) {
            UIUtil.ShowMsgFormat("MSG_LOGIN_ACCOUNT_SIZE_LIMIT", GameConfig.MIN_ACCOUNT_SIZE, GameConfig.MAX_ACCOUNT_SIZE);
            return;
        }

        if (_edtPsw.text.Length < GameConfig.MIN_PASSWORD_SIZE || _edtPsw.text.Length > GameConfig.MAX_PASSWORD_SIZE) {
            UIUtil.ShowMsgFormat("MSG_LOGIN_PASSWORD_SIZE_LIMIT", GameConfig.MIN_PASSWORD_SIZE, GameConfig.MAX_PASSWORD_SIZE);
            return;
        }

        ServerManager.Instance.CurrentAccount = _edtAccount.text;
        ServerManager.Instance.CurrentPsw = _edtPsw.text;

        ServerManager.Instance.RegisterAccount(ServerManager.Instance.CurrentAccount, ServerManager.Instance.CurrentPsw, "");
    }

    public void OnClickLogin()
    {
        if (!ServerManager.Instance.IsAccountValid(_edtAccount.text)) {  // 检测账号名是否合法
            UIUtil.ShowMsgFormat("MSG_LOGIN_ACCOUNT_ERROR");
            return;
        }

        if (!ServerManager.Instance.IsPswValid(_edtPsw.text)) {
            UIUtil.ShowMsgFormat("MSG_LOGIN_PASSWORD_ERROR");
            return;
        }

        // 登录账号
        ServerManager.Instance.CurrentAccount = _edtAccount.text;
        ServerManager.Instance.CurrentPsw = _edtPsw.text;
        ServerManager.Instance.Login();
    }
}
