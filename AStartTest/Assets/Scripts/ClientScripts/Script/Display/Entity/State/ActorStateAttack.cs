using UnityEngine;
using System.Collections;

// 普通攻击状态
public class ActorStateAttack : ActorStateBase
{
    private int _startAttackTime = 0;
    private bool _applyHurt = false;
    private bool _hasAttackFinish = false;  // 当前攻击动作播放完毕，但是出于攻击间隔，尚未开始下一个攻击
    protected Transform _transform;

    public override void OnEnter(params object[] param)
    {
        // 调整逻辑位置
        _transform = Owner.transform;
        Owner.Position = Owner.transform.position;
        PlayAttack();
    }

    public override void OnExit()
    {
        // TODO:这里仅迁就王子冲锋首次攻击会有伤害加成，以后需要修改更加通用框架
        Owner.CallFunction("OnExitAttack");
    }

    private void PlayAttack()
    {
        if (Owner.TargetAttacking == null || Owner.TargetAttacking.IsDead)
        {
            Owner.Idle();
            return;
        }

        _applyHurt = false;
        _hasAttackFinish = false;
        _startAttackTime = BattleTime.GetTime();

        // 攻击时转向对手
        if (Owner.Cfg.TurnToTarget > 0)
        {
            Owner.TurnToTarget(Owner.TargetAttacking.Position);
        }

        // 播放攻击动作
        if (!string.IsNullOrEmpty(AnimationName.Attack))
        {
            // 播放攻击动画
            Owner.PlayAnimation(AnimationName.Attack);
            // TODO:播放攻击特效
            Owner.CallFunction("OnAttackPreview");
        }
    }

    // 逻辑帧更新(100ms)
    public override void OnTick()
    {
        if (!_applyHurt)
        {
            // 攻击前摇
            if (BattleTime.GetTime() - _startAttackTime >= Owner.GetAtkHitpoint())
            {
                _applyHurt = true;
                if (Owner.TargetAttacking != null)
                {
                    // 前摇结束，造成伤害
                    Owner.TargetAttacking.OnHit(Owner.GetDamage());
                    // TODO:这里仅迁就王子冲锋首次攻击会有伤害加成，以后需要修改更加通用框架
                    Owner.CallFunction("OnAfterAttack");
                }
            }
        }
        else
        {
            if (BattleTime.GetTime() - _startAttackTime >= Owner.GetAtkInterval())
            {
                PlayAttack();
            }
            //下一轮攻击时间未到
            else
            {
                //if (!_hasAttackFinish)
                //{
                //    // 如果当前动作播放完毕，则播放待机动作
                //    if (Owner.IsCurrentAnimationFinish())
                //    {
                //        _hasAttackFinish = true;
                //        Owner.PlayAnimation(AnimationName.Idle);
                //    }
                //}
            }
        }
    }

    // 渲染帧更新
    public override void OnUpdate(float delta)
    {
        _transform.position = Vector3.Slerp(_transform.position, Owner.Position, delta);
    }
}