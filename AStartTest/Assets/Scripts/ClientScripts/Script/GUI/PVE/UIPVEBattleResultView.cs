using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 战斗结果展示界面
public class UIPVEBattleResultView : UIWindow
{
    public const string Name = "PVE/UIPVEBattleResultView";
    public Image[] _imgStar;

    public Sprite _grayStar;

    public Text _txtLevel;
    public Text _txtExp;
    public Text _txtMoney;

    public BattleResultHeroWidget[] _heros;

    public SimpleItemWidget[] _items;

    public override void OnOpenWindow()
    {
        BattleResultInfo data = PVEManager.Instance.BattleResult;

        for (int i = 0; i < 3; ++i) {
            if (i >= data.star) {
                _imgStar[i].sprite = _grayStar;
            }
        }

        if (_txtLevel != null) _txtLevel.text = "Lv " + UserManager.Instance.Level;
        _txtExp.text = "x" + data.addPlayerExp;
        _txtMoney.text = data.addMoney.ToString();

        for (int i = 0; i < _heros.Length; ++i) {
            if (i < data.heroInfo.Count) {
                BattleResultHeroInfo heroInfo = data.heroInfo[i];
                _heros[i].SetInfo(heroInfo.heroID, heroInfo.addExp);
            } else {
                _heros[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _items.Length; ++i) {
            if (i < data.itemInfo.Count) {
                ItemInfo itemInfo = data.itemInfo[i];
                _items[i].SetInfo(itemInfo.ConfigID, itemInfo.Number);
            } else {
                _items[i].gameObject.SetActive(false);
            }
        }

        UpdateClientData();
    }

    public override void OnCloseWindow()
    {
        BattleManager.Instance.ChangeToMainScene(UINewPVEEntranceView.Name);
    }

    // 更新客户端本地数据
    private void UpdateClientData()
    {
        UserManager.Instance.OnAddUserExp(PVEManager.Instance.BattleResult.addPlayerExp);
        UserManager.Instance.AddMoney(PVEManager.Instance.BattleResult.addMoney, PriceType.MONEY);

        foreach (var item in PVEManager.Instance.BattleResult.heroInfo) {
            HeroInfo heroInfo = UserManager.Instance.GetHeroInfo(item.heroID);
            if (heroInfo != null) heroInfo.OnAddExp(item.addExp);
        }
        
        UserManager.Instance.AddItem(PVEManager.Instance.BattleResult.itemInfo, true);
    }

    public void OnClickData()
    {
        UIManager.Instance.OpenWindow<UIPVEBattleDataView>();
    }
}

