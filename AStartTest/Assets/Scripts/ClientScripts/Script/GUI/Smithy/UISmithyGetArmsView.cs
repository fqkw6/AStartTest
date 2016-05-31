using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 锻造成功界面
public class UISmithyGetArmsView : UIWindow {

    public const string Name = "Smithy/UISmithyGetArmsView";

    public Text _txtName;
    public Text _txtLevel;
    public Text _txtType;
    public Text _txtQuality;
    public Text _txtScore;
    public Text _txtQualityText;
    public Text _txtScoreText;
    public Image _imgEquip;

    public ItemAttributeWidget _itemAttribute;

    public GameObject[] _effect;

    private ItemInfo _itemInfo;
    public override void OnBindData(params object[] param)
    {
        _itemInfo = param[0] as ItemInfo;
    }

    public override void OnRefreshWindow()
    {
        if (_itemInfo == null) return;

        Color color = ResourceManager.Instance.GetColorByQuality(_itemInfo.Quality);
        _txtName.text = _itemInfo.Cfg.Name;
        _txtName.color = color;

        _txtScoreText.color = color;
        _txtQualityText.color = color;


        _txtLevel.text = _itemInfo.Cfg.Level.ToString();
        //_txtLevel.color = color;

        _txtType.text = ItemInfo.GetItemTypeName(_itemInfo.Cfg.Type);
        //_txtType.color = color;

        _txtScore.text = _itemInfo.GetScore().ToString();
        _txtScore.color = color;

        _txtQuality.text = ItemInfo.GetQualityName(_itemInfo.Quality);
        _txtQuality.color = color;

        _itemAttribute.SetInfo(_itemInfo);

        foreach (var item in _effect) {
            item.SetActive(false);
        }

        if (_itemInfo.Quality > 1) {
            _effect[_itemInfo.Quality - 2].SetActive(true);
        }
        if (_itemInfo.IsEquip()) {
            _txtQuality.text = ItemInfo.GetQualityName(_itemInfo.Quality);
            _txtQuality.color = color;
            _txtQualityText.color = color;
            _txtQualityText.gameObject.SetActive(true);

            _txtScore.text = _itemInfo.GetScore().ToString();
            _txtScore.color = color;
            _txtScoreText.color = color;
            _txtScoreText.gameObject.SetActive(true);

            if (_itemInfo.IsBook()) {
                _imgEquip.sprite = ResourceManager.Instance.GetBookImage(_itemInfo.ConfigID);
            } else {
                _imgEquip.sprite = ResourceManager.Instance.GetEquipImage(_itemInfo.ConfigID);
            }
        }
    }
}
