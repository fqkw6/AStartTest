using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelItemTip : MonoBehaviour
{
    public Button _btnUse;
    public Text _txtUseText;
    public Button _btnDetail;
    public Button _btnSell;
    public Image _ImageItemIconBg;
    public Image _ImageItemIconBgCover;
    public Image _imageItemIcon;
    public Text _txtItemName;
    public Text _txtItemType;
    public Text _txtQualityText;
    public Text _txtQuality;
    public Text _txtScoreText;
    public Text _txtScore;
    public Text _txtItemSellPrice;
    public Text[] _txtAttributes;
    public Text _txtAttrText;
    public Text _txtBaseAttrText;
    public Text _txtBaseAttr;
    public Text _txtAddAttrText;
    public Text[] _txtAddAttr;
    public Text _txtItemDesc;
    
    private ItemInfo _currentItemInfo;
    
    void Start ()
    {
        EventDispatcher.AddEventListener<ItemInfo>(EventID.EVENT_UI_UPDATE_ITEMTIP_INFO, UpdateItemInfo); 
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveEventListener<ItemInfo>(EventID.EVENT_UI_UPDATE_ITEMTIP_INFO, UpdateItemInfo);
    }

    public void SetItemInfo(ItemInfo info)
    {
        _currentItemInfo = info;

        // 在背包中显示
        if (info.CouldUse() || info.IsEquip()) {
            // 可以使用
            _btnUse.gameObject.SetActive(true);
            _btnDetail.gameObject.SetActive(false);

            if (info.IsEquip()) {
                _txtUseText.text = Str.Get("UI_EQUIP");
            } else {
                _txtUseText.text = Str.Get("UI_USE");
            }
        } else {
            _btnUse.gameObject.SetActive(false);
            _btnDetail.gameObject.SetActive(true);
        }

        _btnSell.gameObject.SetActive(true);
        _ImageItemIconBg.sprite = ResourceManager.Instance.GetIconBgByQuality(info.Cfg.Quality);
        _ImageItemIconBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(info.Cfg.Quality);
        _imageItemIcon.sprite = ResourceManager.Instance.GetItemIcon(info.ConfigID);
        _txtItemName.text = info.Cfg.Name;
        _txtItemName.color = ResourceManager.Instance.GetColorByQuality(info.Cfg.Quality);
        _txtItemDesc.text = info.Cfg.Decription;
        _txtItemSellPrice.text = info.Cfg.Price.ToString();

        

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


            _txtBaseAttrText.gameObject.SetActive(false);
            _txtAddAttrText.gameObject.SetActive(false);
            _txtAttrText.gameObject.SetActive(false);
            _txtItemDesc.gameObject.SetActive(false);

            if (info.IsBook()) {
                _txtAttrText.gameObject.SetActive(true);
                // 如果是兵法书
                // 显示tip
                for (int i = 0; i < _txtAttributes.Length; ++i) {
                    string txt = info.GetAttrDesc(i, 0, true);
                    if (!string.IsNullOrEmpty(txt)) {
                        _txtAttributes[i].text = txt;
                        _txtAttributes[i].color = color;
                        _txtAttributes[i].gameObject.SetActive(true);
                    } else {
                        _txtAttributes[i].gameObject.SetActive(false);
                    }
                }
            } else {
                _txtBaseAttrText.gameObject.SetActive(true);
                // 如果是装备
                EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(info.ConfigID);
                _txtBaseAttr.text = info.GetAttrDesc(0, cfg.BasicType, true);

                bool showAdd = false;
                for (int i = 0; i < _txtAddAttr.Length; ++i) {
                    string txt = info.GetAttrDesc(i, cfg.BasicType, false);
                    if (!string.IsNullOrEmpty(txt)) {
                        _txtAddAttr[i].text = txt;
                        _txtAddAttr[i].color = color;
                        _txtAddAttr[i].gameObject.SetActive(true);
                        showAdd = true;
                    } else {
                        _txtAddAttr[i].gameObject.SetActive(false);
                    }
                }
                _txtAddAttrText.gameObject.SetActive(showAdd);
            }
        } else {
            _txtItemDesc.gameObject.SetActive(true);
            _txtItemDesc.text = info.Cfg.Decription;
            _txtQualityText.gameObject.SetActive(false);
            _txtScoreText.gameObject.SetActive(false);
            _txtBaseAttrText.gameObject.SetActive(false);
            _txtAddAttrText.gameObject.SetActive(false);
            _txtAttrText.gameObject.SetActive(false);
        }

    }

    private void UpdateItemInfo(ItemInfo info)
    {
        SetItemInfo(info);
    }

    public void OnClickUse()
    {
        if (_currentItemInfo.IsEquip()) {
            UIManager.Instance.OpenWindow<UIHeroEquipView>(_currentItemInfo);
        } else if (_currentItemInfo.IsExpBall()) {
            UIManager.Instance.OpenWindow<UISelectHeroView>(_currentItemInfo);
        } else if (_currentItemInfo.Cfg.Enable > 0) {
            UserManager.Instance.ReqUseItem(_currentItemInfo.EntityID);
        }
    }

    public void OnClickDetail()
    {
        UIManager.Instance.OpenWindow<UIItemGetWayView>(_currentItemInfo);
    }

    public void OnClickSell()
    {
        UIManager.Instance.OpenWindow<UIItemSellView>(_currentItemInfo);
    }

}
