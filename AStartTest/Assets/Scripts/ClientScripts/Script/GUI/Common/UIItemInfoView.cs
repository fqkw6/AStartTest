using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//物品信息显示界面
public class UIItemInfoView : UIWindow
{
    public const string Name = "Hero/UIItemInfoView";
    public Image _imageResIcon;
    public Image _imageResIconBg;
    public Image _imageResIconBgCover;
    public Text _txtResName;
    public Text _txtResNumber;
    public Text _txtResInfo;

    private ItemInfo _itemInfo;
    private int _itemCfgID;

    public override void OnBindData(params object[] param)
    {
        _itemInfo = param[0] as ItemInfo;

        if (_itemInfo != null) {
            _itemCfgID = _itemInfo.ConfigID;
        } else {
            _itemCfgID = (int)param[1];
        }
    }

    public override void OnRefreshWindow()
    {
        if (_itemInfo != null) {
            // 有这个物品
            _txtResNumber.text = _itemInfo.Number.ToString();
            _imageResIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(_itemInfo.Cfg.Quality);
            _imageResIconBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(_itemInfo.Cfg.Quality);
            _imageResIcon.sprite = ResourceManager.Instance.GetItemIcon(_itemInfo.Cfg.CfgId);
            _txtResName.text = _itemInfo.Cfg.Name;
            _txtResName.color = ResourceManager.Instance.GetColorByQuality(_itemInfo.Quality);
        } else {
            ItemInfo itemInfo = UserManager.Instance.GetItemByConfigID(_itemCfgID);
            if (itemInfo != null) {
                _txtResNumber.color = Color.white;
                _txtResNumber.text = itemInfo.Number.ToString();
            } else {
                int number = 0;
                if (_itemCfgID == GameConfig.ITEM_CONFIG_ID_WOOD) {
                    number = UserManager.Instance.Wood;
                } else if (_itemCfgID == GameConfig.ITEM_CONFIG_ID_STONE) {
                    number = UserManager.Instance.Stone;
                } else if (_itemCfgID == GameConfig.ITEM_CONFIG_ID_MONEY) {
                    number = UserManager.Instance.Money;
                }

                if (number == 0) {
                    _txtResNumber.color = Color.red;
                    _txtResNumber.text = "0";
                } else {
                    _txtResNumber.color = Color.white;
                    _txtResNumber.text = number.ToString();
                }
            }
            ItemsConfig cfg = ItemsConfigLoader.GetConfig(_itemCfgID);

            _imageResIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(cfg.Quality);
            _imageResIconBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(cfg.Quality);
            _imageResIcon.sprite = ResourceManager.Instance.GetItemIcon(cfg.CfgId);
            _txtResName.text = cfg.Name;
            _txtResName.color = ResourceManager.Instance.GetColorByQuality(cfg.Quality);
        }   
    }
}
