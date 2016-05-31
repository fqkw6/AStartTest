using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITavernRight1Panel : MonoBehaviour
{
    public Text _txtFreeCountText;
    public Text _txtFreeCount;
    public Text _txtCost;
    public Image _imgFreeFlag;


    // Use this for initialization
    void Start () {
	    InvokeRepeating("UpdateTime", 0, 1);
	}

    private void UpdateTime()
    {
        if (ShopManager.Instance.GetGoldFreeCD() <= 0) {
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_COUNT");
            _txtFreeCount.text = string.Format("({0}/{1})", 1, 1);
        } else {
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_TIME");
            _txtFreeCount.text = Utils.GetCountDownString(ShopManager.Instance.GetGoldFreeCD());
        }
    }

    public void Refresh()
    {
        // 有免费次数
        if (ShopManager.Instance.GetGoldFreeCD() <= 0) {
            _imgFreeFlag.gameObject.SetActive(true);
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_COUNT");
            _txtFreeCount.text = string.Format("({0}/{1})", 1, 1);
            _txtCost.text = Str.Get("UI_TAVERN_FREE");
        } else {
            _imgFreeFlag.gameObject.SetActive(false);
            _txtFreeCountText.text = Str.Get("UI_TAVERN_FREE_TIME");
            _txtFreeCount.text = Utils.GetCountDownString(ShopManager.Instance.GetGoldFreeCD());
            _txtCost.text = GameConfig.LUCK_DRAW_GOLD_10_COST.ToString();
        }
    }
}
