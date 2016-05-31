using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 战报界面
public class UIPVPReportView : UIWindow
{
    public const string Name = "PVP/UIPVPReportView";
    public UIListView _listView;

    public override void OnOpenWindow()
    {
        // 请求数据
        PVPManager.Instance.RequestReportInfo();
    }

    public override void OnRefreshWindow()
    {
        _listView.Data = PVPManager.Instance.ReportList.ToArray();
        _listView.Refresh();
    }
}
