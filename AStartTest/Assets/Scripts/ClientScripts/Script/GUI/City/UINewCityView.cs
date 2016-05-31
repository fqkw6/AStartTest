using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UINewCityView : UIWindow
{
    public const string Name = "City/UINewCityView";
    public Text _txtPlayerName;
    public Text _txtPlayerGrade;
    public Text _txtPlayerVip;
    public Image _imgPlayerIcon;
    public Text _txtPlayerForce;
    public Image _imageActivityFlag;
    public Image _imageTaskFlag;
    public Image _imageRowFlag;
    public Image _imageVipFlag;
    public Image _imagePubFlag;

    private HeroInfo _currentInfo;

    public override void OnOpenWindow()
    {
        EventDispatcher.AddEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshAttribute);
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshAttribute);
    }

    public override void OnRefreshWindow()
    {
        if (_imageActivityFlag != null) _imageActivityFlag.gameObject.SetActive(false);
        if (_imageTaskFlag != null) _imageTaskFlag.gameObject.SetActive(false);
        if (_imageRowFlag != null) _imageRowFlag.gameObject.SetActive(false);
        if (_imageVipFlag != null) _imageVipFlag.gameObject.SetActive(false);
        if (_imagePubFlag != null) _imagePubFlag.gameObject.SetActive(false);

        _txtPlayerForce.text = UserManager.Instance.GetFightScore().ToString();
        _imgPlayerIcon.sprite = ResourceManager.Instance.GetPlayerIcon(UserManager.Instance.Icon);

        OnRefreshAttribute();
    }

    public void OnRefreshAttribute()
    {
        _txtPlayerName.text = UserManager.Instance.RoleName;
        _txtPlayerGrade.text = UserManager.Instance.Level.ToString();
        _txtPlayerForce.text = UserManager.Instance.GetFightScore().ToString();
    }

    // 购买金钱 木材等
    public void OnClickAddMoney()
    {
        UIManager.Instance.OpenWindow<UICityBuyResourceView>(ResourceType.MONEY);
    }

    public void OnClickAddWood()
    {
        UIManager.Instance.OpenWindow<UICityBuyResourceView>(ResourceType.WOOD);
    }

    public void OnClickAddStone()
    {
        UIManager.Instance.OpenWindow<UICityBuyResourceView>(ResourceType.STONE);
    }

    public void OnClickAddGold()
    {
    }

    // 战报界面
    public void OnClickReport()
    {
        UIManager.Instance.OpenWindow<UIPVPReportView>();
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

    public void OnClickReturnMain()
    {
        UIManager.Instance.OpenWindow<UINewMainView>();
        UIManager.Instance.CloseAllWindow();
        Game.Instance.ShowMainCity(false);
        UIManager.Instance.CloseWindow<UICityView>();
        UIManager.Instance.CloseWindow<UIWorldMapView>();
        CloseWindow();
    }

    public void OnClickSmithy()
    {
        UIManager.Instance.OpenWindow<UISmithyView>(CityManager.Instance.GetSmithyLevel());
    }
}
