using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 静止站立
public class ActorStateDeploy : ActorStateBase
{
    public override void OnEnter(params object[] param)
    {
        Owner.SetCollisionEnable(true);
        Owner.AddTimer(Owner.Cfg.SetTime, ChangeToIdle);
        Owner.IsPauseAnimation = true;
    }

    private void ChangeToIdle()
    {
        // 创建完毕
        Owner.PlayAnimation(AnimationName.Place);
        Owner.SetAlpha(1);
        Owner.OnSpawn();
        Owner.Idle();
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float delta)
    {
    }
}
