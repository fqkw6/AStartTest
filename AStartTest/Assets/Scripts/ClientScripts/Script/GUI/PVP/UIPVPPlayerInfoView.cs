using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 对手玩家详细信息界面
public class UIPVPPlayerInfoView : UIWindow
{
    public const string Name = "PVP/UIPVPPlayerInfoView";
    public Image _imgBg;
    public Image _imgIcon;
    public Text _txtName;
    public Text _txtLevel;
    public Text _txtRank;
    public Text _txtFightScore;
    public Text _txtGuild;

    public SimpleHeroWidget[] _HeroWidgets;
    public Image[] _imgSoldiers;

    private PVPPlayerInfo _info;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        _info = (PVPPlayerInfo)param[0];
        if (_info != null) {
            UserManager.Instance.RequestPlayerInfo(_info.EntityID);
        }
    }

    public override void OnRefreshWindow()
    {
        _imgIcon.sprite = ResourceManager.Instance.GetPlayerIcon(_info.Icon);
        _txtName.text = _info.Name;
        _txtLevel.text = _info.Level.ToString();

        if (_info.Rank > 0) {
            _txtRank.text = _info.Rank.ToString();
        } else {
            _txtRank.text = Str.Get("UI_PVP_NOT_IN_RANK");
        }

        _txtFightScore.text = _info.FightScore.ToString();
        _txtGuild.text = string.IsNullOrEmpty(_info.GuildName) ? Str.Get("UI_NONE") : _info.GuildName;

        for (int i = 0; i < _HeroWidgets.Length; ++i) {
            if (i < _info.HeroList.Count) {
                _HeroWidgets[i].gameObject.SetActive(true);
                _HeroWidgets[i].SetInfo(_info.HeroList[i]);
            } else {
                _HeroWidgets[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _imgSoldiers.Length; ++i) {
            if (i < _info.SoldierList.Count) {
                _imgSoldiers[i].gameObject.SetActive(true);
                _imgSoldiers[i].sprite = ResourceManager.Instance.GetSoldierIcon(_info.SoldierList[i].ConfigID);
            } else {
                _imgSoldiers[i].gameObject.SetActive(false);
            }
        }
    }
}
