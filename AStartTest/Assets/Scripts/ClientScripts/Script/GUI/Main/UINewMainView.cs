using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// 主界面
public class UINewMainView : UIWindow
{
    public const string Name = "Main/UINewMainView";
    public Text _txtPlayName;
    public Text _txtPlayGrade;
    public Image _imgPlayerIcon;
    public Text _txtPlayVip;
    public Text _txtPlayerForce;
    public Image _imageExpPrg;
    public Image _imageBuddyFlag;
    public Image _imageMailFlag;
    public Image _imageActivityFlag;
    public Image _imageTaskFlag;
    public Image _imageRowFlag;
    public Image _imageVipFlag;
    public Image _imagePubFlag;
    public Image _imgBagFlag;

    public override void OnOpenWindow()
    {
        IsMainWindow = true;
        EventDispatcher.AddEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshAttribute);
        EventDispatcher.AddEventListener<GameFunc, bool>(EventID.EVENT_UI_MAIN_NEW_FLAG, OnRefreshFlag);

        CityManager.Instance.RequestBuildingList(); // 请求建筑列表，为了获取战斗力数据
        PVEManager.Instance.RequestLevelInfo(0);   // 请求副本数据
        PVEManager.Instance.RequestLevelPosition();     // 请求当前副本的位置
        PVEManager.Instance.RequestChapterAwardInfo();  // 请求副本奖励领取情况
        ShopManager.Instance.RequestTavernBuyInfo();    // 获取酒馆抽奖的信息
        MailManager.Instance.RequestMailList();         // 获取邮件列表

        ClearAllFlags();
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshAttribute);
        EventDispatcher.RemoveEventListener<GameFunc, bool>(EventID.EVENT_UI_MAIN_NEW_FLAG, OnRefreshFlag);
        EventDispatcher.TriggerEvent(EventID.EVENT_UI_HERO_VIEW_CLOSE);
    }

    public override void OnRefreshWindow()
    {
        _imgPlayerIcon.sprite = ResourceManager.Instance.GetPlayerIcon(UserManager.Instance.Icon);
        OnRefreshAttribute();
    }

    private void ClearAllFlags()
    {
        if (_imageBuddyFlag != null) _imageBuddyFlag.gameObject.SetActive(false);
        if (_imageMailFlag != null) _imageMailFlag.gameObject.SetActive(false);
        if (_imageActivityFlag != null) _imageActivityFlag.gameObject.SetActive(false);
        if (_imageTaskFlag != null) _imageTaskFlag.gameObject.SetActive(false);
        if (_imageRowFlag != null) _imageRowFlag.gameObject.SetActive(false);
        if (_imageVipFlag != null) _imageVipFlag.gameObject.SetActive(false);
        if (_imagePubFlag != null) _imagePubFlag.gameObject.SetActive(false);
        if (_imgBagFlag != null) _imgBagFlag.gameObject.SetActive(false);
    }

    private void OnRefreshFlag(GameFunc func, bool show)
    {
        switch (func) {
            case GameFunc.NONE:
                ClearAllFlags();
                break;
            case GameFunc.BAG:
                if (_imgBagFlag != null) _imgBagFlag.gameObject.SetActive(show);
                break;
        }
    }

    private void OnRefreshAttribute()
    {
        _txtPlayName.text = UserManager.Instance.RoleName;
        _txtPlayGrade.text = UserManager.Instance.Level.ToString();
        _txtPlayerForce.text = UserManager.Instance.GetFightScore().ToString();

        PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(UserManager.Instance.Level);
        if (cfg != null && cfg.Exp > 0) {
            _imageExpPrg.fillAmount = 1f*UserManager.Instance.Exp/cfg.Exp;
        } else {
            _imageExpPrg.fillAmount = 1;

        }
    }

    // 首充奖励
    public void OnClickOneRecharge()
    {
        
    }

    //增加金钱 体力 元宝
    public void OnClickAddMoney()
    {
        UIManager.Instance.OpenWindow<UICityBuyResourceView>(ResourceType.MONEY);
    }

    public void OnClickAddSp()
    {
        PVEManager.Instance.RequestAddSp();
    }

    public void OnClickAddGlod()
    {
    }

    public void OnClickSmithy()
    {
        UIManager.Instance.OpenWindow<UISmithyView>(CityManager.Instance.GetSmithyLevel());
    }

    //战况
    public void OnClickWar()
    {

    }

    //好友
    public void OnClickBuddy()
    {
        
    }

    //邮件
    public void OnClickMail()
    {
        UIManager.Instance.OpenWindow<UIMailView>();
    }

    //设置
    public void OnClickSet()
    {

    }

    //武将
    public void OnClickHero()
    {
        UIManager.Instance.OpenWindow<UINewHeroListView>();
    }

    //士兵布阵
    public void OnClickSoldier()
    {

    }

    //公会
    public void OnClickGuild()
    {

    }

    //背包
    public void OnClickPack()
    {
        UIManager.Instance.OpenWindow<UINewBagView>();
    }

    //商店
    public void OnClickShop()
    {
        UIManager.Instance.OpenWindow<UIShopView>();
    }

    //活动
    public void OnClickActivity()
    {
        
    }

    //任务
    public void OnClickTask()
    {
        
    }

    //排名
    public void OnClickRowName()
    {
        UIManager.Instance.OpenWindow<UIPVPView>();
    }

    //会员
    public void OnClickVip()
    {

    }

    //酒馆
    public void OnClickTavern()
    {
        UIManager.Instance.OpenWindow<UITavernView>();
    }

    //主城
    public void OnClickMainCity()
    {
        // 关闭其他界面
        UIManager.Instance.CloseAllWindow();

        // 关闭主城地图和主界面ui
        Game.Instance.ShowMainCity(true);
        UIManager.Instance.OpenWindow<UICityView>();
        UIManager.Instance.OpenWindow<UINewCityView>();
        CloseWindow();
    }

    //玩家对战
    public void OnClickPlayPVP()
    {

    }

    //攻城夺地
    public void OnClickAttackCity()
    {
        // 关闭其他界面
        UIManager.Instance.CloseAllWindow();

        // 打开世界地图和其ui
        UIManager.Instance.OpenWindow<UIWorldMapView>();
        UIManager.Instance.OpenWindow<UINewCityView>();


        // 关闭主城地图和主界面ui
        // Game.Instance.ShowMainCity(true);
        CloseWindow();
    }

    //副本模式
    public void OnClickPVE()
    {
        UIManager.Instance.OpenWindow<UINewPVEEntranceView>();
    }

    public void OnClickIcon()
    {
        UIManager.Instance.OpenWindow<UIPlayerMenuView>();
    }
}
