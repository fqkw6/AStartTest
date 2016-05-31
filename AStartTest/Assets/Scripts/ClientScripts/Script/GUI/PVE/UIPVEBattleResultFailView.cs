using UnityEngine;
using System.Collections;

// 战斗失败界面
public class UIPVEBattleResultFailView : UIWindow
{
    public const string Name = "PVE/UIPVEBattleResultFailView";

    public override void OnCloseWindow()
    {
        BattleManager.Instance.ChangeToMainScene(UINewPVEEntranceView.Name);
    }

    public void OnClickData()
    {
        UIManager.Instance.OpenWindow<UIPVEBattleDataView>();
    }

    public void OnClickEnhance1()
    {
        BattleManager.Instance.ChangeToMainScene(UITavernView.Name);
    }

    public void OnClickEnhance2()
    {
        BattleManager.Instance.ChangeToMainScene(UINewHeroView.Name);
    }

    public void OnClickEnhance3()
    {
        BattleManager.Instance.ChangeToMainScene(UINewHeroView.Name);
    }
}
