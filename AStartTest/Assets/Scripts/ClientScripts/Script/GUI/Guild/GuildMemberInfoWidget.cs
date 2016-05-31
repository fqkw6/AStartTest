using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 公会成员列表组件
public class GuildMemberInfoWidget : ListItemWidget
{
    public Text _txtName;
    public Text _txtLevel;
    public Text _txtPosition;
    public Text _txtFightScore;
    public Text _txtActiveScore;
    public Text _txtTime;

    public Color _onlineColor;
    public Color _offlineColor;

    private GuildMemberInfo _info;

    public override void SetInfo(object data)
    {
        _info = (GuildMemberInfo) data;
        _txtName.text = _info.Name;
        _txtLevel.text = _info.Level.ToString();
        _txtPosition.text = GuildManager.Instance.GetPositionText(_info.Position);
        _txtFightScore.text = _info.FightScore.ToString();
        _txtActiveScore.text = _info.ActiveScore.ToString();
        _txtTime.text = GuildManager.Instance.GetLoginTimeString(_info.LoginTime.GetTime());

        if (_info.LoginTime.GetTime() > 0) {
            _txtTime.color = _offlineColor;
        } else {
            _txtTime.color = _onlineColor;
        }
    }
}
