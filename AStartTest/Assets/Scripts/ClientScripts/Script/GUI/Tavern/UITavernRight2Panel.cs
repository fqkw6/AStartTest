using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITavernRight2Panel : MonoBehaviour
{
    public Text _txtBuyCount;
    public Text _txtFreeTime;
    public Text _txtBuy1Cost;
    public Text _txtBuy10Cost;
    public Image _imgFreeFlag;

    void Start()
    {
        InvokeRepeating("UpdateTime", 0, 1);
    }

    public void Refresh()
    {
        _txtBuyCount.text = string.Format(Str.Get("UI_TAVERN_GOLD_LEFT_COUNT"), ShopManager.Instance.GoldLeftCount);

        if (ShopManager.Instance.GetGoldFreeCD() <= 0)
        {
            _txtFreeTime.gameObject.SetActive(false);
            _txtBuy1Cost.text = Str.Get("UI_TAVERN_FREE");
            _imgFreeFlag.gameObject.SetActive(true);
        }
        else
        {
            _txtFreeTime.gameObject.SetActive(true);
            _txtFreeTime.text = string.Format(Str.Get("UI_TAVERN_FREE_TIME2"), Utils.GetCountDownString(ShopManager.Instance.GetGoldFreeCD()));
            _txtBuy1Cost.text = GameConfig.LUCK_DRAW_GOLD_1_COST.ToString();
            _imgFreeFlag.gameObject.SetActive(false);
        }

        _txtBuy10Cost.text = GameConfig.LUCK_DRAW_GOLD_10_COST.ToString();
    }

    private void UpdateTime()
    {
        if (ShopManager.Instance.GetGoldFreeCD() <= 0)
        {
            _txtFreeTime.gameObject.SetActive(false);
        }
        else
        {
            _txtFreeTime.gameObject.SetActive(true);
            _txtFreeTime.text = string.Format(Str.Get("UI_TAVERN_FREE_TIME2"), Utils.GetCountDownString(ShopManager.Instance.GetGoldFreeCD()));
        }
    }

    private void OnGoldLimit()
    {
        UIManager.Instance.OpenWindow<UIMsgBoxPurchaseView>();
    }

    public void OnClickBuy1()
    {
        if (ShopManager.Instance.GetGoldFreeCD() <= 0)
        {
            // 免费抽奖
            ShopManager.Instance.RequestTavernGoldFreeBuy();
        }
        else
        {
            if (UserManager.Instance.Gold < GameConfig.LUCK_DRAW_GOLD_1_COST)
            {
                OnGoldLimit();
            }
            ShopManager.Instance.RequestTavernGoldBuy1();
        }

    }

    public void OnClickBuy10()
    {
        if (UserManager.Instance.Gold < GameConfig.LUCK_DRAW_GOLD_10_COST)
        {
            OnGoldLimit();
        }

        ShopManager.Instance.RequestTavernGoldBuy10();
    }
}
