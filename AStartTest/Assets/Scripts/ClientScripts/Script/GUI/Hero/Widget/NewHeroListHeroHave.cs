using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewHeroListHeroHave : ListItemWidget
{
    public UIStarPanel _starPanel;
    public Image _imageHeroIconBg;
    public Image _imageHeroIcon;
    public Image _imageHeroAttribute;
    public Image _heroNamePic;
    public Text _txtHeroLevel;
    public HeroListItem _item1;
    public HeroListItem _item2;
    public HeroListItem _item3;
    public HeroListItem _item4;

    private HeroInfo _currentInfo;

    public override void SetInfo(object data)
    {
        SetHeroInfo(data as HeroInfo);
    }

    public void SetHeroInfo(HeroInfo info)
    {
        _currentInfo = info;
        _starPanel.SetStar(info.StarLevel);
        _imageHeroIconBg.sprite = ResourceManager.Instance.GetHeroBgByStar(info.StarLevel);
        _imageHeroIcon.sprite = ResourceManager.Instance.GetHeroImage(info.ConfigID);
        _heroNamePic.sprite = ResourceManager.Instance.GetHeroNameImage(info.ConfigID);
        _txtHeroLevel.text = "Lv" + info.Level;

        _item1.SetInfo(info, (ItemType)info.Cfg.WeaponType);
        _item2.SetInfo(info, (ItemType)info.Cfg.ArmourType);
        _item3.SetInfo(info, ItemType.DECORATION);
        _item4.SetInfo(info, ItemType.BOOK);
    }
}
