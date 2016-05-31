using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 装备信息Tip界面
public class UIEquipInfoView : UIWindow
{
    public const string Name = "Hero/UIEquipInfoView";

    public Text _txtHasEquip;
    public Image _ImageItemIconBg;
    public Image _imageItemIcon;
    public Text _txtItemName;
    public Text _txtItemLevel;
    public Text _txtItemType;
    public Text _txtQualityText;
    public Text _txtQuality;
    public Text _txtScoreText;
    public Text _txtScore;

    public ItemAttributeWidget _itemAttribute;

    private ItemInfo _info;

    public override void OnBindData(params object[] param)
    {
        _info = param[0] as ItemInfo;
    }

    public override void OnRefreshWindow()
    {
        if (_info == null) {
            return;
        }

        HeroInfo heroInfo = UserManager.Instance.GetHeroByItem(_info.EntityID);
        if (heroInfo != null) {
            _txtHasEquip.gameObject.SetActive(true);
            _txtHasEquip.text = string.Format("{0}({1})", Str.Get("UI_HAS_EQUIP"), heroInfo.Cfg.HeroName);
        } else {
            _txtHasEquip.gameObject.SetActive(false);
        }

        _ImageItemIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(_info.Cfg.Quality);
        _imageItemIcon.sprite = ResourceManager.Instance.GetItemIcon(_info.ConfigID);
        _txtItemName.text = _info.Cfg.Name;
        _txtItemName.color = ResourceManager.Instance.GetColorByQuality(_info.Cfg.Quality);
        _txtItemType.text = ItemInfo.GetItemTypeName(_info.Cfg.Type);

        _itemAttribute.SetInfo(_info);

        if (_info.IsEquip()) {
            Color color = ResourceManager.Instance.GetColorByQuality(_info.Quality);
            _txtQuality.text = ItemInfo.GetQualityName(_info.Quality);
            _txtQuality.color = color;
            _txtQualityText.color = color;
            _txtQualityText.gameObject.SetActive(true);

            _txtScore.text = _info.GetScore().ToString();
            _txtScore.color = color;
            _txtScoreText.color = color;
            _txtScoreText.gameObject.SetActive(true);

            if (_info.IsBook()) {
                // 如果是兵法书
                BingfaConfig cfg = BingfaConfigLoader.GetConfig(_info.ConfigID);
                _txtItemLevel.text = cfg.EquipLevel.ToString();
            } else {
                // 如果是装备
                EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(_info.ConfigID);
                _txtItemLevel.text = cfg.EquipLevel.ToString();
            }
        }
    }
}
