using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// pvp战斗结果界面
public class UIPVPBattleResultView : UIWindow
{
    public const string Name = "PVP/UIPVPBattleResultView";
    public Text _txtRank;
    public Text _txtScore;

    public override void OnBindData(params object[] param)
    {
        PVPBattleResult result = (PVPBattleResult) param[0];
        _txtRank.text = "+" + result.AddRank;
        _txtScore.text = "+" + result.AddScore;
    }

    public override void OnCloseWindow()
    {
        BattleManager.Instance.ChangeToMainScene(UIPVPView.Name);
    }
}
