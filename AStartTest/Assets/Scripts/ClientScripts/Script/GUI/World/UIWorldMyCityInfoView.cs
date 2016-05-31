using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 玩家自己的城池信息
public class UIWorldMyCityInfoView : UIWindow
{
    public const string Name = "World/UIWorldMyCityInfoView";
    public Text _textUserName;
    public Text _textuserLevel;
    public Text _textUserFightScore;
    public Image _imgUserIcon;
    public Text _textProduceMoney;
    public Text _textProduceAddMoney;
    public Text _textProduceWood;
    public Text _textProduceAddWood;
    public Text _textProduceStone;
    public Text _textProduceAddStone;
    public Image _hero1Bg;
    public Image _hero1BgCover;
    public Image _hero1Icon;
    public Image _hero2Bg;
    public Image _hero2BgCover;
    public Image _hero2Icon;
    public Image _hero3Bg;
    public Image _hero3BgCover;
    public Image _hero3Icon;

    public override void OnRefreshWindow()
    {
        _textUserName.text = UserManager.Instance.RoleName;
        _textuserLevel.text = "Lv" + UserManager.Instance.Level;
        
        _textUserFightScore.text = UserManager.Instance.GetFightScore().ToString();
        _imgUserIcon.sprite = ResourceManager.Instance.GetPlayerIcon(UserManager.Instance.Icon);
        _textProduceMoney.text = CityManager.Instance.GetTotalProduce(ResourceType.MONEY).ToString();
        _textProduceWood.text = CityManager.Instance.GetTotalProduce(ResourceType.WOOD).ToString();
        _textProduceStone.text = CityManager.Instance.GetTotalProduce(ResourceType.STONE).ToString();

        _textProduceAddMoney.text = string.Format("(+{0})", WorldManager.Instance.GetTotalProduce(ResourceType.MONEY));
        _textProduceAddWood.text = string.Format("(+{0})", WorldManager.Instance.GetTotalProduce(ResourceType.WOOD));
        _textProduceAddStone.text = string.Format("(+{0})", WorldManager.Instance.GetTotalProduce(ResourceType.STONE));

        // TODO 当前驻守的英雄
    }

    // 进入主城
    public void OnClickEnter()
    {
        UIManager.Instance.CloseAllWindow();
        UIManager.Instance.CloseWindow<UIWorldMapView>();
        UIManager.Instance.OpenWindow<UINewMainView>();
        CloseWindow();
    }

    // 城防建设
    public void OnClickDefendBuild()
    {
        // TODO 城防建设相关   
    }
}
