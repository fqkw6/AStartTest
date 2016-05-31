using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 装备属性显示
public class ItemAttributeWidget : MonoBehaviour
{
    // 兵法的数据
    public Text _txtAttributesText;
    public Image[] _imgAttributes;
    public Text[] _txtAttributes;

    // 基础属性
    public Text _txtBaseAttrText;
    public Image _imgBaseAttr;
    public Text _txtBaseAttr;

    // 附加属性
    public Text _txtAddAttrText;
    public Image[] _imgAddAttr;
    public Text[] _txtAddAttr;

    private ItemInfo _info;
    
    void Start () {
	
	}
	
    public void SetInfo(ItemInfo info)
    {
        _info = info;
        if (_info == null || !_info.IsEquip()) {
            return;
        }

        Color color = ResourceManager.Instance.GetColorByQuality(_info.Quality);

        _txtBaseAttrText.gameObject.SetActive(false);
        _txtAddAttrText.gameObject.SetActive(false);
        _txtAttributesText.gameObject.SetActive(false);

        if (_info.IsBook()) {
            _txtAttributesText.gameObject.SetActive(true);
            // 如果是兵法书
            // 显示tip
            for (int i = 0; i < _txtAttributes.Length; ++i) {
                string txt = _info.GetAttrDesc(i, 0, true);
                if (!string.IsNullOrEmpty(txt)) {
                    _txtAttributes[i].text = txt;
                    _txtAttributes[i].color = color;
                    _txtAttributes[i].gameObject.SetActive(true);
                    _imgAttributes[i].sprite = ResourceManager.Instance.GetItemAttrFlag(_info.Quality);
                } else {
                    _txtAttributes[i].gameObject.SetActive(false);
                }
            }
        } else {
            _txtBaseAttrText.gameObject.SetActive(true);
            // 如果是装备
            EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(_info.ConfigID);
            _txtBaseAttr.text = _info.GetAttrDesc(0, cfg.BasicType, true);
            _imgBaseAttr.sprite = ResourceManager.Instance.GetItemAttrFlag(1);

            bool showAdd = false;
            for (int i = 0; i < _txtAddAttr.Length; ++i) {
                string txt = _info.GetAttrDesc(i, cfg.BasicType, false);
                if (!string.IsNullOrEmpty(txt)) {
                    _txtAddAttr[i].text = txt;
                    _txtAddAttr[i].color = color;
                    _txtAddAttr[i].gameObject.SetActive(true);
                    _imgAddAttr[i].sprite = ResourceManager.Instance.GetItemAttrFlag(_info.Quality);
                    showAdd = true;
                } else {
                    _txtAddAttr[i].gameObject.SetActive(false);
                }
            }
            _txtAddAttrText.gameObject.SetActive(showAdd);
        }
    }
}
