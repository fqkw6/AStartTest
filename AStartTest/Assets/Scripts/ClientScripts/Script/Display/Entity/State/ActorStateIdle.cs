using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 静止站立
public class ActorStateIdle : ActorStateBase
{
    private int _lastScanTime = 0;
    public override void OnEnter(params object[] param)
    {
        Owner.IsPauseAnimation = false;
        _lastScanTime = 0;
    }

    public override void OnExit()
    {
    }

    public override void OnTick()
    {
        Owner.PlayAnimation(AnimationName.Idle);
        ScanTarget();
    }

    private void ScanTarget()
    {
        if (BattleTime.GetTime() - _lastScanTime > 1000) {
            _lastScanTime = BattleTime.GetTime();
            Actor target = Owner.ScanTarget();
            if (target != null) {
                if (Owner.IsBuilding()) {
                    Owner.PlayAttack(target);
                } else {
                    Owner.MoveToTarget(target);
                }
            }
        }
    }
}
