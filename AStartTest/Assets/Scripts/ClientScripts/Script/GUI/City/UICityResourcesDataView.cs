using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICityResourcesDataView : UIWindow
{
    public const string Name = "City/UICityResourcesDataView";
    public Text _txtResourceTitle;
    public Text _txtHousesResourceNumber;
    public Text _txtBuilding;
    public Text _txtCityResource; 
    public Text _txtDepositMaxrResource;
    private ResourceType _resType;
    private Vector2 _startPos;

    public override void OnBindData(params object[] param)
    {
        _resType = (ResourceType)param[0];
        _startPos = (Vector2)param[1];

        RectTransform rcTransform = GetComponent<RectTransform>();
        if (rcTransform) {
            rcTransform.anchoredPosition = _startPos + new Vector2(0, 133);
        }
    }

    public override void OnRefreshWindow()
    {
        switch (_resType) {
                case ResourceType.MONEY:
                _txtResourceTitle.text = Str.Get("UI_CITY_BUILDING_MONEY");
                _txtDepositMaxrResource.text = UserManager.Instance.MaxMoneyStorage.ToString();
                break;
                case ResourceType.STONE:
                _txtResourceTitle.text = Str.Get("UI_CITY_BUILDING_STONE");
                _txtDepositMaxrResource.text = UserManager.Instance.MaxStoneStorage.ToString();
                break;
                case ResourceType.WOOD:
                _txtResourceTitle.text = Str.Get("UI_CITY_BUILDING_WOOD");
                _txtDepositMaxrResource.text = UserManager.Instance.MaxWoodStorage.ToString();
                break;
        }
        
        //TODO 将来计算将他修改成正确的值
        _txtHousesResourceNumber.text = CityManager.Instance.GetProduceByteRes(_resType).ToString();
        _txtBuilding.text = CityManager.Instance.GetBuildingNameByRes(_resType);
        _txtCityResource.text = WorldManager.Instance.GetProduceByteRes(_resType).ToString();
       
    }

}
