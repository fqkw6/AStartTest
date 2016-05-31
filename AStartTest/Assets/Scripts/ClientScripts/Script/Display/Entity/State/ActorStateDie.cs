using UnityEngine;
using System.Collections;

// 死亡状态
public class ActorStateDie : ActorStateBase
{
    public override void OnEnter(params object[] param)
    {
        // 停止移动 停止攻击
        // 播放死亡动作(根据实际情况决定是否击飞)
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float delta)
    {
    }
}
