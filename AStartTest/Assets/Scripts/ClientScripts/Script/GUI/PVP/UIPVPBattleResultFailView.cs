using UnityEngine;
using System.Collections;

// pvp失败界面
public class UIPVPBattleResultFailView : UIWindow
{
    public const string Name = "PVP/UIPVPBattleResultFailView";
    public override void OnCloseWindow()
    {
        BattleManager.Instance.ChangeToMainScene(UIPVPView.Name);
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
