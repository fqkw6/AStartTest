using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 玩家城池信息界面
public class UIWorldCityInfoView : UIWindow
{
    public const string Name = "World/UIWorldCityInfoView";
    public Text _textUserName;
    public Text _textuserLevel;
    public Text _textUserFightScore;
    public Image _imgUserIcon;
    public Text _textRewardMoney;
    public Text _textRewardWood;
    public Text _textRewardStone;
    public Text _textRewardGold;
    public Text _textRewardGoldText;
    public Image _hero1Bg;
    public Image _hero1Icon;
    public Image _hero2Bg;
    public Image _hero2Icon;
    public Image _hero3Bg;
    public Image _hero3Icon;

    public Button _btnDetect;
    public Button _btnAttack;
    public Button _btnSwitch;
    public Text _txtAttackCost;

    private WorldCityInfo _curInfo = null;
    
    public override void OnOpenWindow()
    {
    }

    public override void OnBindData(params object[] param)
    {
        _curInfo = param[0] as WorldCityInfo;
    }

    public override void OnRefreshWindow()
    {
        SetInfo(_curInfo);
    }

    private void SetInfo(WorldCityInfo info)
    {
        _curInfo = info;
        if (_curInfo == null) return;

        const string UNKNOWN = "????";
        _textUserName.text = _curInfo.UserName;
        _imgUserIcon.sprite = ResourceManager.Instance.GetPlayerIcon(_curInfo.UserIcon);

        _textuserLevel.text = _curInfo.UserLevel != 0 ? "Lv" + _curInfo.UserLevel : UNKNOWN;
        _textUserFightScore.text = _curInfo.UserFightScore != -1 ? _curInfo.UserFightScore.ToString() : UNKNOWN;

        _textRewardMoney.text = _curInfo.RewardMoney != -1 ?_curInfo.RewardMoney.ToString() : UNKNOWN;
        _textRewardWood.text = _curInfo.RewardWood != -1 ? _curInfo.RewardWood.ToString() : UNKNOWN;
        _textRewardStone.text = _curInfo.RewardStone != -1 ? _curInfo.RewardStone.ToString() : UNKNOWN;

        if (_curInfo.RewardGold > 0) {
            _textRewardGoldText.gameObject.SetActive(true);
            _textRewardGold.text = _curInfo.RewardGold.ToString();
        } else {
            _textRewardGoldText.gameObject.SetActive(false);
        }

        // TODO 根据品阶设置背景
        if (_curInfo.HeroInfoList.Count > 0) {
			_hero1Bg.gameObject.SetActive(true);
			_hero1Icon.gameObject.SetActive(true);
            _hero1Icon.sprite = ResourceManager.Instance.GetHeroIcon(_curInfo.HeroInfoList[0].heroCfgID);
        } else {
			_hero1Bg.gameObject.SetActive(false);
			_hero1Icon.gameObject.SetActive(false);
        }

        if (_curInfo.HeroInfoList.Count > 1) {
			_hero2Bg.gameObject.SetActive(true);
			_hero2Icon.gameObject.SetActive(true);
            _hero2Icon.sprite = ResourceManager.Instance.GetHeroIcon(_curInfo.HeroInfoList[1].heroCfgID);
        } else {
			_hero2Bg.gameObject.SetActive(false);
			_hero2Icon.gameObject.SetActive(false);
        }

        if (_curInfo.HeroInfoList.Count > 2) {
			_hero3Bg.gameObject.SetActive(true);
			_hero3Icon.gameObject.SetActive(true);
            _hero3Icon.sprite = ResourceManager.Instance.GetHeroIcon(_curInfo.HeroInfoList[2].heroCfgID);
        } else {
			_hero3Bg.gameObject.SetActive(false);
			_hero3Icon.gameObject.SetActive(false);
        }

        // 可以刷新
        if (!_curInfo.IsMyCity() && _curInfo.CouldRefresh()) {
            _btnSwitch.gameObject.SetActive(true);
        } else {
            _btnSwitch.gameObject.SetActive(false);
        }

        PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(UserManager.Instance.Level);
        _txtAttackCost.text = cfg.AttackCost.ToString();
    }

    // 侦查
    public void OnClickDetect()
    {
        WorldManager.Instance.RequestDetect(_curInfo.MapPosition);
    }

    // 攻击
    public void OnClickAttack()
    {
        PlayerLevelConfig cfg = PlayerLevelConfigLoader.GetConfig(UserManager.Instance.Level);
        WorldManager.Instance.RequestAttack(_curInfo.MapPosition, _curInfo.UserEntityID, _curInfo.IsNpc, cfg.AttackCost);        
    }

    // 更换对手
    public void OnClickSwitch()
    {
        WorldManager.Instance.RequestSwitch(_curInfo.MapPosition);
        CloseWindow();
    }
}

