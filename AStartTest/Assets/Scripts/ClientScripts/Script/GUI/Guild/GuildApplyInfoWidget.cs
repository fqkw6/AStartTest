using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 公会申请列表组件
public class GuildApplyInfoWidget : ListItemWidget
{
    public Toggle _toggleCheck;
    public Text _txtName;
    public Text _txtLevel;
    public Text _txtFightScore;

    public GuildApplyInfo _info;

    public override void SetInfo(object data)
    {
        _info = (GuildApplyInfo)data;

        _txtName.text = _info.Name;
        _txtLevel.text = _info.Level.ToString();
        _txtFightScore.text = _info.FightScore.ToString();
    }
}
