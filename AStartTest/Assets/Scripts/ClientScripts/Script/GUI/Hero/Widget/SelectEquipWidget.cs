using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 选择装备的widget
public class SelectEquipWidget : ListItemWidget
{
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

    public override void SetInfo(object data)
    {
        ItemInfo info = data as ItemInfo;
        if (info == null) return;

        _info = info;

        _ImageItemIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(info.Cfg.Quality);
        _imageItemIcon.sprite = ResourceManager.Instance.GetItemIcon(info.ConfigID);
        _txtItemName.text = info.Cfg.Name;
        _txtItemName.color = ResourceManager.Instance.GetColorByQuality(info.Cfg.Quality);
        _txtItemType.text = ItemInfo.GetItemTypeName(info.Cfg.Type);

        _itemAttribute.SetInfo(_info);

        if (info.IsEquip()) {
            Color color = ResourceManager.Instance.GetColorByQuality(info.Quality);
            _txtQuality.text = ItemInfo.GetQualityName(info.Quality);
            _txtQuality.color = color;
            _txtQualityText.color = color;
            _txtQualityText.gameObject.SetActive(true);

            _txtScore.text = info.GetScore().ToString();
            _txtScore.color = color;
            _txtScoreText.color = color;
            _txtScoreText.gameObject.SetActive(true);

            if (info.IsBook()) {
                // 如果是兵法书
                BingfaConfig cfg = BingfaConfigLoader.GetConfig(info.ConfigID);
                _txtItemLevel.text = cfg.EquipLevel.ToString();
            } else {
                // 如果是装备
                EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(info.ConfigID);
               _txtItemLevel.text = cfg.EquipLevel.ToString();
            }
        }
    }
}
