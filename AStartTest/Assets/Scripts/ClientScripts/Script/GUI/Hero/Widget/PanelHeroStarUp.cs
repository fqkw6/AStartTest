using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelHeroStarUp : MonoBehaviour
{
    public UIStarPanel _starPanel;
    public Image _imageStonePrg;
    public Text _txtStoneNumber;
    public Image _imageHeroIcon;
    public Image _imageHeroIconBg;
    public Text _txtHeroStoneNumber;
    public Button _btnUpgradeStar;
    private HeroInfo _currentInfo;
    public Image _imgStarAdd;
    
	// Use this for initialization
	void Start ()
    {
	
	}

    public void SetHeroInfo(HeroInfo info, bool fullRefresh = true)
    {
        if (info == null) return;

        _currentInfo = info;

        // 英雄魂石碎片
        int currentCount = UserManager.Instance.GetHeroPieceCount(_currentInfo.Cfg.Cost);
        int needCount = UserManager.Instance.GetHeroStarUpgradeCost(_currentInfo.ConfigID, _currentInfo.StarLevel);
        _imageStonePrg.fillAmount = 1.0f * currentCount / needCount;
        _txtStoneNumber.text = string.Format("{0}/{1}", currentCount, needCount);
        _txtHeroStoneNumber.text = string.Format("{0}/{1}", currentCount, needCount);

        _imageHeroIcon.sprite = ResourceManager.Instance.GetHeroIcon(_currentInfo.ConfigID);

        _starPanel.SetStar(_currentInfo.StarLevel);
        _btnUpgradeStar.interactable = UserManager.Instance.CheckHeroUpgradeStarItem(_currentInfo);

        if (currentCount >= needCount) {
            _imgStarAdd.gameObject.SetActive(false);
        } else {
            _imgStarAdd.gameObject.SetActive(true);
        }
    }

    public void OnClickStarUpgrade()
    {
        UserManager.Instance.RequestHeroUpgradeStar(_currentInfo.EntityID);
    }

    public void OnClickAddStar()
    {
        UIManager.Instance.OpenWindow<UIItemGetWayView>(_currentInfo.Cfg.Cost);
    }
}
