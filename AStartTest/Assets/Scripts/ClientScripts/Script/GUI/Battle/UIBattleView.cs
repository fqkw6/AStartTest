using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Security.Principal;

public partial class EventID
{
    public const string UI_BATTLE_PREVIEW_CARD = "UI_BATTLE_PREVIEW_CARD";
    public const string UI_BATTLE_SELECT_CARD = "UI_BATTLE_SELECT_CARD";
    public const string UI_BATTLE_PUSH_NEW_CARD = "UI_BATTLE_PUSH_NEW_CARD";
    public const string UI_BATTLE_REFRESH_TIME = "UI_BATTLE_REFRESH_TIME";
    public const string UI_BATTLE_CANCEL_USE_CARD = "UI_BATTLE_CANCEL_USE_CARD";
}

// 战斗界面
public class UIBattleView : UIWindow
{
    public const string Name = "Battle/UIBattleView";

    public CardWidget[] _card;  // 当前4张卡牌
    public CardWidget _cardPreview; // 下一张卡牌
    public Text _txtMP; // 当前圣水数量
    public Image _imgMP;    // 圣水进度条
    public Text _txtOverTimeText;   // 加时赛文字
    public Text _txtTime; // 倒计时

    public override void OnOpenWindow()
    {
        EventDispatcher.AddEventListener<CardInfo>(EventID.UI_BATTLE_PREVIEW_CARD, OnPreviewCard);
        EventDispatcher.AddEventListener<CardInfo, float>(EventID.UI_BATTLE_PUSH_NEW_CARD, OnPushNewCard);
        EventDispatcher.AddEventListener<int>(EventID.UI_BATTLE_SELECT_CARD, OnSelectCard);
        EventDispatcher.AddEventListener<int>(EventID.UI_BATTLE_CANCEL_USE_CARD, OnCancelCard);
        EventDispatcher.AddEventListener(EventID.UI_BATTLE_REFRESH_TIME, OnRefreshTime);

        foreach (var item in _card) {
            item.gameObject.SetActive(false);
        }
        _cardPreview.gameObject.SetActive(false);

        _txtMP.text = "0";
        _imgMP.fillAmount = 0;
        _txtOverTimeText.gameObject.SetActive(false);
    }

    public override void OnCloseWindow()
    {
        EventDispatcher.RemoveEventListener<CardInfo>(EventID.UI_BATTLE_PREVIEW_CARD, OnPreviewCard);
        EventDispatcher.RemoveEventListener<CardInfo, float>(EventID.UI_BATTLE_PUSH_NEW_CARD, OnPushNewCard);
        EventDispatcher.RemoveEventListener<int>(EventID.UI_BATTLE_SELECT_CARD, OnSelectCard);
        EventDispatcher.RemoveEventListener<int>(EventID.UI_BATTLE_CANCEL_USE_CARD, OnCancelCard);
        EventDispatcher.RemoveEventListener(EventID.UI_BATTLE_REFRESH_TIME, OnRefreshTime);
    }

    // 刷新倒计时
    private void OnRefreshTime()
    {
        int time = 0;
        int timestamp = BattleTime.GetTimestamp();
        _txtOverTimeText.gameObject.SetActive(false);
        if (timestamp > GameConfig.GAME_TIME + GameConfig.OVER_TIME) {
            // 比赛结束
            time = GameConfig.GAME_TIME + GameConfig.OVER_TIME - timestamp;
        } else if (timestamp > GameConfig.GAME_TIME) {
            // 加时赛时间
            time = GameConfig.GAME_TIME + GameConfig.OVER_TIME - timestamp;
            _txtOverTimeText.gameObject.SetActive(true);
        } else if (timestamp > GameConfig.GAME_TIME - GameConfig.DOUBLE_TIME) {
            // 双倍圣水时间
            time = GameConfig.GAME_TIME - timestamp;
        } else {
            // 正常时间
            time = GameConfig.GAME_TIME - timestamp;
        }

        _txtTime.text = Utils.GetCountDownTime(time / 1000f);

        // _txtOverTimeText  如果是加时赛，则显示这个文本

        // 圣水量显示为0-10
        _txtMP.text = (BattleController.Instance.Mana/GameConfig.MANA_MUL).ToString();

        _imgMP.fillAmount = 1f*BattleController.Instance.Mana/GameConfig.MANA_MAX;
    }

    // 结束战斗
    public void OnClickWin()
    {
        BattleManager.Instance.OnBattleFinish(true);
    }

    // 脱离战斗
    public void OnClickLose()
    {
        BattleManager.Instance.OnBattleFinish(false);
    }

    // 强制离开战斗
    public void OnClickExit()
    {
        BattleManager.Instance.ChangeToMainScene();
    }

    // 添加新卡牌到手牌
    private void OnPushNewCard(CardInfo info, float delay)
    {
        int index = info.Index;
        if (index >= 0 && index < GameConfig.MAX_HANDCARD_COUNT) {
            _card[index].gameObject.SetActive(true);
            _card[index].SetInfo(index, info);
            _card[index].OnPushNewCard(_cardPreview.transform.position, delay);
        }
    }

    // 刷新预览卡牌
    private void OnPreviewCard(CardInfo info)
    {
        if (_cardPreview != null) {
            _cardPreview.gameObject.SetActive(true);
            _cardPreview.SetInfo(-1, info);
        }
    }

    // 选中卡牌
    private void OnSelectCard(int index)
    {
        foreach (var item in _card) {
            if (item.Info.Index == index) {
                // 选中这个卡牌
                BattleController.Instance.SelectedCard = item.Info;
                item.OnSelectCard();
            } else {
                // 取消其他卡牌的选中效果
                item.OnUnSelectCard();
            }
        }
    }

    // 强制取消卡牌的拖拽选中效果
    private void OnCancelCard(int index)
    {
        if (index >= 0 && index < GameConfig.MAX_HANDCARD_COUNT) {
            _card[index].MoveBack();
        }
    }

}
