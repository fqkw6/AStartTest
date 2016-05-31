using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 士兵详细信息界面
public class UICitySoldierInfoView : UIWindow
{
    public const string Name = "City/UICitySoldierInfoView";
    public Text _title;
    public Text _txtLevel;
    public Text _hp;
    public Text _space;
    public Text _trainCost;
    public Text _trainTime;
    public Text _moveSpeed;
    public Text _attackRange;
    public Text _txtAttack;
    public Text _desc;
    public Image _imgSoldier;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        int soldierCfgID = (int) param[0];
        int level = (int) param[1];

        if (level <= 0) return;

        SoldierConfig cfg = SoldierConfigLoader.GetConfig(soldierCfgID);
        SoldierLevelConfig cfgLevel = SoldierLevelConfigLoader.GetConfig(soldierCfgID, level);

        _title.text = cfg.SoldierName;
        _txtLevel.text = Str.Format("UI_LEVEL", level);
        _hp.text = cfgLevel.SoldierHp.ToString();
        _space.text = cfg.States.ToString();
        _trainCost.text = cfgLevel.ProduceCost.ToString();
        _trainTime.text = Utils.GetCountDownString(Utils.GetSeconds(cfg.Producetime));
        _imgSoldier.sprite = ResourceManager.Instance.GetSoldierImage(soldierCfgID);
        _txtAttack.text = cfg.Attack.ToString();
    }
}
