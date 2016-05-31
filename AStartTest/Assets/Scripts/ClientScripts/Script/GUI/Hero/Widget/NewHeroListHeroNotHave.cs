using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewHeroListHeroNotHave : ListItemWidget
{
    public Image _imageHeroIcon;
    public Image _heroNamePic;
    public Text _txtHeroPiece;
    public Image _txtHeroPiecePrg;
    public Image _imageAttribute;
    public UIStarPanel _starPanel;
    
    public override void SetInfo(object data)
    {
        SetHeroInfo((int)data);
    }

    public void SetHeroInfo(int cfgID)
    {
        HeroConfig cfg = HeroConfigLoader.GetConfig(cfgID);
        _starPanel.SetStar(cfg.Star);
        _imageHeroIcon.sprite = ResourceManager.Instance.GetHeroImage(cfgID);
        _heroNamePic.sprite = ResourceManager.Instance.GetHeroNameImage(cfgID);

        int curStone = UserManager.Instance.GetHeroPieceCount(cfg.Cost);
        int needStone = UserManager.Instance.GetHeroStarUpgradeCost(cfgID, cfg.Star);
        _txtHeroPiece.text = string.Format("{0}/{1}", curStone, needStone);
        _txtHeroPiecePrg.fillAmount = 1.0f * curStone / needStone;
    }
    
}
