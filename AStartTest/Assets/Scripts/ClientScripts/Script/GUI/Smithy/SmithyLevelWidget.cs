using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 锻造铺的装备等级限制
public class SmithyLevelWidget : ListItemWidget
{
    public Text _txtLevel;
    public Image _imgLock;
    public Button _btnLevel;

    public int EquipID = 0;

	void Start () {
	
	}

    public void SetInfo(EquipmentConfig cfg, bool isLock)
    {
        EquipID = cfg.CfgId;

        _txtLevel.text = Str.Format("UI_LEVEL", cfg.EquipLevel) + " " + cfg.Name;

        _imgLock.gameObject.SetActive(isLock);
        _btnLevel.interactable = !isLock;
    }

    public void SetInfo(BingfaConfig cfg, bool isLock)
    {
        EquipID = cfg.CfgId;

        _txtLevel.text = Str.Format("UI_LEVEL", cfg.EquipLevel) + " " + cfg.Name;

        _imgLock.gameObject.SetActive(isLock);
        _btnLevel.interactable = !isLock;
    }
}
