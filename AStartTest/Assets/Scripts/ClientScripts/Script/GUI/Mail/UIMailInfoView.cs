using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 邮件界面
public class UIMailInfoView : UIWindow
{
    public const string Name = "Mail/UIMailInfoView";
    public Text _txtTitle;
    public Text _txtContent;
    public SimpleItemWidget[] _itemWidgets;
    public Image _imgBg;

    private MailInfo _info;

    public override void OnBindData(params object[] param)
    {
        _info = (MailInfo) param[0];
    }

    public override void OnRefreshWindow()
    {
        _txtTitle.text = _info.Title;
        _txtContent.text = _info.Content;

        for (int i = 0; i < _itemWidgets.Length; ++i) {
            if (i < _info.ItemList.Count) {
                _itemWidgets[i].gameObject.SetActive(true);
                _itemWidgets[i].SetInfo(_info.ItemList[i]);
            } else {
                _itemWidgets[i].gameObject.SetActive(false);
            }
        }

        _imgBg.gameObject.SetActive(_info.ItemList.Count > 0);
    }

    public void OnClickAward()
    {
        
    }
}
