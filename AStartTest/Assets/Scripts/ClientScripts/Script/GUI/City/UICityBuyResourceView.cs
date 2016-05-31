using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICityBuyResourceView : UIWindow
{
    public const string Name = "City/UICityBuyResourceView";
    public Text _txtAcerNumber ;
    public Image _imageResourceIcon;
    public Text _txtResourceNumber;
    public Slider _sliderResourceNumber;

    private float _curValue;
    private ResourceType _resType;
    private int _maxValue;
    private int _proportion;
    private int _curGold;
    private int _curRes;


    public override void OnBindData(params object[] param)
    {
        _resType = (ResourceType)param[0];
    }

    public override void OnRefreshWindow()
    {
        _imageResourceIcon.sprite = ResourceManager.Instance.GetResIcon(_resType);
        if (_resType == ResourceType.MONEY) {
            _maxValue = GetBuyResourceMaxGold(UserManager.Instance.MaxMoneyStorage,UserManager.Instance.Money,GameConfig.BUY_MONEY_UNIT_COST);
            _proportion = GameConfig.BUY_MONEY_UNIT_COST;
        } else if (_resType == ResourceType.WOOD) {
            _maxValue = GetBuyResourceMaxGold(UserManager.Instance.MaxWoodStorage, UserManager.Instance.Wood, GameConfig.BUY_WOOD_UNIT_COST);
            _proportion = GameConfig.BUY_WOOD_UNIT_COST;
        } else if (_resType == ResourceType.STONE) {
            _maxValue = GetBuyResourceMaxGold(UserManager.Instance.MaxStoneStorage, UserManager.Instance.Stone, GameConfig.BUY_STONE_UNIT_COST);
            _proportion = GameConfig.BUY_STONE_UNIT_COST; ;
        }
        SetValue(1);
        _sliderResourceNumber.value = 1;
    }

    public int GetBuyResourceMaxGold(int max, int curr, int unit)
    {
        int value = (max - curr)/unit;
        value = Mathf.FloorToInt(value);
        return Mathf.Min(value,UserManager.Instance.Gold);
    }

    //根据百分比设置界面的资源值跟元宝值
    public void SetValue(float percent)
    {
        _curValue = percent;
        int value = Mathf.FloorToInt(percent * _maxValue);
        if (value < 0 ) {
            value = 0;
        }
        _txtAcerNumber.text = value.ToString();
        _txtResourceNumber.text = (value*_proportion).ToString();

        _curGold = value;
        _curRes = value*_proportion;
    }

    public void OnClickReduction()
    {
        _curValue = Mathf.Max(_curValue-0.1f,0);
        SetValue(_curValue);
        _sliderResourceNumber.value = _curValue;
    }

    public void OnClickAdd()
    {
        _curValue = Mathf.Min(_curValue+0.1f,1);
        SetValue(_curValue);
        _sliderResourceNumber.value = _curValue;
    }

    public void OnValueChange(float value)
    {
        _curValue = _sliderResourceNumber.value;
        SetValue(_curValue);
    }

    public void OnClickBuy()
    {
        UserManager.Instance.RequestBuyRes(_resType, _curGold, _curRes);
        CloseWindow();
    }
}
