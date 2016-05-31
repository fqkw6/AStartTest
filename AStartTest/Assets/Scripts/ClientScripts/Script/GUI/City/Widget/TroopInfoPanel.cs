using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 兵营的图标提示
public class TroopInfoPanel : MonoBehaviour {

    public Image _imageIcon;
    public Text _txtNumber;

    private TroopBuildingInfo _info;

    // Use this for initialization
    void Start () {
	
	}
    
    public void SetInfo(BuildingInfo info)
    {
        _info = info as TroopBuildingInfo;
        if (_info == null) return;

        if (_info.SoldierConfigID == 0 || _info.IsProducingSoldier() || _info.IsInBuilding()) {
            gameObject.SetActive(false);
        } else {
            _imageIcon.sprite = ResourceManager.Instance.GetSoldierTypeIcon(_info.SoldierConfigID);

            if (_info.SoldierConfigID != 0) {
                _txtNumber.text = string.Format("X {0}/{1}", _info.SoldierCount, _info.GetMaxSoldierCount(_info.SoldierConfigID));
            }
            gameObject.SetActive(true);
        }
    }
}
