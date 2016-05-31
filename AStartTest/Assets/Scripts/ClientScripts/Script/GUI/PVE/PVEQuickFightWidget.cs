using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// 扫荡10次的组件
public class PVEQuickFightWidget : MonoBehaviour
{
    public SimpleItemWidget[] _itemWidget;
    public Text _txtPlayerExp;
    public Text _txtMoney;
    public Text _txtTitle;

    public void SetInfo(BattleResultInfo result, int index)
    {
        for (int i = 0; i < _itemWidget.Length; ++i) {
            SimpleItemWidget itemWidget = _itemWidget[i];
            if (i < result.itemInfo.Count) {
                ItemInfo itemInfo = result.itemInfo[i];
                itemWidget.SetInfo(itemInfo.ConfigID, itemInfo.Number);
            } else {
                itemWidget.gameObject.SetActive(false);
            }
        }

        _txtPlayerExp.text = "x"+result.addPlayerExp.ToString();
        _txtMoney.text = result.addMoney.ToString();
        _txtTitle.text = string.Format(Str.Get("UI_PVE_QUICK_FIGHT_RESULT_TITLE"), index + 1);
    }

	// Use this for initialization
	void Start () {
	
	}
}
