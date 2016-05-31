using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 资源城信息界面
public class UIWorldResTownInfoView : UIWindow
{
    public const string Name = "World/UIWorldResTownInfoView";
    public Image _imageResType;
    public Text _textUserName;
    public Text _textUserLevelText;
    public Text _textUserFightScoreText;
    public Text _textuserLevel;
    public Text _textUserFightScore;
    public Image _imgUserIcon;
    public Text _textRewardText;
    public Image _imgReward;
    public Text _textRewardValue;
    public Text _textTotalText;
    public Image _imgTotal;
    public Text _textTotalValue;
    public Text _textTimeText;
    public Text _textTime;
    public Image _heroBg1;
    public Image _heroIcon1;
    public Image _heroBg2;
    public Image _heroIcon2;
    public Image _heroBg3;
    public Image _heroIcon3;

    public Button _btnDetect;
    public Button _btnDefendBuild;
    public Button _btnAttack;
    public Button _btnSwitch;

    private WorldResTownInfo _curInfo;

    public override void OnBindData(params object[] param)
    {
        SetInfo(param[0] as WorldResTownInfo);
    }

    public override void OnRefreshWindow()
    {
        
    }

    private void SetInfo(WorldResTownInfo info)
    {
        _curInfo = info;
        if (_curInfo == null) return;

		_imageResType.gameObject.SetActive(false);

        if (_curInfo.UserEntityID == 0) {
            // 无人资源岛
            WorldMapConfig cfg = WorldMapConfigLoader.GetConfig(_curInfo.MapConfigID);
            if (cfg != null) {
                _textUserName.text = cfg.CityName;
                _imgUserIcon.sprite = ResourceManager.Instance.GetResIcon(_curInfo.ProduceType);
                _textuserLevel.text = "Lv" + _curInfo.MapLevel;
                _textUserFightScore.text = "0";
            }
        } else {
            // 玩家资源岛
            _textUserLevelText.gameObject.SetActive(true);
            _textUserFightScoreText.gameObject.SetActive(true);

            _textUserName.text = _curInfo.UserName;
            _textuserLevel.text = "Lv" + _curInfo.UserLevel;
            _textUserFightScore.text = _curInfo.UserFightScore.ToString();
            _imgUserIcon.sprite = ResourceManager.Instance.GetPlayerIcon(_curInfo.UserIcon);
        }


        switch (_curInfo.ProduceType) {
            case ResourceType.MONEY:
                _textRewardText.text = Str.Get("UI_WORLD_RES_PRODUCE_MONEY");
                break;
            case ResourceType.WOOD:
                _textRewardText.text = Str.Get("UI_WORLD_RES_PRODUCE_WOOD");
                break;
            case ResourceType.STONE:
                _textRewardText.text = Str.Get("UI_WORLD_RES_PRODUCE_STONE");
                break;
            case ResourceType.GOLD:
                _textRewardText.text = Str.Get("UI_WORLD_RES_PRODUCE_GOLD");
                break;
        }
        _imgReward.sprite = ResourceManager.Instance.GetResIcon(_curInfo.ProduceType);
        _textRewardValue.text = _curInfo.ProduceValue.ToString();

        // 我方资源城还显示总产值和剩余时间
        if (_curInfo.IsMyCity()) {
            _textTotalText.gameObject.SetActive(true);
            _textTimeText.gameObject.SetActive(true);

            _imgTotal.sprite = ResourceManager.Instance.GetResIcon(_curInfo.ProduceType);
            _textTotalValue.text = _curInfo.GetTotalProduceValue().ToString();
            _textTime.text = Utils.GetCountDownString(_curInfo.GetConquerCD());
        } else {
            _textTotalText.gameObject.SetActive(false);
            _textTimeText.gameObject.SetActive(false);
        }

        // TODO 根据品阶设置背景
        if (_curInfo.HeroInfoList.Count > 0) {
            _heroBg1.gameObject.SetActive(true);
			_heroIcon1.gameObject.SetActive(true);
            _heroIcon1.sprite = ResourceManager.Instance.GetHeroIcon(_curInfo.HeroInfoList[0].heroCfgID);
        } else {
			_heroBg1.gameObject.SetActive(false);
			_heroIcon1.gameObject.SetActive(false);
        }

        if (_curInfo.HeroInfoList.Count > 1) {
			_heroBg2.gameObject.SetActive(true);
			_heroIcon2.gameObject.SetActive(true);
            _heroIcon2.sprite = ResourceManager.Instance.GetHeroIcon(_curInfo.HeroInfoList[1].heroCfgID);
        } else {
			_heroBg2.gameObject.SetActive(false);
			_heroIcon2.gameObject.SetActive(false);
        }

        if (_curInfo.HeroInfoList.Count > 2) {
			_heroBg3.gameObject.SetActive(true);
			_heroIcon3.gameObject.SetActive(true);
            _heroIcon3.sprite = ResourceManager.Instance.GetHeroIcon(_curInfo.HeroInfoList[2].heroCfgID);
        } else {
			_heroBg3.gameObject.SetActive(false);
			_heroIcon3.gameObject.SetActive(false);
        }
        
        if (_curInfo.IsMyCity()) {
            _btnDefendBuild.gameObject.SetActive(true);
            _btnDetect.gameObject.SetActive(false);
            _btnAttack.gameObject.SetActive(false);
            _btnSwitch.gameObject.SetActive(false);
        } else {
            _btnDefendBuild.gameObject.SetActive(false);
            _btnDetect.gameObject.SetActive(true);
            _btnAttack.gameObject.SetActive(true);

            if (_curInfo.CouldRefresh()) {
                _btnSwitch.gameObject.SetActive(true);
            } else {
                _btnSwitch.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickDetect()
    {
        WorldManager.Instance.RequestDetect(_curInfo.MapPosition);
    }

    public void OnClickDefendBuild()
    {
        // TODO  城防布置
    }

    // 攻打资源城
    public void OnClickAttack()
    {
        WorldManager.Instance.RequestAttack(_curInfo.MapPosition, _curInfo.UserEntityID, _curInfo.IsNpc, 0);
        CloseWindow();
    }

    public void OnClickSwitch()
    {
        if (_curInfo.IsMyCity() || !_curInfo.CouldRefresh()) {
            // 不可刷新
            UIUtil.ShowErrMsgFormat("MSG_WORLD_COULD_NOT_REFRESH");
            return;
        }
        WorldManager.Instance.RequestSwitch(_curInfo.MapPosition);
        CloseWindow();
    }
}

