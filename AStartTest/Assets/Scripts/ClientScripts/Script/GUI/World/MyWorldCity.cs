using UnityEngine;
using System.Collections;

public class MyWorldCity : MonoBehaviour {
    public MyUserInfoPanel _infoPanel;

	void Start ()
	{
	    Refresh();
	}

    public void OnClick()
    {
        // 我的主城
        UIManager.Instance.OpenWindow<UIWorldMyCityInfoView>();
    }

    public void Refresh()
    {
        if (_infoPanel != null) {
            _infoPanel.Refresh();
        }
    }
}
