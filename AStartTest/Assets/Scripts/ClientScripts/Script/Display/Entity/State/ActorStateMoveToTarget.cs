using UnityEngine;
using System.Collections;

// 移动到目标单位
public class ActorStateMoveToTarget : ActorStateMove
{
    public override void OnEnter(params object[] param)
    {
        // 停止攻击
        // 朝目标移动
        base.OnEnter(param);
        // 停止攻击
        // 朝目标移动
        _currentMoveToPos = Owner.TargetPursuing.Position;
        SearchPath();
    }

    public override void OnRefresh(params object[] param)
    {
        base.OnRefresh(param);

        _currentMoveToPos = Owner.TargetPursuing.Position;
        SearchPath();
    }

    public override void OnTick()
    {
        base.OnTick();

        if (Owner.TargetPursuing == null || Owner.TargetPursuing.IsDead)
        {
            Owner.Idle();
        }
        else
        {
            if (Vector3.Distance(Owner.Position, Owner.TargetPursuing.Position) - Owner.GetCollisionRadius() - Owner.TargetPursuing.GetCollisionRadius() <= Owner.GetAtkRange())
            {
                Owner.PlayAttack(Owner.TargetPursuing);
            }
        }
    }
}
