using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserInfoPanel : MonoBehaviour
{
    public Image _imageBg;
    public Image _imageIcon;
    public Text _userLevel;
    public Image _imageRefreshFlag;

    public Sprite _blueBg;
    public Sprite _redBg;

    private WorldCityInfo _curInfo;

	void Start () {
	
	}

    public void SetInfo(WorldCityInfo info)
    {
        _curInfo = info;

        if (_curInfo == null) {
            // 玩家主城
        } else {
            _imageRefreshFlag.gameObject.SetActive(_curInfo.CouldRefresh());
            _imageIcon.sprite = ResourceManager.Instance.GetPlayerIcon(_curInfo.UserIcon);

            _userLevel.text = _curInfo.UserLevel != 0 ? _curInfo.UserLevel.ToString() : "?";

            if (_curInfo.IsMyCity()) {
                _imageBg.sprite = _blueBg;
            } else {
                _imageBg.sprite = _redBg;
            }
        }
    }

    public void OnClick()
    {
        
    }
}
