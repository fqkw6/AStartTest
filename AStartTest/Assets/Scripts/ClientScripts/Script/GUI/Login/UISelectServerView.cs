using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 选择服务器界面
public class UISelectServerView : UIWindow
{
    public const string Name = "Login/UISelectServerView";
    public SelectServerWidget _lastLoginServer;
    public RectTransform _panelArea;
    public RectTransform _panelMy;

    public SelectGroupWidget _myServer;

    public UIListView _listGroup;
    public UIListView _listRecomment;
    public UIListView _listServer;

    public override void OnOpenWindow()
    {
    }

    public override void OnRefreshWindow()
    {
        int lastLoginServer = ServerManager.Instance.CurrentGameServerID;
        if (lastLoginServer > 0) {
            _lastLoginServer.gameObject.SetActive(true);
            ServerInfo info = ServerManager.Instance.GetServerInfoByID(lastLoginServer);
            if (info != null) {
                _lastLoginServer.SetInfo(info);
            }
        } else {
            _lastLoginServer.gameObject.SetActive(false);
        }
        
        UpdateGroupList();
        UpdateRecommentServerList();
        UpdateServerList();

        _panelArea.gameObject.SetActive(false);
        _panelMy.gameObject.SetActive(true);

        _listGroup.SelectWidget(null);
    }

    public override void OnCloseWindow()
    {
    }

    private void UpdateGroupList()
    {        
        List<GroupInfo> groupData = new List<GroupInfo>();
        GroupInfo data = new GroupInfo();
        data.index = 0;
        data.name = Str.Format("UI_SERVER_GROUP", Str.Get("UI_NUMBER_1"));
        data.servers.AddRange(ServerManager.Instance.GameServerList);
        groupData.Add(data);

        _listGroup.OnClickListItem = OnClickGroup;
        _listGroup.Data = groupData.ToArray();
        _listGroup.Refresh();
    }

    private void UpdateServerList()
    {
        // TODO 将来再分区
        _listServer.Data = ServerManager.Instance.GameServerList.ToArray();
        _listServer.Refresh();
    }

    private void UpdateRecommentServerList()
    {
        _listRecomment.Data = new []
        {
            ServerManager.Instance.GetRecommendServerID()
        };

        _listRecomment.Refresh();
    }

    public void OnClickGroup(int index, ListItemWidget widget)
    {
        _myServer.OnUnselect();
        _listGroup.SelectWidget(widget);

        _panelArea.gameObject.SetActive(true);
        _panelMy.gameObject.SetActive(false);
    }

    public void OnClickMyServer()
    {
        _myServer.OnSelect();
        _listGroup.SelectWidget(null);

        _panelArea.gameObject.SetActive(false);
        _panelMy.gameObject.SetActive(true);
    }
}
