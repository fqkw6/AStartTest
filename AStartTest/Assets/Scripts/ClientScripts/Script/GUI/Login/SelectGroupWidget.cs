using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectGroupWidget : ListItemWidget
{
    public bool _isListWidget = true;
    public Image _imgFrame;
    public Text _txtGroupName;

    public GroupInfo _info;

    public override void Awake()
    {
        if (_isListWidget) {
            base.Awake();
        }
    }

    public override void SetInfo(object data)
    {
        _info = data as GroupInfo;
        if (_info == null) return;

        _txtGroupName.text = _info.name;
    }

    public override void OnSelect()
    {
        _imgFrame.gameObject.SetActive(true);
    }

    public override void OnUnselect()
    {
        _imgFrame.gameObject.SetActive(false);
    }
}
