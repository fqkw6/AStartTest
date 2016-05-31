using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

public class UINewPVELevelInfoView : UIWindow
{
    public const string Name = "PVE/UINewPVELevelInfoView";

    public Text _txtPVETitle;
    public Text _txtPVEInfo;
    public Text _txtRecommendForce;
    public Text _txtCurrentForce;
    public Text _txtLossSp;
    public Text _txtDekaronGameObject;
    public Text _txtDekaronNumber;
    public Image _imageConditionStar1;
    public Text _txtCondition1;
    public Text _txtCondition1OK1;
    public Image _imageConditionStar2;
    public Text _txtCondition2;
    public Text _txtCondition1OK2;
    public Image _imageConditionStar3;
    public Text _txtCondition3;
    public Text _txtCondition1OK3;
    public Image _imageCommonPVE;
    public Image _imageSeniorPVE;
    public Image _imageSweepBg;
    public Button _btnQuickFight;
    public Button _btnQuickFight10;
    public Text _txtQuickFight10;
    public LevelInfoHeroWidget[] _myHeroWidget;
    public LevelInfoHeroWidget[] _enemyHeroWidget;
    public SimpleItemWidget[] _itemWidget;
    public Sprite _sprStarEmpty;

    private int _levelID;

    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        int levelID = (int)param[0];
        _levelID = levelID;
    }

    private void UpdateCondition(LevelInfo info)
    {
        _txtCondition1OK1.gameObject.SetActive(true);
        _txtCondition1OK2.gameObject.SetActive(true);
        _txtCondition1OK3.gameObject.SetActive(true);

        if (info == null || info.star == 0) {
            _imageConditionStar1.sprite = _sprStarEmpty;
            _imageConditionStar2.sprite = _sprStarEmpty;
            _imageConditionStar3.sprite = _sprStarEmpty;

            _txtCondition1OK1.gameObject.SetActive(false);
            _txtCondition1OK2.gameObject.SetActive(false);
            _txtCondition1OK3.gameObject.SetActive(false);
        } else if (info.star == 1) {
            _imageConditionStar2.sprite = _sprStarEmpty;
            _imageConditionStar3.sprite = _sprStarEmpty;
            _txtCondition1OK2.gameObject.SetActive(false);
            _txtCondition1OK3.gameObject.SetActive(false);
        } else if (info.star == 2) {
            _imageConditionStar3.sprite = _sprStarEmpty;
            _txtCondition1OK3.gameObject.SetActive(false);
        }
    }

    public override void OnRefreshWindow()
    {
        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(_levelID);
        if (cfg == null) return;

        _txtLossSp.text = cfg.StaminaCost.ToString();
        _txtPVEInfo.text = cfg.MissionDescription;

        _imageCommonPVE.gameObject.SetActive(cfg.MissionDegree == (int)ChapterType.NORMAL);
        _imageSeniorPVE.gameObject.SetActive(cfg.MissionDegree == (int)ChapterType.ELITE);

        List<AwardInfo> awardList = AwardManager.Instance.GetAwardList(cfg.CompleteAward, true);
        for (int i = 0; i < 4; ++i) {
            SimpleItemWidget widget = _itemWidget[i];
            if (i < awardList.Count) {
                widget.SetInfo(awardList[i].ItemID, 1);
            } else {
                widget.gameObject.SetActive(false);
            }
        }

        LevelInfo info = PVEManager.Instance.GetLevelInfo(_levelID);
        if (info == null || info.star < 3) {
            _btnQuickFight.gameObject.SetActive(false);
            _btnQuickFight10.gameObject.SetActive(false);
            _txtDekaronNumber.text = string.Format("{0}/{1}", cfg.TimesLimit, cfg.TimesLimit);
        } else {
            _txtDekaronNumber.text = string.Format("{0}/{1}", cfg.TimesLimit - info.fightCount, cfg.TimesLimit);
        }

        if (cfg.TimesLimit <= 0) {
            _txtDekaronGameObject.gameObject.SetActive(false);
        } else {
            _txtDekaronGameObject.gameObject.SetActive(true);
        }

        // 上阵英雄
        for (int i = 0; i < _myHeroWidget.Length; ++i) {
            var widget = _myHeroWidget[i];
            if (i < UserManager.Instance.PVEHeroList.Count) {
                HeroInfo heroInfo = UserManager.Instance.PVEHeroList[i];
                widget.gameObject.SetActive(true);
                widget.SetInfo(heroInfo.ConfigID, heroInfo.StarLevel, heroInfo.Level, heroInfo.StarLevel);
            } else {
                widget.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _enemyHeroWidget.Length; ++i) {
            var widget = _enemyHeroWidget[i];

        }
        foreach (var item in UserManager.Instance.HeroList) {
            
        }

        UpdateCondition(info);

        _txtQuickFight10.text = Str.Format("UI_PVE_QUICK_FIGHT_COUNT", PVEManager.Instance.GetQuickFightCount(_levelID));
    }

    public override void OnCloseWindow()
    {
        UIManager.Instance.CloseWindow<UIPVEHeroInfoTipView>();
    }

    public void OnClickFight()
    {
        if (!PVEManager.Instance.HasFightCount(_levelID)) {
            UIUtil.ShowConfirm(Str.Format("UI_PVE_RESET_FIGHT_COUNT", GameConfig.PVE_RESET_FIGHT_COUNT_COST), "", () =>
            {
                PVEManager.Instance.RequestResetFightCount(_levelID);
            });
            return;
        }
        PVEManager.Instance.CurrentSelectLevelID = _levelID;
        PVEManager.Instance.RequestFight(PVEManager.Instance.CurrentSelectLevelID);
        CloseWindow();
    }

    public void OnClickQuickFight()
    {
        if (!PVEManager.Instance.HasFightCount(_levelID)) {
            UIUtil.ShowConfirm(Str.Format("UI_PVE_RESET_FIGHT_COUNT", GameConfig.PVE_RESET_FIGHT_COUNT_COST), "", () => {
                PVEManager.Instance.RequestResetFightCount(_levelID);
            });
            return;
        }
        PVEManager.Instance.CurrentSelectLevelID = _levelID;

        PVEManager.Instance.RequestQuickFight(PVEManager.Instance.CurrentSelectLevelID);
        CloseWindow();
    }

    public void OnClickQuickFight10()
    {
        if (!PVEManager.Instance.HasFightCount(_levelID)) {
            UIUtil.ShowConfirm(Str.Format("UI_PVE_RESET_FIGHT_COUNT", GameConfig.PVE_RESET_FIGHT_COUNT_COST), "", () => {
                PVEManager.Instance.RequestResetFightCount(_levelID);
            });
            return;
        }

        PVEManager.Instance.CurrentSelectLevelID = _levelID;

        MissionConstConfig cfg = MissionConstConfigLoader.GetConfig(_levelID);
        if (cfg == null) return;

        // 体力不足
        if (UserManager.Instance.SP < cfg.StaminaCost) {
            UIUtil.ShowMsgFormat("UI_NOT_ENOUGTH_SP");
            return;
        }

        // 取最大扫荡次数
        // 受体力限制
        int count = Mathf.Min(GameConfig.PVE_MAX_QUICK_FIGHT_COUNT, UserManager.Instance.SP / cfg.StaminaCost);
        LevelInfo levelInfo = PVEManager.Instance.GetLevelInfo(_levelID);
        if (levelInfo != null) {
            // 挑战次数不足
            if (cfg.TimesLimit > 0) {
                if (levelInfo.fightCount >= cfg.TimesLimit) {
                    UIUtil.ShowMsgFormat("UI_PVE_FIGHT_COUNT_LIMIT");
                    return;
                }

                // 还有次数
                count = Mathf.Min(count, cfg.TimesLimit - levelInfo.fightCount);
            }
        } else {
            if (cfg.TimesLimit > 0) {
                count = Mathf.Min(count, cfg.TimesLimit);
            }
        }
        PVEManager.Instance.RequestQuickFight(PVEManager.Instance.CurrentSelectLevelID, count);
        CloseWindow();
    }

    // 打开布阵界面
    public void OnClickFormation()
    {
        UIManager.Instance.OpenWindow<UIPVEFormationView>();
    }
}
