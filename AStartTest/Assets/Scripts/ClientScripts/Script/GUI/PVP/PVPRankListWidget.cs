using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 竞技场排行榜列表组件
public class PVPRankListWidget : ListItemWidget
{
    public Image _imgRank;
    public Text _txtRank;
    public Image _imgIcon;
    public Text _txtLevel;
    public Text _txtName;
    public Text _txtGuild;
    public Text _txtFightScore;

    public Sprite[] _sprRank;
    public Color[] _color;

    private PVPRankInfo _info;

    public override void SetInfo(object data)
    {
        _info = (PVPRankInfo) data;
        // 排名
        if (_info.Rank < _sprRank.Length) {
            _imgRank.gameObject.SetActive(true);
            _txtRank.gameObject.SetActive(false);
            _imgRank.sprite = _sprRank[_info.Rank - 1];
        } else {
            _imgRank.gameObject.SetActive(false);
            _txtRank.gameObject.SetActive(true);
            _txtRank.text = _info.Rank.ToString();
        }

        if (_info.Rank < _color.Length) {
            _txtName.color = _color[_info.Rank - 1];
            _txtRank.color = _color[_info.Rank - 1];
            _txtGuild.color = _color[_info.Rank - 1];
            _txtFightScore.color = _color[_info.Rank - 1];
        }

        _imgIcon.sprite = ResourceManager.Instance.GetPlayerIcon(_info.Icon);
        _txtName.text = _info.Name;
        _txtLevel.text = "Lv" + _info.Level;
        _txtGuild.text = _info.GuildName;
        _txtFightScore.text = _info.FightScore.ToString();
    }

    public override void OnClick()
    {
        
    }
}
