using UnityEngine;
using System.Collections;

// 主界面逻辑
public class GameStateNormal : FSMStateBase
{
    
    public override void OnEnter(params object[] param)
    {
        UIManager.Instance.OpenWindow<UINewMainView>();
        UIManager.Instance.OpenWindow<UISystemMsgView>();
    }
}
