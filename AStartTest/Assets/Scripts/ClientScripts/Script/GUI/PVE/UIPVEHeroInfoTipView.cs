using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 英雄信息的tip界面
public class UIPVEHeroInfoTipView : UIWindow
{
    public const string Name = "PVE/UIPVEHeroInfoTipView";
    public Image _imgHeroBg;
    public Image _imgHeroIcon;
    public Text _txtHeroName;
    public Text _txtHeroLevel;
    public Text _txtHeroDesc;
    public UIStarPanel _starPanel;

    public override void OnBindData(params object[] param)
    {
        int heroID = (int)param[0];
        int level = (int)param[1];
        int star = (int) param[2];
        int quality = (int) param[3];

        HeroConfig cfg = HeroConfigLoader.GetConfig(heroID);
        if (cfg == null) return;

        _imgHeroBg.sprite = ResourceManager.Instance.GetIconBgByQuality(quality);
        _imgHeroIcon.sprite = ResourceManager.Instance.GetHeroIcon(heroID);
        _starPanel.SetStar(star);
        _txtHeroName.text = cfg.HeroName;
        _txtHeroDesc.text = cfg.HeroName;
        _txtHeroLevel.text = "Lv " + level;
    }
}
