using UnityEngine;
using System.Collections;

// 公会成员数据
public class GuildMemberInfo
{
    public long EntityID;
    public string Name;
    public int Level;
    public int FightScore;  // 战斗力
    public int ActiveScore; // 活跃度
    public ElapseTime LoginTime;// 上次登录时间
    public GuildPosition Position;    // 职位
}

// 公会请求
public class GuildApplyInfo
{
    public long EntityID;
    public string Name;
    public int Level;
    public int FightScore;
}

// 公会数据
public class GuildInfo
{
    public int EntityID;
    public string Name;
    public int Level;
    public long Money;
    public int MemberCount;
    public int TotalCount;
}
