using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroViewItem : MonoBehaviour {
    public Image _itemIcon;
    public Image _itemFlag;
    public Image _itemType;
    public Image _itemBg;
    public Image _itemBgCover;
    public Text _itemText;

    public Color _textCouldGetColor;
    public Color _textNotColor;
    public Color _textHaveColor;

    private Sprite _greenAdd;
    private Sprite _yellowAdd;

    private HeroInfo _currentInfo = null;
    private ItemType _currentType;

    public System.Action<ItemType> OnClickItem;
    
    void Awake()
    {

    }

    public void SetSprite(Sprite g, Sprite y)
    {
        _greenAdd = g;
        _yellowAdd = y;
    }

    public void SetInfo(HeroInfo info, ItemType itemType)
    {
        // TODO 显示合成逻辑，当判定有合成材料，并且没有对应装备时显示可合成
        // TODO 如果判定副本已通关，并且显示可获取（如果可以装备显示绿色+，否则显示黄色+），可获取颜色为黄色
        // 未装备和无装备显示偏灰黄

        _currentInfo = info;
        _currentType = itemType;

        ItemInfo itemInfo = info.GetItemByType(itemType);

        if (itemInfo != null) {
            // 已经装备
            _itemFlag.gameObject.SetActive(false);
            _itemIcon.gameObject.SetActive(true);
            _itemIcon.sprite = ResourceManager.Instance.GetItemIcon(itemInfo.ConfigID);
            _itemBg.sprite = ResourceManager.Instance.GetIconBgByQuality(itemInfo.Quality);
            _itemBgCover.gameObject.SetActive(true);
            _itemBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(itemInfo.Quality);
            _itemText.gameObject.SetActive(false);
        } else {
            _itemIcon.gameObject.SetActive(false);
            _itemBg.sprite = ResourceManager.Instance.GetIconBgByQuality(1);
            _itemBgCover.gameObject.SetActive(false);
            _itemBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(1);

            ItemInfo itemInfoInBag = UserManager.Instance.GetItemByType(itemType);
            if (itemInfoInBag != null) {
                _itemFlag.gameObject.SetActive(true);
                _itemText.gameObject.SetActive(true);

                // 背包中有对应物品
                if (info.Level >= itemInfoInBag.Cfg.Level) {
                    // 可以装备，显示绿色+
                    _itemFlag.sprite = _greenAdd;
                    _itemText.text = Str.Get("UI_HERO_ITEM_EQUIP");
                    _itemText.color = Color.green;
                } else {
                    // 不可以装备，显示黄色+
                    _itemFlag.sprite = _yellowAdd;
                    _itemText.text = Str.Get("UI_HERO_ITEM_NOT_EQUIP");
                    _itemText.color = new Color32(255,184,0,255);
                }
            } else {
                // 背包中没有对应物品，直接留空
                _itemFlag.gameObject.SetActive(false);
                _itemText.gameObject.SetActive(false);
            }
        }
    }

    public void OnClick()
    {
        if (OnClickItem != null) {
            OnClickItem(_currentType);
        }
    }
}
