using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 邮件列表界面
public class UIMailView : UIWindow
{
    public const string Name = "Mail/UIMailView";
    public UIListView _listView;
    public Text _txtMailNumber;

    public override void OnRefreshWindow()
    {
        _listView.Data = MailManager.Instance.MailList.ToArray();
        _listView.Refresh();
        _txtMailNumber.text = string.Format("{0}/{1}",MailManager.Instance.MailList.Count,GameConfig.MAIL_MAX_COST);
    }

    // 一键领取
    public void OnClickGet()
    {
        
    }
}
