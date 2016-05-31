using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 竞技场主界面
public class UIPVPView : UIWindow
{
    public const string Name = "PVP/UIPVPView";
    public Text _txtMyRank;
    public Text _txtFightScore;
    public SimpleHeroWidget[] _myHero;
    public Image[] _mySoldier;
    public Text _txtLeftTime;
    public Button _btnBuy;
    public PVPPlayerInfoWidget[] _players;

    public override void OnOpenWindow()
    {
        PVPManager.Instance.RequestPVPInfo();

        _btnBuy.gameObject.SetActive(false);
    }

    public override void OnRefreshWindow()
    {
        if (PVPManager.Instance.MyRank == 0) {
            // 未入榜
            _txtMyRank.text = Str.Get("UI_PVP_NOT_IN_RANK");
        } else {
            _txtMyRank.text = PVPManager.Instance.MyRank.ToString();
        }

        _btnBuy.gameObject.SetActive(PVPManager.Instance.AttackCount == 0);
        _txtFightScore.text = UserManager.Instance.GetFightScore().ToString();

        // 我的防守阵容
        for (int i = 0; i < _myHero.Length; ++i) {
            if (i < PVPManager.Instance.MyHeroList.Count) {
                _myHero[i].gameObject.SetActive(true);
                _myHero[i].SetInfo(PVPManager.Instance.MyHeroList[i]);
            } else {
                _myHero[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _mySoldier.Length; ++i) {
            if (i < PVPManager.Instance.MySoldierList.Count) {
                _mySoldier[i].gameObject.SetActive(true);
                _mySoldier[i].sprite = ResourceManager.Instance.GetSoldierIcon(PVPManager.Instance.MySoldierList[i].ConfigID);
            } else {
                _mySoldier[i].gameObject.SetActive(false);
            }
        }

        // 剩余挑战次数
        _txtLeftTime.text = string.Format("{0}/{1}", PVPManager.Instance.AttackCount, GameConfig.PVP_MAX_ATTACK_COUNT);

        // 对手
        for (int i = 0; i < _players.Length; ++i) {
            if (i < PVPManager.Instance.PlayerList.Count) {
                _players[i].gameObject.SetActive(true);
                _players[i].SetInfo(PVPManager.Instance.PlayerList[i]);
            } else {
                _players[i].gameObject.SetActive(false);
            }
        }
    }

    public override void OnCloseWindow()
    {
        UIManager.Instance.CloseWindow<UIPVPAwardView>();
        UIManager.Instance.CloseWindow<UIPVPBattleResultFailView>();
        UIManager.Instance.CloseWindow<UIPVPBattleResultView>();
        UIManager.Instance.CloseWindow<UIPVPPlayerInfoView>();
        UIManager.Instance.CloseWindow<UIPVPRankView>();
        UIManager.Instance.CloseWindow<UIPVPReportView>();
        UIManager.Instance.CloseWindow<UIPVPRuleView>();
        UIManager.Instance.CloseWindow<UIPVPScoreView>();
    }

    // 排行界面
    public void OnClickRank()
    {
        UIManager.Instance.OpenWindow<UIPVPRankView>();
    }

    // 商城界面
    public void OnClickShop()
    {
        UIManager.Instance.OpenWindow<UIShopView>(ShopType.PVP);
    }

    // 战报界面
    public void OnClickReport()
    {
        UIManager.Instance.OpenWindow<UIPVPReportView>();
    }

    // 最高排名奖励界面
    public void OnClickAward()
    {
        UIManager.Instance.OpenWindow<UIPVPAwardView>();
    }

    // 积分奖励界面
    public void OnClickScore()
    {
        UIManager.Instance.OpenWindow<UIPVPScoreView>();
    }

    // 规则说明
    public void OnClickRule()
    {
        UIManager.Instance.OpenWindow<UIPVPRuleView>();
    }

    // 调整防守阵容
    public void OnClickModify()
    {
        UIManager.Instance.OpenWindow<UIFormationView>();
    }

    // 换一批对手
    public void OnClickChange()
    {
        PVPManager.Instance.RequestChangePlayer();
    }

    // 补充攻击次数
    public void OnClickAddCount()
    {
        int count = GameConfig.PVP_MAX_ATTACK_COUNT - PVPManager.Instance.AttackCount;
        UIUtil.ShowConfirm(Str.Format("UI_PVP_BUY_ATTACK_COUNT_CONFIRM", count * GameConfig.PVP_GOLD_PER_ATTACK_COUNT), "", () =>
        {
            if (UserManager.Instance.Gold < count * GameConfig.PVP_GOLD_PER_ATTACK_COUNT) {
                UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_GOLD_LIMIT");
                return;
            }
            PVPManager.Instance.RequestBuyAttackChance(count);
        });
    }
}
