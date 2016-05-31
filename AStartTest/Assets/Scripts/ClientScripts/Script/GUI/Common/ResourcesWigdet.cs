using UnityEngine;
using System.Collections;
using comrt.comnet;
using UnityEngine.UI;

public class ResourcesWigdet : MonoBehaviour
{

    public Text _txtMoneyNumber;
    public Text _txtWoodNumber;
    public Text _txtStoneNumber;
    public Text _txtGoldNumber;
    public Text _txtSpNumber;
    public Image _imageMoneyPrg;
    public Image _imageWoodPrg;
    public Image _imageStonePrg;
    public RectTransform _rectMoneyTransform;
    public RectTransform _rectWoodTransform;
    public RectTransform _rectStoneTransform;

    // Use this for initialization
    void Start()
    {
        EventDispatcher.AddEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshAttribute);
        if (_txtSpNumber != null) {
            InvokeRepeating("SyncSP", 0, GameConfig.SP_GET_INTERVAL);
            EventDispatcher.AddEventListener(EventID.EVENT_UI_REFRESH_SP, RefreshValue);
        }
        OnRefreshAttribute();
        RefreshValue();
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveEventListener(EventID.EVENT_UI_MAIN_REFRESH_VALUE, OnRefreshAttribute);
        if (_txtSpNumber != null) {
            EventDispatcher.RemoveEventListener(EventID.EVENT_UI_REFRESH_SP, RefreshValue);
            CancelInvoke("SyncSP");
        }
    }

    public void OnRefreshAttribute()
    {
        if (_txtMoneyNumber != null) _txtMoneyNumber.text = Utils.GetMoneyString(UserManager.Instance.Money);
        if (_txtWoodNumber != null) _txtWoodNumber.text = Utils.GetMoneyString(UserManager.Instance.Wood);
        if (_txtStoneNumber != null) _txtStoneNumber.text = Utils.GetMoneyString(UserManager.Instance.Stone);
        if (_txtGoldNumber != null) _txtGoldNumber.text = UserManager.Instance.Gold.ToString();
        if (_imageMoneyPrg != null) {
            if (UserManager.Instance.MaxMoneyStorage > 0) {
                _imageMoneyPrg.gameObject.SetActive(true);
                _imageMoneyPrg.fillAmount = 1.0f * UserManager.Instance.Money / UserManager.Instance.MaxMoneyStorage;
            } else {
                _imageMoneyPrg.gameObject.SetActive(false);
            }
        }
        if (_imageStonePrg != null) {
            if (UserManager.Instance.MaxStoneStorage > 0) {
                _imageStonePrg.fillAmount = 1.0f*UserManager.Instance.Stone/UserManager.Instance.MaxStoneStorage;
                _imageStonePrg.gameObject.SetActive(true);
            } else {
                _imageStonePrg.gameObject.SetActive(false);
            }
        }
        if (_imageWoodPrg != null) {
            if (UserManager.Instance.MaxWoodStorage > 0) {
                _imageWoodPrg.fillAmount = 1.0f*UserManager.Instance.Wood/UserManager.Instance.MaxWoodStorage;
                _imageWoodPrg.gameObject.SetActive(true);
            } else {
                _imageWoodPrg.gameObject.SetActive(false);
            }
        }
    }

    private void RefreshValue()
    {
        if (_txtSpNumber == null) return;

        int curSp = UserManager.Instance.SP;
        int maxSp = UserManager.Instance.GetMaxSP();
        string color = "white";
        if (curSp == 0) {
            color = "red";
        } else if (curSp >= maxSp) {
            color = "#00ffffff";
        }
        _txtSpNumber.text = string.Format("<color={2}>{0}</color>/{1}", curSp, maxSp, color);
    }

    private void SyncSP()
    {
        Net.Send(eCommand.GET_ENERGY);
    }

    //点击金钱 木材 底框弹出资源的详细信息
    public void OnClickMoneyFrame()
    {
        UIManager.Instance.OpenWindow<UICityResourcesDataView>(ResourceType.MONEY, _rectMoneyTransform.anchoredPosition);
    }

    public void OnClickWoodFrame()
    {
        UIManager.Instance.OpenWindow<UICityResourcesDataView>(ResourceType.WOOD, _rectWoodTransform.anchoredPosition);
    }

    public void OnClickStoneFrame()
    {
        UIManager.Instance.OpenWindow<UICityResourcesDataView>(ResourceType.STONE, _rectStoneTransform.anchoredPosition);
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

    public void OnClickAddSp()
    {
        UIUtil.ShowConfirm(Str.Format("MSG_BUY_SP_CONFIRM", GameConfig.BUY_SP_COST, GameConfig.BUY_SP_GET), "", () => {
            PVEManager.Instance.RequestAddSp();
        });
    }
}
