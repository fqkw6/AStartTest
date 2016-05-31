using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 竞技场最高排名奖励界面
public class UIPVPAwardView : UIWindow
{
    public const string Name = "PVP/UIPVPAwardView";
    public Text _txtHighRank;
    public UIListView _listView;

    public override void OnOpenWindow()
    {
        // 请求数据
        PVPManager.Instance.RequestReportInfo();
    }

    public override void OnRefreshWindow()
    {
        if (PVPManager.Instance.MyHighRank <= 0) {
            _txtHighRank.text = Str.Get("UI_PVP_NOT_IN_RANK");
        } else {
            _txtHighRank.text = PVPManager.Instance.MyHighRank.ToString();
        }

        List<int> list = new List<int>();
        foreach (var item in ArenaHistoryRankConfigLoader.Data) {
            list.Add(item.Key);
        }

        _listView.Data = list.ToArray();
        _listView.Refresh();
    }
}
