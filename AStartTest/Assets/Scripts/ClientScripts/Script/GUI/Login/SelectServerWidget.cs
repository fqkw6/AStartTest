using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectServerWidget : ListItemWidget
{
    public bool _isListWidget = true;
    public Text _txtServerName;
    public Image _imgFlag;
    public Image _imgStatus;
    public Sprite[] _sprStatus;

    private ServerInfo _currentInfo = null;


    public override  void Awake()
    {
        if (_isListWidget) {
            base.Awake();
        }        
    }

    public override void SetInfo(object info)
    {
        _currentInfo = info as ServerInfo;
        if (_currentInfo == null) return;

        _txtServerName.text = _currentInfo.name.ToString();
        _imgStatus.gameObject.SetActive(true);

        switch (_currentInfo.busyType) {
            case ServerBusyType.UNKNOWN:
                _imgStatus.gameObject.SetActive(false);
                break;
            case ServerBusyType.RUN_WELL:
                _imgStatus.sprite = _sprStatus[1];
                break;
            case ServerBusyType.BUSY:
                _imgStatus.sprite = _sprStatus[3];
                break;
            case ServerBusyType.FULL:
                _imgStatus.sprite = _sprStatus[0];
                break;
            case ServerBusyType.HALTED:
                _imgStatus.sprite = _sprStatus[2];
                break;
        }

        if (_imgFlag != null) _imgFlag.gameObject.SetActive(ServerManager.Instance.RegisteredGameServerList.IndexOf(_currentInfo.id) >= 0);
    }

    public override void OnClick()
    {
        if (_currentInfo.busyType == ServerBusyType.HALTED) {
            UIUtil.ShowMsgFormat("MSG_LOGIN_SERVER_CLOSE");
            return;
        }

        if (_currentInfo.busyType == ServerBusyType.FULL) {
            UIUtil.ShowMsgFormat("MSG_LOGIN_SERVER_FULL");
            return;
        }

        // 点击登录服务器
        ServerManager.Instance.SelectGameServer(_currentInfo.id);
        UIManager.Instance.CloseWindow<UISelectServerView>();
    }
}
