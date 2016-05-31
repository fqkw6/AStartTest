using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITavernLeft2Panel : MonoBehaviour
{
    public Text _txtFreeCount;
    public Text _txtFreeCD;
    public Text _txtBuy1Cost;
    public Text _txtBuy10Cost;
    public Image _imgFreeFlag;

	void Start ()
    {
        InvokeRepeating("UpdateTime", 0, 1);
    }

    public void Refresh()
    {
        if (ShopManager.Instance.GetMoneyFreeCD() <= 0) {
            _txtFreeCount.text = string.Format(Str.Get("UI_TAVERN_FREE_COUNT2"), ShopManager.Instance.MoneyFreeCount);
            _imgFreeFlag.gameObject.SetActive(true);
            _txtBuy1Cost.text = Str.Get("UI_TAVERN_FREE");
            _txtFreeCD.gameObject.SetActive(false);
        } else {
            _txtFreeCount.text = string.Format(Str.Get("UI_TAVERN_FREE_COUNT2"), ShopManager.Instance.MoneyFreeCount);
            _imgFreeFlag.gameObject.SetActive(false);
            _txtBuy1Cost.text = GameConfig.LUCK_DRAW_MONEY_1_COST.ToString();
            _txtFreeCD.text = string.Format(Str.Get("UI_TAVERN_FREE_TIME2"), Utils.GetCountDownString(ShopManager.Instance.GetMoneyFreeCD()));
            _txtFreeCD.gameObject.SetActive(true);
        }

        _txtBuy10Cost.text = GameConfig.LUCK_DRAW_MONEY_10_COST.ToString();
    }

    private void UpdateTime()
    {
        if (ShopManager.Instance.GetMoneyFreeCD() <= 0) {
            _txtFreeCD.gameObject.SetActive(false);
        } else {
            _txtFreeCD.gameObject.SetActive(true);
            _txtFreeCD.text = string.Format(Str.Get("UI_TAVERN_FREE_TIME2"), Utils.GetCountDownString(ShopManager.Instance.GetMoneyFreeCD()));
        }
    }

    public void OnClickBuy1()
    {
        if (ShopManager.Instance.GetMoneyFreeCD() <= 0) {
            // 免费抽奖
            ShopManager.Instance.RequestTavernMoneyFreeBuy();
        } else {
            if (UserManager.Instance.Money < GameConfig.LUCK_DRAW_MONEY_1_COST) {
                // 银两不足
                UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
                return;
            }

            ShopManager.Instance.RequestTavernMoneyBuy1();
        }
    }

    public void OnClickBuy10()
    {
        if (UserManager.Instance.Money < GameConfig.LUCK_DRAW_MONEY_10_COST) {
            // 银两不足
            UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
            return;
        }
        ShopManager.Instance.RequestTavernMoneyBuy10();
    }
}
