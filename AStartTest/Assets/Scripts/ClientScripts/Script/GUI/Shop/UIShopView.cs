using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 商店
public class UIShopView : UIWindow
{
    public const string Name = "Shop/UIShopView";
    //商店选项
    public Toggle[] _tabToggleList;
    //商品刷新时间
    public Text _txtRefreshTime;
    public Text _updateTime;
    public UIListView _listView;

    private ShopType _shopType; // 当前商店分页
    private bool _hasRefreshPage = false;

    public override void OnOpenWindow()
    {
        EventDispatcher.AddEventListener<ShopType>(EventID.EVENT_SHOP_REFRESH_SHOP, OnRefreshShop);

        // 请求商店数据
        ShopManager.Instance.RequestShopInfo(ShopType.SUNDRY);
        ShopManager.Instance.RequestShopInfo(ShopType.BLACK_MARKET);
        ShopManager.Instance.RequestShopInfo(ShopType.PVP);
        ShopManager.Instance.RequestShopInfo(ShopType.GUILD);

        foreach (var item in ShopManager.Instance.ShopInfoData) {
            item.Value.hasRequest = false;
        }
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener<ShopType>(EventID.EVENT_SHOP_REFRESH_SHOP, OnRefreshShop);
    }

    void Update()
    {
        // 时间到后重新请求当前数据
        ShopInfo shopInfo = ShopManager.Instance.GetShopInfo(_shopType);
        if (shopInfo != null && !shopInfo.hasRequest && shopInfo.refreshCD.GetTime() <= 0) {
            shopInfo.hasRequest = true;
            ShopManager.Instance.RequestShopInfo(_shopType);
        }
    }
    
    public override void OnBindData(params object[] param)
    {
        // 设置显示哪个分页
        if (param.Length > 0) {
            _shopType = (ShopType) param[0];
        } else {
            _shopType = ShopType.SUNDRY;
        }
    }

    private void UpdateToggle()
    {
        switch (_shopType) {
            case ShopType.SUNDRY:
                _tabToggleList[0].isOn = true;
                break;
            case ShopType.BLACK_MARKET:
                _tabToggleList[1].isOn = true;
                break;
            case ShopType.PVP:
                _tabToggleList[2].isOn = true;
                break;
            case ShopType.GUILD:
                _tabToggleList[3].isOn = true;
                break;
        }
    }

    public override void OnRefreshWindow()
    {
        UpdateToggle();

        ShopInfo shopInfo = ShopManager.Instance.GetShopInfo(_shopType);
        if (shopInfo == null) {
            return;
        }

        // 刷新倒计时
        _txtRefreshTime.gameObject.SetActive(shopInfo.refreshCD.IsValid());
        _updateTime.text = Utils.GetCountDownString(shopInfo.refreshCD.GetTime());

        _listView.Data = shopInfo.itemList.ToArray();
        _listView.Refresh();
    }

    private void OnRefreshShop(ShopType shopType)
    {
        if (_shopType != shopType) return;
        OnRefreshWindow();
    }

    //以下四个属于Toggle点击事件  选择商店
    public void OnClickGroceriesShop(bool value)
    {
        if (!_tabToggleList[0].isOn || _shopType == ShopType.SUNDRY) return;
        _shopType = ShopType.SUNDRY;
        OnRefreshWindow();
    }

    public void OnClickBlackShop(bool value)
    {
        if (!_tabToggleList[1].isOn || _shopType == ShopType.BLACK_MARKET) return;
        _shopType = ShopType.BLACK_MARKET;
        OnRefreshWindow();
    }

    public void OnClickPVPShop(bool value)
    {
        if (!_tabToggleList[2].isOn || _shopType == ShopType.PVP) return;
        _shopType = ShopType.PVP;
        OnRefreshWindow();
    }

    public void OnClickMettingShop(bool value)
    {
        if (!_tabToggleList[3].isOn || _shopType == ShopType.GUILD) return;
        _shopType = ShopType.GUILD;
        OnRefreshWindow();
    }

    //以下四个属于资源Button触发事件 购买金钱 木材等
    public void OnClickAddMoney()
    {

    }

    public void OnClickAddWood()
    {
        
    }

    public void OnClickAddStone()
    {
        
    }

    public void OnClickAddGold()
    {
        
    }

    // 刷新商店 
    public void OnClickUpdateShop()
    {
        ShopInfo info = ShopManager.Instance.GetShopInfo(_shopType);
        if (info == null) {
            return;
        }

        UIUtil.ShowConfirm(Str.Format("UI_SHOP_REFRESH_CONFIRM", info.GetRefreshCost()), "", () => {
            ShopManager.Instance.RequestRefreshShop(_shopType);
        });
    }
}
