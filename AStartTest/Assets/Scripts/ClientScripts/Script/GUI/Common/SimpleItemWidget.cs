using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

// 通用的物品图标组件 (不属于ListView)
public class SimpleItemWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image _imgBg;
    public Image _imgIcon;
    public Text _txtCount;
    public Text _txtName;
    public bool _enableTip = true;

    private ItemInfo _itemInfo;
    private int _itemCfgID;

    public System.Action<ItemInfo, int> OnClickHandler;

    public void SetInfo(ItemInfo info)
    {
        _itemInfo = info;

        SetInfo(_itemInfo.ConfigID, _itemInfo.Number);
    }

    public void SetInfo(int cfgID, int count)
    {
        _itemCfgID = cfgID;
        ItemsConfig cfg = ItemsConfigLoader.GetConfig(cfgID);
        if (cfg == null) {
            gameObject.SetActive(false);
            return;
        }

        _imgBg.sprite = ResourceManager.Instance.GetIconBgByQuality(cfg.Quality);
        _imgIcon.sprite = ResourceManager.Instance.GetItemIcon(cfgID);
        if (_txtCount != null) {
            _txtCount.gameObject.SetActive(count > 1);
            _txtCount.text = count.ToString();
        }

        if (_txtName != null) {
            _txtName.text = cfg.Name;
        }
    }

    public void OnClick()
    {
        if (OnClickHandler != null) {
            OnClickHandler(_itemInfo, _itemCfgID);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_enableTip) return;
        // TODO 显示tip界面

        ItemsConfig cfg = ItemsConfigLoader.GetConfig(_itemCfgID);
        if (cfg.Type <= (int)ItemType.BOOK) {
            // 装备
            if (_itemInfo == null) {
                UIManager.Instance.OpenWindow<UIDummyEquipInfoView>(_itemCfgID);
            } else {
                UIManager.Instance.OpenWindow<UIEquipInfoView>(_itemInfo, _itemCfgID);
            }
        } else {
            // 其他物品
            UIManager.Instance.OpenWindow<UIItemInfoView>(_itemInfo, _itemCfgID);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_enableTip) return;
        // 关闭tip界面
        UIManager.Instance.CloseWindow<UIItemInfoView>();
        UIManager.Instance.CloseWindow<UIEquipInfoView>();
        UIManager.Instance.CloseWindow<UIDummyEquipInfoView>();
    }
}
