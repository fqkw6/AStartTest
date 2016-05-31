using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITavernLeft1Panel : MonoBehaviour
{
    public Text _txtFreeCountText;
    public Text _txtFreeCount;
    public Text _txtCost;
    public Image _imgFreeFlag;


	void Start ()
    {
        InvokeRepeating("UpdateTime", 0, 1);
	}

    private void UpdateTime()
    {
        if (ShopManager.Instance.GetMoneyFreeCD() <= 0) {
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_COUNT");
            _txtFreeCount.text = string.Format("({0}/{1})", ShopManager.Instance.MoneyFreeCount, GameConfig.LUCK_DRAW_MAX_FREE_COUNT);
        } else {
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_TIME");
            _txtFreeCount.text = Utils.GetCountDownString(ShopManager.Instance.GetMoneyFreeCD());
        }
    }

    public void Refresh()
    {
        // 有免费次数
        if (ShopManager.Instance.GetMoneyFreeCD() <= 0) {
            _imgFreeFlag.gameObject.SetActive(true);
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_COUNT");
            _txtFreeCount.text = string.Format("({0}/{1})", ShopManager.Instance.MoneyFreeCount, GameConfig.LUCK_DRAW_MAX_FREE_COUNT);
            _txtCost.text = Str.Get("UI_TAVERN_FREE");
        } else {
            _imgFreeFlag.gameObject.SetActive(false);
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_TIME");
            _txtFreeCount.text = Utils.GetCountDownString(ShopManager.Instance.GetMoneyFreeCD());
            _txtCost.text = GameConfig.LUCK_DRAW_MONEY_10_COST.ToString();
        }
    }
}
