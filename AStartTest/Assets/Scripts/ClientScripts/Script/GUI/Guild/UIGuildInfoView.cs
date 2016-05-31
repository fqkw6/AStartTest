using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 公会信息界面
public class UIGuildInfoView : UIWindow
{
    public const string Name = "Guild/UIGuildInfoView";
    public Text _txtGuildName;
    public Text _txtID;
    public Text _txtLevel;
    public Text _txtMoney;
    public Text _txtActiveScore;
    public Text _txtMember;
    public InputField _inputAnnounce;
    public Button _btnApplyList;    // 只有会长副会长可见

    public UIListView _listView;

    public float _xStart = 20;
    public float _yStart = -20;
    public float _xOffset = 290;
    public float _yOffset = 150;

    public override void OnOpenWindow()
    {
    }

    public override void OnRefreshWindow()
    {
        _txtGuildName.text = GuildManager.Instance.GuildName;
        _inputAnnounce.text = GuildManager.Instance.GuildAnnounce;
        _txtLevel.text = GuildManager.Instance.GuildLevel.ToString();
        _txtMoney.text = Utils.GetMoneyString(GuildManager.Instance.GuildMoney);
        _txtActiveScore.text = GuildManager.Instance.GuildActiveScore.ToString();
        _txtMember.text = string.Format("{0}/{1}", GuildManager.Instance.MemberList.Count, GuildManager.Instance.GuildMemberMaxCount);

        // 打开界面的时候按默认规则进行排序
        GuildManager.Instance.SortMember(SortType.SORT_DEFAULT);
    }

    private void RefreshList()
    {
        _listView.Data = GuildManager.Instance.MemberList.ToArray();
        _listView.Refresh();
    }

    // 打开贡献界面
    public void OnClickDonate()
    {
        GuildManager.Instance.RequestGuildDonate();
    }

    // 打开申请列表界面
    public void OnClickApplyList()
    {
        // 只有会长和副会长可以查看申请列表
        if (GuildManager.Instance.GuildPosition > GuildPosition.VICE_CHAIRMAN) {
            return;
        }
        
        UIManager.Instance.OpenWindow<UIGuildApplyView>();
    }

    // 申请退出公会
    public void OnClickQuitGuild()
    {
        GuildManager.Instance.RequestQuitGuild();
    }

    // 公会副本
    public void OnClickGuildCopy()
    {
        
    }

    // 公会战
    public void OnClickGuildWar()
    {
        
    }

    // 公会领地
    public void OnClickGuildLand()
    {
        
    }

    // 公会训练营
    public void OnClickGuildTrain()
    {
        
    }

    // 按名字排序
    public void OnClickSortByName()
    {
        GuildManager.Instance.SortMember(SortType.SORT_BY_NAME);
        RefreshList();
    }

    // 按等级排序
    public void OnClickSortByLevel()
    {

        GuildManager.Instance.SortMember(SortType.SORT_BY_LEVEL);
        RefreshList();
    }

    // 按职位排序
    public void OnClickSortByPosition()
    {
        GuildManager.Instance.SortMember(SortType.SORT_BY_POSITION);
        RefreshList();

    }

    // 按战斗力排序
    public void OnClickSortByFightScore()
    {
        GuildManager.Instance.SortMember(SortType.SORT_BY_FIGHT_SCORE);
        RefreshList();

    }

    // 按活跃度排序
    public void OnClickSortByActiveScore()
    {
        GuildManager.Instance.SortMember(SortType.SORT_BY_ACTIVE_SCORE);
        RefreshList();

    }

    // 按登陆时间排序
    public void OnClickSortByTime()
    {
        GuildManager.Instance.SortMember(SortType.SORT_BY_LOGIN_TIME);
        RefreshList();
    }
}
