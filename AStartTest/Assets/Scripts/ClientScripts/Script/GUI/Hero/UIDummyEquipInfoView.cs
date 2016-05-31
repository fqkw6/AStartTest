using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 没有获得的装备信息
public class UIDummyEquipInfoView : UIWindow
{
    public const string Name = "Hero/UIDummyEquipInfoView";
    public Image _ImageItemIconBg;
    public Image _imageItemIcon;
    public Text _txtItemName;
    public Text _txtItemLevel;
    public Text _txtItemType;

    private int _itemCfgID;

    public override void OnBindData(params object[] param)
    {
        _itemCfgID = (int) param[0];
    }

    public override void OnRefreshWindow()
    {
        ItemsConfig cfg = ItemsConfigLoader.GetConfig(_itemCfgID);
        _ImageItemIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(cfg.Quality);
        _imageItemIcon.sprite = ResourceManager.Instance.GetItemIcon(_itemCfgID);
        _txtItemName.text = cfg.Name;
        _txtItemName.color = ResourceManager.Instance.GetColorByQuality(cfg.Quality);
        _txtItemType.text = ItemInfo.GetItemTypeName(cfg.Type);

        if (cfg.Type == (int)ItemType.BOOK) {
            // 如果是兵法书
            BingfaConfig cfgBook = BingfaConfigLoader.GetConfig(_itemCfgID);
            _txtItemLevel.text = cfgBook.EquipLevel.ToString();
        } else {
            // 如果是装备
            EquipmentConfig cfgEquip = EquipmentConfigLoader.GetConfig(_itemCfgID);
            _txtItemLevel.text = cfgEquip.EquipLevel.ToString();
        }
    }
}