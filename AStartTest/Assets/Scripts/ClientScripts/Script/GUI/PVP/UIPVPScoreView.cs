using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 竞技场积分奖励界面
public class UIPVPScoreView : UIWindow
{
    public const string Name = "PVP/UIPVPScoreView";
    public Text _txtCurrentScore;
    public UIListView _listView;

    public override void OnOpenWindow()
    {
        // 请求数据
        PVPManager.Instance.RequestReportInfo();
    }

    public override void OnRefreshWindow()
    {
        List<int> list = new List<int>();
        foreach (var item in ArenaScoreConfigLoader.Data) {
            list.Add(item.Key);
        }

        _listView.Data = list.ToArray();
        _listView.Refresh();

        _txtCurrentScore.text = PVPManager.Instance.MyScore.ToString();
    }
}

