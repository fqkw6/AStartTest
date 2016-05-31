using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

public class MailInfo
{
    public long EntityID;
    public long FromPlayerID;
    public string FromPlayerName;
    public ElapseTime SendTime;
    public string Title;
    public string Content;
    public List<ItemInfo> ItemList = new List<ItemInfo>();
    public bool HasGet;
     
    public void Deserialize(PMyMail data)
    {
        EntityID = data.myMessage.id;
        FromPlayerID = data.myMessage.fromId;
        FromPlayerName = data.myMessage.fromName;
        SendTime.SetTime(Utils.GetSeconds(data.myMessage.time));
        Title = data.myMessage.subject;
        Content = data.myMessage.content;
        foreach (var item in data.myMessage.extraItems) {
            ItemInfo info = new ItemInfo();
            info.Deserialize(item);
            ItemList.Add(info);
        }

        HasGet = data.myMessage.isObtain;
    }
}

// 邮件管理
public class MailManager
{
    public static readonly MailManager Instance = new MailManager();
    public List<MailInfo> MailList = new List<MailInfo>();

    public void RegisterMsg()
    {
        Net.Register(eCommand.MAIL_LIST, OnMsgMailList);
        Net.Register(eCommand.PUSH_COMMON_MAIL, OnMsgPushCommonMail);
    }

    // 请求邮件列表
    public void RequestMailList()
    {
        Net.Send(eCommand.MAIL_LIST);
    }

    private void OnMsgMailList(byte[] buffer)
    {
        PMailList ret = Net.Deserialize<PMailList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.MAIL_LIST)) return;

        // 添加邮件
        MailManager.Instance.MailList.Clear();
        foreach (var item in ret.mails) {
            MailInfo info = new MailInfo();
            info.Deserialize(item);
            MailManager.Instance.MailList.Add(info);
        }

        UIManager.Instance.RefreshWindow<UIMailView>();
    }

    private void OnMsgPushCommonMail(byte[] buffer)
    {
        PMailList ret = Net.Deserialize<PMailList>(buffer);
        if (!Net.CheckErrorCode(ret.errorCode, eCommand.PUSH_COMMON_MAIL)) return;

        // 添加邮件
        MailManager.Instance.MailList.Clear();
        foreach (var item in ret.mails) {
            MailInfo info = new MailInfo();
            info.Deserialize(item);
            MailManager.Instance.MailList.Add(info);
        }

        UIManager.Instance.RefreshWindow<UIMailView>();
    }

    // 请求读取邮件
    public void RequestReadMail(long mailID)
    {
        
    }

    // 请求领取邮件奖励
    public void RequestGetAward(long mailID)
    {
        
    }

    // 请求领取所有邮件的奖励
    public void RequestGetAwardAll()
    {
        
    }
}
