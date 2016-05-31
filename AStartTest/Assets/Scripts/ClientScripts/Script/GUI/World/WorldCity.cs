using System;
using UnityEngine;
using System.Collections;

// 城池
public class WorldCity : MonoBehaviour
{
    [NonSerialized] public int MapPosition = 0;
    private WorldCityInfo _curInfo;
    private UserInfoPanel _userInfoPanel;

	void Start () {
	
	}

    public void CreateUserPanel(UserInfoPanel prefab)
    {
        _userInfoPanel = Instantiate(prefab);
        _userInfoPanel.transform.SetParent(transform, false);
        _userInfoPanel.gameObject.SetActive(true);
        _userInfoPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
	
    public void SetInfo(WorldCityInfo info)
    {
        _curInfo = info;
        
        if (_userInfoPanel != null) {
            _userInfoPanel.SetInfo(_curInfo);
        }

        if (_curInfo == null) return;
        
        MapPosition = _curInfo.MapPosition;
    }

    public void OnClick()
    {
        if (_curInfo == null) {
            // 我的主城
            UIManager.Instance.OpenWindow<UIWorldMyCityInfoView>();
            return;
        }

        if (_curInfo is WorldResTownInfo) {
            // 资源城池
            UIManager.Instance.OpenWindow<UIWorldResTownInfoView>(_curInfo);
        } else {
            // 其他玩家的主城
            UIManager.Instance.OpenWindow<UIWorldCityInfoView>(_curInfo);
        }
    }

    public void Refresh()
    {
        SetInfo(_curInfo);
    }
}
