using UnityEngine;
using System.Collections;

// 移动到目的地
public class ActorStateMoveToPosition : ActorStateMove
{
    public override void OnEnter(params object[] param)
    {
        base.OnEnter(param);
        // 停止攻击
        // 朝目标移动
        _currentMoveToPos = Owner.TargetPosition;
        SearchPath();
    }

    public override void OnRefresh(params object[] param)
    {
        base.OnRefresh(param);
        _currentMoveToPos = Owner.TargetPosition;
        SearchPath();
    }
}
