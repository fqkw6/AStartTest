using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 战斗结果界面
public class UIWorldBattleResultView : UIWindow
{
    public const string Name = "World/UIWorldBattleResultView";
    public Text _txtRank;
    public Text _txtMoney;
    public Text _txtWood;
    public Text _txtStone;

    public override void OnBindData(params object[] param)
    {
        WorldBattleResultInfo result = (WorldBattleResultInfo)param[0];
        _txtRank.text = "+" + result.Token;
        _txtMoney.text = result.Money.ToString();
        _txtWood.text = result.Wood.ToString();
        _txtStone.text = result.Stone.ToString();
    }

    public override void OnCloseWindow()
    {
        BattleManager.Instance.ChangeToMainScene();
    }
}
