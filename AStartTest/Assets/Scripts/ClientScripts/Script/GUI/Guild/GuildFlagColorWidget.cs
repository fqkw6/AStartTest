using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 公会旗帜颜色选择
public class GuildFlagColorWidget : MonoBehaviour
{
    private Image _image;

    [NonSerialized] public int Index;
    public System.Action<int> OnClickCallback;
    
	void Start ()
	{
	    _image = GetComponent<Image>();
	}

    public void SetColor(Color color)
    {
        _image.color = color;
    }

    public void OnClick()
    {
        if (OnClickCallback != null) {
            OnClickCallback(Index);
        }
    }
}
