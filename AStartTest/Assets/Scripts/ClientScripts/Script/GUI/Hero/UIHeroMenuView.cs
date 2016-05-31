using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 英雄界面的几个按钮的界面，因为要显示在模型上面，所以单独提取出来作为一个独立界面
public class UIHeroMenuView : UIWindow
{
    public const string Name = "Hero/UIHeroMenuView";

    public Toggle _toggleAdvance;
    public Toggle _toggleStarUp;
    public Toggle _ToggleExpUp;
    public Toggle _toggleSkill;

    public void OnClickAdvance()
    {
        if (_toggleAdvance.isOn) {
            EventDispatcher.TriggerEvent(EventID.EVENT_HERO_SELECT_EQUIP);
        } else {
            CheckShowDetail();
        }
    }

    public void OnClickStarUp()
    {
        if (_toggleStarUp.isOn) {
            EventDispatcher.TriggerEvent(EventID.EVENT_HERO_SELECT_STAR);
        } else {
            CheckShowDetail();
        }
    }

    public void OnClickExpBook()
    {
        if (_ToggleExpUp.isOn) {
            EventDispatcher.TriggerEvent(EventID.EVENT_HERO_SELECT_EXP);
        } else {
            CheckShowDetail();
        }
    }

    public void OnClickSkill()
    {
        if (_toggleSkill.isOn) {
            EventDispatcher.TriggerEvent(EventID.EVENT_HERO_SELECT_SKILL);
        } else {
            CheckShowDetail();
        }
    }

    // 检测所有toggle都点掉的情况
    private void CheckShowDetail()
    {
        if (_toggleSkill.isOn || _toggleStarUp.isOn || _toggleAdvance.isOn || _ToggleExpUp.isOn) {
            return;
        }

        EventDispatcher.TriggerEvent(EventID.EVENT_HERO_SELECT_DETAIL);
    }
}
