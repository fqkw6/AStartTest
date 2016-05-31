using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MailListItemWidget : ListItemWidget
{
    public SimpleItemWidget _itemWidget;
    public Text _txtTitle;
    public Text _txtFrom;
    public Text _txtTime;
    public Image _imgRead;
    public Image _imgNotRead;

    private MailInfo _info;
    void Start()
    {
	
	}

    public override void SetInfo(object info)
    {
        _info = (MailInfo)info;

        if (_info.ItemList.Count > 0) {
            _itemWidget.SetInfo(_info.ItemList[0]);
            _itemWidget.gameObject.SetActive(true);
        } else {
            _itemWidget.gameObject.SetActive(false);
        }

        _txtTitle.text = Str.Format("UI_MAIL_TITLE", _info.Title);
        _txtFrom.text = Str.Format("UI_MAIL_FROM", _info.FromPlayerName);
        _txtTime.text = Str.Format("UI_MAIL_TIME", _info.SendTime.GetTime());
        _imgRead.gameObject.SetActive(_info.HasGet);
        _imgNotRead.gameObject.SetActive(!_info.HasGet);
    }

    public override void OnClick()
    {
        UIManager.Instance.OpenWindow<UIMailInfoView>(_info);   
    }
}
