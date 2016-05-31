using UnityEngine;
using System.Collections;

// 登录逻辑
public class GameStateLogin : FSMStateBase
{
    public override void OnEnter(params object[] param)
    {
        Game.Instance.ShowMainCity(false);  // 隐藏主城

        // 信息显示界面
        UIManager.Instance.OpenWindow<UISystemMsgView>();
        UIManager.Instance.OpenWindow<UIAccountLoginView>();
    }

    public override void OnExit()
    {
    }
}
