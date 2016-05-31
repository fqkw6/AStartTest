using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PVESelectHeroWidget : ListItemWidget
{
    public Image _imgBg;
    public Image _imgIcon;
    public Image _imgCheck;
    public Text _txtLevel;

    public System.Action<int> OnClickItem;

    private HeroInfo _currentInfo;

    public override void SetInfo(object data)
    {
        _currentInfo = (HeroInfo)data;
        if (_currentInfo == null) return;

        _imgBg.sprite = ResourceManager.Instance.GetIconBgByQuality(_currentInfo.StarLevel);
        _imgIcon.sprite = ResourceManager.Instance.GetHeroIcon(_currentInfo.ConfigID);
        _imgCheck.gameObject.SetActive(_currentInfo.IsOnPVE());
        _txtLevel.text = "Lv " + _currentInfo.Level;
    }

    public bool IsWidget(int heroConfigID)
    {
        return _currentInfo != null && _currentInfo.ConfigID == heroConfigID;
    }

    public override void OnClick()
    {
        if (OnClickItem != null) {
            OnClickItem(_currentInfo.ConfigID);
        }
    }
}
