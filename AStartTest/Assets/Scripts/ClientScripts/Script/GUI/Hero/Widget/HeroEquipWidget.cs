using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// (装备)选择英雄widget
public class HeroEquipWidget : ListItemWidget
{

    public Text _txtHeroName;
    public UIStarPanel _starPanel;
    public Image _imgIconBg;
    public Image _imgIcon;
    public Text _txtFightScore;
    public Image _imgItemType;
    public SimpleItemWidget _itemWidget;

    private HeroInfo _heroInfo;
    private ItemInfo _itemInfo;

    public void SetInfo(HeroInfo heroInfo, ItemInfo itemInfo)
    {
        _heroInfo = heroInfo;
        _itemInfo = itemInfo;

        _starPanel.SetStar(heroInfo.StarLevel);
        _imgIconBg.sprite = ResourceManager.Instance.GetHeroBgByStar(heroInfo.StarLevel);
        _imgIcon.sprite = ResourceManager.Instance.GetHeroImage(heroInfo.ConfigID);
        _txtHeroName.text = heroInfo.GetName();

        _txtHeroName.color = ResourceManager.Instance.GetColorByQuality(heroInfo.StarLevel);
        _txtFightScore.text = heroInfo.FightingScore.ToString();

        _imgItemType.sprite = ResourceManager.Instance.GetItemTypeIcon((ItemType)itemInfo.Cfg.Type);

        ItemInfo curItemInfo = heroInfo.GetItemByType((ItemType)itemInfo.Cfg.Type);
        if (curItemInfo != null) {
            _itemWidget.SetInfo(curItemInfo);
            _itemWidget.gameObject.SetActive(true);
        } else {
            _itemWidget.gameObject.SetActive(false);
        }
    }
}
