using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public partial class EventID
{
    public const string SMITHY_SELECT_EQUIP = "SMITHY_SELECT_EQUIP";
}


// 铁匠铺界面
public class UISmithyView : UIWindow
{
    public const string Name = "Smithy/UISmithyView";

    public Text _txtLuckSmithing;
    public Text _txtSmithing;
    public Toggle _toggleWeapon;
    public Toggle _toggleArmor;
    public Toggle _toggleDecoration;
    public Toggle _toggleBook;

    public Toggle _toggleSword;
    public Toggle _toggleSpear;
    public Toggle _toggleFan;
    public Toggle _toggleBow;

    public Toggle _toggleCloth;
    public Toggle _toggleChest;

    public RectTransform _panelWeapon;
    public RectTransform _panelArmor;

    public UIListView _listLevel;
    public SmithyItemPanel[] _itemPanel;

    public Image _imgPreview;
    public Text _txtPreviewName;
    public Text _txtBaseAttr;
    public Text[] _txtPinkAttr;
    public Text[] _txtOrangeAttr;
    public Text _txtRandom;
    public Text _txtBase;
    public Text _txtPink;
    public Text _txtOrange;

    private int _smithyLevel;
    private ItemType _equipType = ItemType.NONE;

    private Vector3 _originArmorPosition;
    private Vector3 _originDecorationPosition;
    private Vector3 _originBookPosition;
    private Vector3 _originArmorPanelPositoin;

    private List<SmithyManager.SmithyItemInfo> _needStuffItems;
    private List<ItemInfo> _needMoldItems;
    private int _needMoldCount;
    private int _needMoney;
    private int _needGold;
    private SmithyItemPanel _currentPanel;


    private int _selectEquipID;

    public override void OnOpenWindow()
    {
        _toggleWeapon.isOn = false;
        _toggleArmor.isOn = false;
        _toggleDecoration.isOn = false;
        _toggleBook.isOn = false;

        _toggleCloth.isOn = false;
        _toggleChest.isOn = false;

        _toggleSword.isOn = false;
        _toggleSpear.isOn = false;
        _toggleBow.isOn = false;
        _toggleFan.isOn = false;

        _originArmorPosition = _toggleArmor.transform.localPosition;
        _originDecorationPosition = _toggleDecoration.transform.localPosition;
        _originBookPosition = _toggleBook.transform.localPosition;
        _originArmorPanelPositoin = _panelArmor.transform.localPosition;

        EventDispatcher.AddEventListener<int, ItemInfo>(EventID.SMITHY_SELECT_EQUIP, OnSelectEquip);

        float y = _imgPreview.transform.localPosition.y;
        _imgPreview.transform.DOLocalMoveY(y + 20, 1).SetEase(Ease.OutSine).SetLoops(999, LoopType.Yoyo);
    }

    public override void OnBindData(params object[] param)
    {
        if (param.Length <= 0) {
            _smithyLevel = 1;
        } else {
            _smithyLevel = (int)param[0];
        }

        _equipType = ItemType.SWORD;

        OnClickWeapon(true);
    }

    public override void OnRefreshWindow()
    {
        UpdateList();
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener<int, ItemInfo>(EventID.SMITHY_SELECT_EQUIP, OnSelectEquip);
    }

    private void UpdateList()
    {
        if (_equipType == ItemType.NONE) {
            _listLevel.gameObject.SetActive(false);
            return;
        }

        _listLevel.gameObject.SetActive(true);

        if (_equipType == ItemType.BOOK) {
            // 兵法
            List<BingfaConfig> list = new List<BingfaConfig>();
            foreach (var item in BingfaConfigLoader.Data) {
                if (item.Value.Type == (int)_equipType)
                    list.Add(item.Value);
            }

            _listLevel.MaxCount = list.Count;

            _listLevel.OnListItemAtIndex = (index) =>
            {
                SmithyLevelWidget widget = _listLevel.CreateListItemWidget<SmithyLevelWidget>(0);
                widget.OnClickCallback = OnClickLevel;
                widget.SetInfo(list[index], _smithyLevel < list[index].BuildingLevelDemand);
                return widget;
            };

            _listLevel.Refresh();
        } else {
            // 装备
            List<EquipmentConfig> list = new List<EquipmentConfig>();
            foreach (var item in EquipmentConfigLoader.Data) {
                if (item.Value.Type == (int)_equipType)
                    list.Add(item.Value);
            }

            _listLevel.MaxCount = list.Count;

            _listLevel.OnListItemAtIndex = (index) =>
            {
                SmithyLevelWidget widget = _listLevel.CreateListItemWidget<SmithyLevelWidget>(0);
                widget.OnClickCallback = OnClickLevel;
                widget.SetInfo(list[index], _smithyLevel < list[index].BuildingLevelDemand);
                return widget;
            };

            _listLevel.Refresh();
        }

        SelectLevelByIndex(0);
    }

    // 选择模具
    private void OnSelectEquip(int index, ItemInfo itemInfo)
    {
        _needMoldItems[index] = itemInfo;
        _currentPanel.SetMold(index, itemInfo);
    }

    private void SelectLevelByIndex(int index)
    {
        List<ListItemWidget> listWidgets = _listLevel.ListWidget;
        for (int i = 0; i < listWidgets.Count; ++i) {
            var item = listWidgets[i] as SmithyLevelWidget;
            if (i == index) {
                OnClickLevel(index, item);   
            }
        }
    }

    // 幸运锻造
    public void OnClickLuckSmithing()
    {
        if (!Check()) {
            return;
        }

        SmithyManager.SmithyCost cost = new SmithyManager.SmithyCost();
        cost.Money = _needMoney;
        cost.Gold = _needGold;
        cost.Mold.AddRange(_needMoldItems);
        cost.Material.AddRange(_needStuffItems);
        SmithyManager.Instance.RequestCombineEquip(_selectEquipID, _needMoldItems, true, cost);
    }

    // 锻造
    public void OnClickSmithing()
    {
        if (!Check()) {
            return;
        }

        SmithyManager.SmithyCost cost = new SmithyManager.SmithyCost();
        cost.Money = _needMoney;
        cost.Gold = _needGold;
        cost.Mold.AddRange(_needMoldItems);
        cost.Material.AddRange(_needStuffItems);
        SmithyManager.Instance.RequestCombineEquip(_selectEquipID, _needMoldItems, false, cost);
    }

    private bool Check()
    {
        if (_needMoney > 0 && UserManager.Instance.Money < _needMoney) {
            UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
            return false;
        }

        if (_needGold > 0 && UserManager.Instance.Gold < _needGold) {
            UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_GOLD_LIMIT");
            return false;
        }
        // 模具数量不足
        if (_needMoldItems.Count < _needMoldCount) {
            UIUtil.ShowErrMsgFormat("MSG_MOLD_NOT_ENOUGH");
            return false;
        }

        // 材料不足
        foreach (var item in _needStuffItems) {
            int itemCount = UserManager.Instance.GetItemCount(item.CfgID);
            if (item.CfgID == GameConfig.ITEM_CONFIG_ID_WOOD) {
                // 木材
                if (UserManager.Instance.Wood < item.Count) {
                    UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_WOOD_LIMIT");
                    return false;
                }
            } else if (item.CfgID == GameConfig.ITEM_CONFIG_ID_STONE) {
                // 石材
                if (UserManager.Instance.Stone < item.Count) {
                    UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_STONE_LIMIT");
                    return false;
                }
            } else if (itemCount < item.Count) {
                // 普通材料
                UIUtil.ShowErrMsgFormat("MSG_STUFF_NOT_ENOUGH");
                return false;
            }
        }
        return true;
    }

    // 武器
    public void OnClickWeapon(bool value)
    {
        _toggleWeapon.isOn = value;
        _toggleCloth.isOn = false;
        _toggleChest.isOn = false;

        _toggleSword.isOn = true;

        UpdatePosition(true, value, false);
        UpdatePanel(_panelWeapon, value);
        UpdatePanel(_panelArmor, false);
    }

    // 防具
    public void OnClickArmor(bool value)
    {
        _toggleSword.isOn = false;
        _toggleSpear.isOn = false;
        _toggleBow.isOn = false;
        _toggleFan.isOn = false;

        _toggleCloth.isOn = true;

        UpdatePosition(true, false, value);
        UpdatePanel(_panelArmor, value);
        UpdatePanel(_panelWeapon, false);
    }

    // 饰品
    public void OnClickDecoration(bool value)
    {
        if (value) {
            _equipType = ItemType.DECORATION;
        } else {
            _equipType = ItemType.NONE;
        }

        UpdateList();
    }

    // 兵法
    public void OnClickBook(bool value)
    {
        if (value) {
            _equipType = ItemType.BOOK;
        } else {
            _equipType = ItemType.NONE;
        }

        UpdateList();
    }

    // 刀
    public void OnClickSword(bool value)
    {
        if (value) {
            _equipType = ItemType.SWORD;
            UpdateList();
        }
    }

    // 枪
    public void OnClickSpear(bool value)
    {
        if (value) {
            _equipType = ItemType.SPEAR;
            UpdateList();
        }
    }

    // 扇
    public void OnClickFan(bool value)
    {
        if (value) {
            _equipType = ItemType.FAN;
            UpdateList();
        }
    }

    // 弓
    public void OnClickBow(bool value)
    {
        if (value) {
            _equipType = ItemType.BOW;
            UpdateList();
        }
    }

    // 布甲
    public void OnClickCloth(bool value)
    {
        if (value) {
            _equipType = ItemType.CLOTH;
            UpdateList();
        }
    }

    // 重甲
    public void OnClickChest(bool value)
    {
        if (value) {
            _equipType = ItemType.CHEST;
            UpdateList();
        } 
    }

    // 点选装备
    private void OnClickLevel(int index, ListItemWidget widget)
    {
        SmithyLevelWidget w = widget as SmithyLevelWidget;
        if (w != null) {
            SelectEquip(w.EquipID);
        }
    }

    private void SelectEquip(int equipID)
    {
        _selectEquipID = equipID;

        if (_equipType == ItemType.BOOK) {
            // 选择的是兵法
            BingfaConfig cfg = BingfaConfigLoader.GetConfig(_selectEquipID);
            _needMoldCount = cfg.MoldDemand;
            _needStuffItems = SmithyManager.Instance.ParseItems(cfg.MaterialDemand);
            _needMoldItems = SmithyManager.Instance.SelectModelBook(_selectEquipID);
            _needMoney = 0;
            _needGold = 0;
        } else {
            // 选择的是装备
            EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(_selectEquipID);
            _needMoldCount = cfg.MoldDemand;
            _needStuffItems = SmithyManager.Instance.ParseItems(cfg.MaterialDemand);
            _needMoldItems = SmithyManager.Instance.SelectModelEquip(_selectEquipID);
            _needMoney = cfg.GoldDemand;
            _needGold = cfg.LuckyBuild;
        }

        foreach (var item in _itemPanel) {
            item.gameObject.SetActive(false);
        }

        int count = _needMoldCount + _needStuffItems.Count;
        _currentPanel = _itemPanel[count - 2];
        _currentPanel.gameObject.SetActive(true);
        _currentPanel.SetInfo(equipID, _needMoldItems, _needStuffItems, _needMoldCount);

        _txtSmithing.text = _needMoney.ToString();
        _txtLuckSmithing.text = _needGold.ToString();

        UpdatePreview();
    }

    // 更新预览的内容
    private void UpdatePreview()
    {
        if (_equipType == ItemType.BOOK) {
            _imgPreview.sprite = ResourceManager.Instance.GetBookImage(_selectEquipID);
            _txtRandom.gameObject.SetActive(true);
            _txtBase.gameObject.SetActive(false);
            _txtPink.gameObject.SetActive(false);
            _txtOrange.gameObject.SetActive(false);

            BingfaConfig cfg = BingfaConfigLoader.GetConfig(_selectEquipID);
            _txtPreviewName.text = Str.Format("UI_LEVEL", cfg.EquipLevel) + "  " + cfg.Name;
        } else {
            EquipmentConfig cfg = EquipmentConfigLoader.GetConfig(_selectEquipID);
            _imgPreview.sprite = ResourceManager.Instance.GetEquipImage(_selectEquipID);
            _txtRandom.gameObject.SetActive(false);
            _txtBase.gameObject.SetActive(true);
            _txtPink.gameObject.SetActive(true);
            _txtOrange.gameObject.SetActive(true);

            _txtBaseAttr.text = SmithyManager.Instance.GetBaseAttr(_selectEquipID);

            _txtPreviewName.text = Str.Format("UI_LEVEL", cfg.EquipLevel) + "  " + cfg.Name;

            _txtPinkAttr[0].text = SmithyManager.Instance.GetAddAttr(cfg.PurpleAttribute1);
            _txtOrangeAttr[0].text = SmithyManager.Instance.GetAddAttr(cfg.OrangeAttribute1);
            _txtOrangeAttr[1].text = SmithyManager.Instance.GetAddAttr(cfg.OrangeAttribute2);
        }
    }

    private void UpdatePosition(bool withAnimation, bool showWeaponPanel, bool showArmorPanel)
    {
        float weaponAddHeight = 0;
        float armorAddHeight = 0;
        if (showWeaponPanel) {
            weaponAddHeight = _panelWeapon.sizeDelta.y;
        }

        if (showArmorPanel) {
            armorAddHeight = _panelArmor.sizeDelta.y;
        }

        Vector3 destArmor = _originArmorPosition - new Vector3(0, weaponAddHeight, 0);
        Vector3 destDecoration = _originDecorationPosition - new Vector3(0, weaponAddHeight + armorAddHeight, 0);
        Vector3 destBook = _originBookPosition - new Vector3(0, weaponAddHeight + armorAddHeight, 0);

        const float ANIMATION_TIME = 0.2f;
        if (withAnimation) {
            _toggleArmor.transform.DOLocalMoveY(destArmor.y, ANIMATION_TIME);
            _toggleDecoration.transform.DOLocalMoveY(destDecoration.y, ANIMATION_TIME);
            _toggleBook.transform.DOLocalMoveY(destBook.y, ANIMATION_TIME);
        } else {
            _toggleArmor.transform.localPosition = destArmor;
            _toggleDecoration.transform.localPosition = destDecoration;
            _toggleBook.transform.localPosition = destBook;
        }
    }

    private void UpdatePanel(RectTransform panel, bool show)
    {
        const float ANIMATION_TIME = 0.2f;
        if (show) {
            if (panel.gameObject.activeInHierarchy) {
                return;
            }
            panel.gameObject.SetActive(true);
            panel.transform.localScale = new Vector3(1, 0, 1);
            panel.DOScaleY(1, ANIMATION_TIME);
        } else {
            if (!panel.gameObject.activeInHierarchy) {
                return;
            }
            panel.DOScaleY(0, ANIMATION_TIME).OnComplete(() =>
            {
                panel.gameObject.SetActive(false);
            });
        }
    }
}
