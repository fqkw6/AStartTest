using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 竞技场排行界面
public class UIPVPRankView : UIWindow
{
    public const string Name = "PVP/UIPVPRankView";
    public UIListView _listView;
    public Text _txtMyRank;
    public Text _txtName;
    public Text _txtGuildName;
    public Text _txtFightScore;

    public override void OnOpenWindow()
    {
        PVPManager.Instance.RequestRankInfo();
    }

    public override void OnRefreshWindow()
    {
        if (PVPManager.Instance.MyRank > 0) {
            _txtMyRank.text = PVPManager.Instance.MyRank.ToString();
        } else {
            _txtMyRank.text = Str.Get("UI_PVP_NOT_IN_RANK");
        }

        _listView.Data = PVPManager.Instance.RankList.ToArray();
        _listView.Refresh();

        _txtName.text = UserManager.Instance.RoleName;
        _txtGuildName.text = GuildManager.Instance.GuildName;
        _txtFightScore.text = UserManager.Instance.GetFightScore().ToString();
    }
}
