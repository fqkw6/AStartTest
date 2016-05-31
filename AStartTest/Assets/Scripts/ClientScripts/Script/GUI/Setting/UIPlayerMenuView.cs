using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 玩家菜单界面
public class UIPlayerMenuView : UIWindow
{
    public const string Name = "Setting/UIPlayerMenuView";

    public Image _imgIcon;
    public Text _txtName;
    public Text _txtFightScore;
    public Text _txtVip;
    public Text _txtLevel;
    public Text _txtExp;
    public Image _imgExp;
    public Text _txtUserID;
    public Text _txtGuild;
    public Text _txtHeroLevelLimit;

    public override void OnRefreshWindow()
    {
        _imgIcon.sprite = ResourceManager.Instance.GetPlayerIcon(0);
        _txtName.text = UserManager.Instance.RoleName;
        _txtFightScore.text = UserManager.Instance.GetFightScore().ToString();
        _txtVip.text = "VIP" + UserManager.Instance.VipLevel;
        _txtLevel.text = "LV." + UserManager.Instance.Level;

        PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(UserManager.Instance.Level);
        if (cfg != null) {
            _txtExp.text = string.Format("{0}/{1}", UserManager.Instance.Exp, cfg.Exp);
            _txtExp.gameObject.SetActive(true);
            _imgExp.fillAmount = 1.0f*UserManager.Instance.Exp/cfg.Exp;
            _txtHeroLevelLimit.text = cfg.HeroLevelLimit.ToString();
        } else {
            _txtExp.gameObject.SetActive(false);
            _imgExp.fillAmount = 1;
        }

        _txtUserID.text = UserManager.Instance.EntityID.ToString();
        _txtGuild.text = GuildManager.Instance.GuildID == 0 ? Str.Get("UI_NONE") : GuildManager.Instance.GuildName;
    }

    public void OnClickSetting()
    {
        UIManager.Instance.OpenWindow<UIPlayerSetSystemView>();   
    }

    public void OnClickChangeAccount()
    {
        UserManager.Instance.RequestLogout();
    }

    public void OnClickChangeName()
    {
        UIManager.Instance.OpenWindow<UIPlayerSetNameView>();
    }

    public void OnClickChangeIcon()
    {
        UIManager.Instance.OpenWindow<UIPlayerSetHeadIconView>();
    }
}
