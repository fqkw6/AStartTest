using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

// 排序方式
public enum SortType
{
    SORT_DEFAULT,   // 打开界面的时候根据特殊规则排序，优先排列在线玩家，然后根据职位排序
    SORT_BY_NAME,
    SORT_BY_LEVEL,
    SORT_BY_POSITION,
    SORT_BY_FIGHT_SCORE,
    SORT_BY_ACTIVE_SCORE,
    SORT_BY_LOGIN_TIME,
}

// 公会职位
public enum GuildPosition
{
    CHAIRMAN,   // 会长
    VICE_CHAIRMAN,  // 副会长
    ELITE_MEMBER,   // 精英成员
    MEMBER, // 成员
    NEWBIE_MEMBER,  // 新手成员
}

// 公会管理
public partial class GuildManager
{
    public static readonly GuildManager Instance = new GuildManager();

    public List<GuildInfo> GuildList = new List<GuildInfo>();   // 所有公会列表（申请加入公会时请求）
    public List<GuildMemberInfo> MemberList = new List<GuildMemberInfo>();  // 当前公会的成员列表 
    public List<GuildApplyInfo> ApplyList = new List<GuildApplyInfo>(); // 申请加入公会的玩家列表 

    // 当前公会数据（GuildID为0，则没有公会）
    public int GuildID;
    public string GuildName;
    public int GuildLevel;
    public int GuildMoney;
    public int GuildActiveScore;
    public int GuildMemberCount;
    public int GuildMemberMaxCount;
    public int GuildFlagIndex;
    public int GuildFlagColorIndex;
    public int GuildFlagTextIndex;
    public string GuildFlagText;
    public string GuildAnnounce;
    public GuildPosition GuildPosition; // 玩家在公会中的职位

    private SortType _curSortType;

    // 根据职位获取其职位名称
    public string GetPositionText(GuildPosition position)
    {
        switch (GuildPosition) {
            case GuildPosition.CHAIRMAN:
                return Str.Get("UI_GUILD_POSITION_CHAIRMAN");
            case GuildPosition.VICE_CHAIRMAN:
                return Str.Get("UI_GUILD_POSITION_VICE_CHAIRMAN");
            case GuildPosition.ELITE_MEMBER:
                return Str.Get("UI_GUILD_POSITION_ELITE_MEMBER");
            case GuildPosition.MEMBER:
                return Str.Get("UI_GUILD_POSITION_MEMBER");
            case GuildPosition.NEWBIE_MEMBER:
                return Str.Get("UI_GUILD_POSITION_NEWBIE_MEMBER");
        }
        return "";
    }

    // 根据上次登录时间，获取应该正确显示的字符串
    public string GetLoginTimeString(float loginTime)
    {
        if (loginTime <= 0) {
            // 在线
            return Str.Get("UI_GUILD_ONLINE");
        } else if (loginTime <= 3600) {
            // 1小时内
            return Str.Get("UI_GUILD_HOUR_IN");
        } else if (loginTime <= 3600*24) {
            int hour = Mathf.FloorToInt(loginTime/3600);
            return Str.Format("UI_GUILD_HOUR", hour);
        } else {
            int day = Mathf.FloorToInt(loginTime/(3600*24));
            return Str.Format("UI_GUILD_DAY", day);
        }
    }

    // 排序成员
    public void SortMember(SortType sortType)
    {
        _curSortType = sortType;
        SortMember();
    }

    public void SortMember()
    {
        switch (_curSortType) {
            case SortType.SORT_BY_NAME:
                break;
        }
    }

    // 请求自己的公会数据
    public void RequestGuildInfo()
    {
        Net.Send(eCommand.ADJUST_PLAYER_LEVEL, (buffer) =>
        {
            PComItemList ret = Net.Deserialize<PComItemList>(buffer);
            if (!Net.CheckErrorCode(ret.errorCode, eCommand.ADJUST_PLAYER_LEVEL)) return;

            UIManager.Instance.RefreshWindow<UIGuildInfoView>();
        });
    }

    // 请求公会列表(没有公会的时候)
    public void RequestGuildList()
    {
        
    }

    // 请求公会申请人列表（只有会长、副会长有此权限）
    public void RequestGuildApplyList()
    {
        
    }

    // 同意申请
    public void RequestGuildApplyAgree(List<long> list)
    {
        
    }

    // 拒绝申请
    public void RequestGuildApplyRefuse(List<long> list)
    {
        
    }

    // 清空申请列表
    public void RequestGuildApplyClear()
    {
        
    }

    // 请求贡献
    public void RequestGuildDonate()
    {
    }

    // 请求退出公会
    public void RequestQuitGuild()
    {
        
    }

    // 请求踢人（会长、副会长）
    public void RequestKickout(long playerID)
    {
        
    }

    // 请求修改公会公告
    public void RequestModifyGuildAnnounce(string newText)
    {
        
    }

    // 请求修改公会旗帜
    public void RequestModifyFlag(int flagIndex, int flagColorIndex, int textColorIndex, string text)
    {
        
    }
}
