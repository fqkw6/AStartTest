using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelHeroAdvance : MonoBehaviour
{
    public HeroViewItem _item1;
    public HeroViewItem _item2;
    public HeroViewItem _item3;
    public HeroViewItem _item4;
    public RectTransform _panelUnEquipBg;
    public RectTransform _panelUnEquip;
    
    public Sprite _greeAdd;
    public Sprite _yellowAdd;

    private HeroInfo _currentInfo;

    private ItemType _currentItemType;

    // Use this for initialization
    void Start () {
        _panelUnEquipBg.gameObject.SetActive(false);
    }

    public void SetHeroInfo(HeroInfo info, bool fullRefresh = true)
    {
        if (info == null) return;

        _currentInfo = info;

        _item1.SetSprite(_greeAdd, _yellowAdd);
        _item2.SetSprite(_greeAdd, _yellowAdd);
        _item3.SetSprite(_greeAdd, _yellowAdd);
        _item4.SetSprite(_greeAdd, _yellowAdd);

        _item1.OnClickItem = OnClickItem;
        _item2.OnClickItem = OnClickItem;
        _item3.OnClickItem = OnClickItem;
        _item4.OnClickItem = OnClickItem;

        _item1.SetInfo(info, (ItemType)info.Cfg.WeaponType);
        _item2.SetInfo(info, (ItemType)info.Cfg.ArmourType);
        _item3.SetInfo(info, ItemType.DECORATION);
        _item4.SetInfo(info, ItemType.BOOK);
    }

    public void OnClickItem(ItemType itemType)
    {
        _currentItemType = itemType;
        ItemInfo itemInfo = _currentInfo.GetItemByType(itemType);
        if (itemInfo == null) {
            // 没有穿戴物品，直接显示选物品界面
            UIManager.Instance.OpenWindow<UISelectEquipView>(_currentInfo, itemType);
        } else {
            _panelUnEquipBg.gameObject.SetActive(true);
            if (itemType == ItemType.DECORATION) {
                _panelUnEquip.transform.position = _item3.transform.position;
            } else if (itemType == ItemType.BOOK) {
                _panelUnEquip.transform.position = _item4.transform.position;
            } else if (itemType == ItemType.CHEST || itemType == ItemType.CLOTH) {
                _panelUnEquip.transform.position = _item2.transform.position;
            } else {
                _panelUnEquip.transform.position = _item1.transform.position;
            }
        }
    }

    public void OnClickEquip()
    {
        UIManager.Instance.OpenWindow<UISelectEquipView>(_currentInfo, _currentItemType);
        _panelUnEquipBg.gameObject.SetActive(false);
    }

    public void OnClickUnEquip()
    {
        ItemInfo info = _currentInfo.GetItemByType(_currentItemType);
        if (info == null) return;

        UserManager.Instance.RequestTakeOffEquip(_currentInfo.EntityID, info.EntityID);
        _panelUnEquipBg.gameObject.SetActive(false);
    }

    public void OnClickEquipBg()
    {
        _panelUnEquipBg.gameObject.SetActive(false);
    }
}
