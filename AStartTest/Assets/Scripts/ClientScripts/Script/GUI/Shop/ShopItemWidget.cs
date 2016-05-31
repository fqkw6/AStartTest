using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO.IsolatedStorage;

// 物品单元格
public class ShopItemWidget : ListItemWidget
{
    public Image _itemBg;
    public Image _itemBgCover;
    public Image _itemIcon;
    public Image _itemMoneyIcon;
    public Image _itemSoldout;
    public Text _itemMoneyNumber;
    public Text _itemName;
    public Material GUIGrayScale;

    private ShopItemInfo _info;

    public override void SetInfo(object data)
    {
        _info = (ShopItemInfo)data;

        ItemsConfig cfg = ItemsConfigLoader.GetConfig(_info.ItemConfigID);
        if (cfg == null) {
            return;
        }

        _itemIcon.sprite = ResourceManager.Instance.GetItemIcon(_info.ItemConfigID);
        _itemBg.sprite = ResourceManager.Instance.GetIconBgByQuality(cfg.Quality);
        _itemBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(cfg.Quality);
        _itemName.text = cfg.Name + "x" + _info.ContainNum;
        _itemName.color = _info.IsSoldOut ? new Color(128/255f, 128/255f, 128/255f, 1) : ResourceManager.Instance.GetColorByQuality(cfg.Quality);
        _itemMoneyIcon.sprite = ResourceManager.Instance.GetPriceIcon(_info.PriceType);
        _itemMoneyIcon.SetNativeSize();
        _itemSoldout.material = _info.IsSoldOut ? GUIGrayScale : null;
        _itemMoneyNumber.text = _info.Price.ToString();
    }

    public override void OnClick()
    {
        if (_info.IsSoldOut) {
            return;
        }

        ShopManager.Instance.RequestBuyItem(_info.PriceType, _info.EntityID, _info.IsFixGet);
    }
}
