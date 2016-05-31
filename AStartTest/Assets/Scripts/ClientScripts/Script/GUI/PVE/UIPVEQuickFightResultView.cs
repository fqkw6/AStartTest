using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// pve扫荡结果界面
public class UIPVEQuickFightResultView : UIWindow
{
    public const string Name = "PVE/UIPVEQuickFightResultView";
    public SimpleItemWidget[] _itemWidget;
    public SimpleItemWidget[] _itemWidgetExtra;

    public Text _txtPlayerExp;
    public Text _txtMoney;

    public override void OnOpenWindow()
    {
        if (PVEManager.Instance.QuickFightResult.Count <= 0) return;

        BattleResultInfo result = PVEManager.Instance.QuickFightResult[0];

        _txtMoney.text = result.addMoney.ToString();
        _txtPlayerExp.text = "x" + result.addPlayerExp.ToString();

        for (int i = 0; i < _itemWidget.Length; ++i) {
            SimpleItemWidget itemWidget = _itemWidget[i];
            if (i >= result.itemInfo.Count) {
                itemWidget.gameObject.SetActive(false);
            } else {
                ItemInfo itemInfo = result.itemInfo[i];
                itemWidget.SetInfo(itemInfo.ConfigID, itemInfo.Number);
            }
        }

        for (int i = 0; i < _itemWidgetExtra.Length; ++i) {
            SimpleItemWidget itemWidget = _itemWidgetExtra[i];
            itemWidget.gameObject.SetActive(false);
//            if (i >= result.itemInfo.Count) {
//                itemWidget.gameObject.SetActive(false);
//            } else {
//                ItemInfo itemInfo = result.itemInfo[i];
//                itemWidget.SetInfo(itemInfo.ConfigID, itemInfo.Number);
//            }
        }
    }
    
    // 再次扫荡
    public void OnClickQuickFight()
    {
        PVEManager.Instance.RequestQuickFight(PVEManager.Instance.CurrentSelectLevelID);
        CloseWindow();
    }
}
