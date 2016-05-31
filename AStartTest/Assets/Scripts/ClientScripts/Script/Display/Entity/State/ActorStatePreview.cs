using UnityEngine;
using System.Collections;

public class ActorStatePreview : ActorStateBase
{
    public override void OnEnter(params object[] param)
    {
        Owner.SetCollisionEnable(false);
        Owner.SetAlpha(0.5f);
        Owner.IsPauseAnimation = true;
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float delta)
    {
    }
}
