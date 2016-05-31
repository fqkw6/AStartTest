using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 登录界面
public class UILoginView : UIWindow
{
    public const string Name = "Login/UILoginView";
    public Text _serverName;
    private ServerInfo _info = null;

    public override void OnOpenWindow()
    {
        _info = ServerManager.Instance.GetRecommendServerID();
        if (_info != null) {
            _serverName.text = _info.name;
        } else {
            _serverName.text = "";
        }

        EventDispatcher.AddEventListener(EventID.EVENT_UI_LOGIN_OK, OnLoginOK);
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_LOGIN_OK, OnLoginOK);
    }

    private void OnLoginOK()
    {
        Game.Instance.EnterNormal();
        CloseWindow();
    }

    public void OnClickSelectServer()
    { 
        // 打开选服界面
        UIManager.Instance.OpenWindow<UISelectServerView>();
    }

    public void OnClickLogin()
    {
        if (_info == null) {
            Log.Error("尚未选择游戏服务器");
            return;
        }

        // 登录游戏服务器（给登录服务器发送选择游戏服务器的消息，在它的返回消息里面处理登录连接逻辑）
        ServerManager.Instance.SelectGameServer(_info.id);
    }
}
