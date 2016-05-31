using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIItemSellView : UIWindow
{
    public const string Name = "Hero/UIItemSellView";
    public Image _itemIconBg;
    public Image _itemIcon;
    public Text _itemName;
    public Text _itemNum;
    public Text _itemSellPrice;
    public Text _txtGetMoney;
    public Text _txtSellCount;

    private ItemInfo _currentItemInfo;
    private int _sellCount = 0;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        SetItemInfo(param[0] as ItemInfo);
    }

    public void SetItemInfo(ItemInfo info)
    {
        _sellCount = info.Number;

        _currentItemInfo = info;
        _itemIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(info.Cfg.Quality);
        _itemIcon.sprite = ResourceManager.Instance.GetItemIcon(info.ConfigID);
        _itemName.text = info.Cfg.Name;
        _itemNum.text = string.Format(Str.Get("UI_ITEM_TIP_COUNT"), info.Number);
        _itemSellPrice.text = info.Cfg.Price.ToString();
        _txtGetMoney.text = (info.Cfg.Price*_sellCount).ToString();
        _txtSellCount.text = _sellCount.ToString();

//        switch ((ItemType)info.Cfg.Type) {
//            case ItemType.SOUL_PIECE:
//                _title.sprite = _titleStone;
//                break;
//            case ItemType.ARMOR:
//            case ItemType.BOOT:
//            case ItemType.SPECIAL:
//            case ItemType.WEAPON:
//                _title.sprite = _titleEquip;
//                break;
//            default:
//                _title.sprite = _titleItem;
//                break;
//        }
    }

    public void OnClickDec()
    {
        _sellCount = Mathf.Max(_sellCount - 1, 0);
        UpdateSellCount();
    }

    public void OnClickAdd()
    {
        _sellCount = Mathf.Min(_sellCount + 1, _currentItemInfo.Number);
        UpdateSellCount();
    }

    public void OnClickMax()
    {
        _sellCount = _currentItemInfo.Number;
        UpdateSellCount();
    }

    public void OnClickOK()
    {
        UserManager.Instance.ReqSellItem(_currentItemInfo.EntityID, _sellCount);
        CloseWindow();
    }

    private void UpdateSellCount()
    {
        _txtSellCount.text = _sellCount.ToString();
        _txtGetMoney.text = (_currentItemInfo.Cfg.Price * _sellCount).ToString();
    }
}
