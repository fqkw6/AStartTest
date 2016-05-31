using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class LevelInfoHeroWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image _heroBg;
    public Image _heroIcon;
    public Text _heroLevel;

    private int _heroID;
    private int _quality;
    private int _star;
    private int _level;

	void Start () {
	
	}

    public void SetInfo(int heroID, int quality, int level, int star)
    {
        _heroID = heroID;
        _level = level;
        _star = star;
        _quality = quality;

        _heroBg.sprite = ResourceManager.Instance.GetIconBgByQuality(quality);
        _heroIcon.sprite = ResourceManager.Instance.GetHeroIcon(heroID);
        _heroLevel.text = "Lv " + level;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.OpenWindow<UIPVEHeroInfoTipView>(_heroID, _level, _star, _quality);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIManager.Instance.CloseWindow<UIPVEHeroInfoTipView>();
    }
}
