using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

// 锻造铺物品控件
public class SmithyItemWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum WidgetType
    {
        EQUIP,  // 锻造的最终装备
        MOLD,   // 模具
        MATERIAL,   // 材料
    }
    public Image _imgBg;
    public Image _imgBgCover;
    public Image _imgIcon;
    public Text _txtName;
    public Text _txtCount;

    private ItemInfo _itemInfo;
    private int _itemCfgID;
    private WidgetType _type;
    private int _moldSlotIndex;
    private ItemType _equipType;
    private int _limitLevel;

    public void SetInfo(ItemInfo info, WidgetType type, int count, int needCount)
    {
        _itemInfo = info;
        _type = type;
        if (_itemInfo == null) {
            _imgBg.gameObject.SetActive(false);
            _imgIcon.gameObject.SetActive(false);
            _txtName.gameObject.SetActive(false);
            if (_txtCount != null) _txtCount.gameObject.SetActive(false);
            return;
        }

        SetInfo(_itemInfo.ConfigID, type, count, needCount);
    }

    public void SetInfo(int cfgID, WidgetType type, int count, int needCount)
    {
        _itemCfgID = cfgID;
        _type = type;

        _imgBg.gameObject.SetActive(true);
        _imgIcon.gameObject.SetActive(true);
        _txtName.gameObject.SetActive(true);
        if (_txtCount != null) _txtCount.gameObject.SetActive(true);
        ItemsConfig cfg = ItemsConfigLoader.GetConfig(_itemCfgID);
        if (cfg == null) {
            gameObject.SetActive(false);
            return;
        }

        _imgBg.sprite = ResourceManager.Instance.GetIconBgByQuality(cfg.Quality);
        if(_imgBgCover != null)
        _imgBgCover.sprite = ResourceManager.Instance.GetIconBgCoverByQuality(cfg.Quality);
        _imgIcon.sprite = ResourceManager.Instance.GetItemIcon(_itemCfgID);
        if (_txtCount != null && needCount > 0) {
            if (count < needCount) {
                _txtCount.text = string.Format("<color=red>{0}</color>/{1}", count, needCount);
            } else {
                _txtCount.text = string.Format("{0}/{1}", count, needCount);
            }
        }

        if (_txtName != null) {
            _txtName.text = cfg.Name;
        }
    }

    // 设置这个widget是第几个模具槽 什么类型的装备 最低等级是多少
    public void SetMoldInfo(int slotIndex, ItemType equipType, int level)
    {
        _moldSlotIndex = slotIndex;
        _equipType = equipType;
        _limitLevel = level;
    }

    public void OnClick()
    {
        if (_type == WidgetType.MOLD) {
            UIManager.Instance.OpenWindow<UISmithySelectArmsView>(_moldSlotIndex, _equipType, _limitLevel);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_type == WidgetType.MOLD) {
            return;
        }

        // TODO 显示tip界面
        UIManager.Instance.OpenWindow<UIItemInfoView>(_itemInfo, _itemCfgID);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_type == WidgetType.MOLD) {
            return;
        }

        // 关闭tip界面
        UIManager.Instance.CloseWindow<UIItemInfoView>();
    }
}
