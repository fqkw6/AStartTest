using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 通用的英雄头像组件
public class SimpleHeroWidget : MonoBehaviour
{
    public Image _imgHeroBg;
    public Image _imgHeroIcon;
    public Text _txtHeroLevel;

    public UIStarPanel _starPanel;
    public Text _txtHeroName;

	void Start () {
	
	}

    public void SetInfo(HeroInfo info)
    {
        
    }
}
