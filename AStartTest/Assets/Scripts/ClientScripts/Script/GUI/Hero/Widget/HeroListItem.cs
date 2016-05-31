using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroListItem : MonoBehaviour
{
    public Image _itemBg;
    public Image _itemIcon;
    public Image _itemFlagY;
    public Image _itemFlagG;
    public Image _itemIconBg;

	void Awake()
    {

    }

    public void SetInfo(HeroInfo info, ItemType itemType)
    {
        _itemBg.sprite = ResourceManager.Instance.GetItemBgByStar(info.StarLevel);
        _itemIcon.gameObject.SetActive(false);
        _itemIconBg.gameObject.SetActive(false);
        _itemFlagY.gameObject.SetActive(false);
        _itemFlagG.gameObject.SetActive(false);
        ItemInfo itemInfo = info.GetItemByType(itemType);
        if (itemInfo != null) {
            // 已经装备
            _itemIcon.gameObject.SetActive(true);
            _itemIcon.sprite = ResourceManager.Instance.GetItemIcon(itemInfo.ConfigID);
            _itemIconBg.gameObject.SetActive(true);
            _itemIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(itemInfo.Quality);
        } else {
            _itemIcon.gameObject.SetActive(false);

            ItemInfo itemInfoInBag = UserManager.Instance.GetItemByType(itemType);
            if (itemInfoInBag != null) {
                // 背包中有对应物品
                ItemsConfig itemConfig = ItemsConfigLoader.GetConfig(itemInfoInBag.ConfigID);
                if (info.Level >= itemConfig.Level) {
                    // 可以装备，显示绿色+
                    _itemFlagG.gameObject.SetActive(true);
                } else {
                    // 不可以装备，显示黄色+
                    _itemFlagY.gameObject.SetActive(true);
                }
            } else {
                // 背包中没有对应物品，直接留空
            }
        }
    }
}
