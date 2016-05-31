using UnityEngine;
using System.Collections;

// 失败界面
public class UIWorldBattleResultFailView : UIWindow
{
    public const string Name = "World/UIWorldBattleResultFailView";
    public override void OnCloseWindow()
    {
        BattleManager.Instance.ChangeToMainScene();
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
