using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 物品单元格
public class ItemWidget : ListItemWidget {
    public Image _itemBg;
    public Image _itemBgCover;
    public Image _itemIcon;
    public Text _itemCount;
    public Text _txtName;
    public Image _imgBg;

    public bool _showEffect = false;
    public bool _showNameCount = false;
    
    public ItemInfo _info;

    public override void SetInfo(object data)
    {
        _info = (ItemInfo)data;

        if (_info == null) {
            if (_itemCount != null) _itemCount.gameObject.SetActive(false);
            _itemIcon.gameObject.SetActive(false);
            _itemBg.gameObject.SetActive(false);
            if (_txtName != null) _txtName.gameObject.SetActive(false);
            return;
        } 


        _itemIcon.gameObject.SetActive(true);
        _itemBg.gameObject.SetActive(true);

        if (_itemCount != null) {
            _itemCount.gameObject.SetActive(_info.Number > 1);
            _itemCount.text = _info.Number.ToString();
        }

        _itemIcon.sprite = ResourceManager.Instance.GetItemIcon(_info.ConfigID);
        int quality = 0;
        if (_info.IsEquip()) {
            quality = _info.Quality;
        } else {
            quality = _info.Cfg.Quality;
        }

        _itemBg.sprite = ResourceManager.Instance.GetIconBgByQuality(quality);
        if(_itemBgCover != null)
        _itemBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(quality);

        if (_txtName != null) {
            _txtName.gameObject.SetActive(true);
            if (_info.Cfg != null) {
                if (_showNameCount && _info.Number > 1) {
                    _txtName.text = _info.Cfg.Name + " x" + _info.Number;
                } else {
                    _txtName.text = _info.Cfg.Name;
                }
            }
        }

       

        if (_showEffect && quality > 1) {
            string prefabName = "";
            switch (quality) {
                case 1:
                    break;
                case 2:
                    // 绿色
                    prefabName = "Effect/UI/Eff_wupindiguanglvse";
                    break;
                case 3:
                    // 蓝色
                    prefabName = "Effect/UI/Eff_wupindiguanglanse";
                    break;
                case 4:
                    // 紫色
                    prefabName = "Effect/UI/Eff_wupindiguangzise";
                    break;
                case 5:
                    // 橙色
                    prefabName = "Effect/UI/Eff_wupindiguangchengse";
                    break;
            }
            if (!string.IsNullOrEmpty(prefabName)) {
                GameObject effect = Instantiate(Resources.Load<GameObject>(prefabName));
                effect.transform.SetParent(transform, false);
                effect.transform.localPosition = Vector3.zero;
            }
        }

    }
    
    public void Select()
    {
        if (_imgBg != null) {
            _imgBg.gameObject.SetActive(true);
        }
    }

    public void UnSelect()
    {
        if (_imgBg != null) {
            _imgBg.gameObject.SetActive(false);
        }
    }
}
