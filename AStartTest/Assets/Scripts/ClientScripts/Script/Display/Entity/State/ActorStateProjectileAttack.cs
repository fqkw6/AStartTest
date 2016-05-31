using UnityEngine;
using System.Collections;

// 普通攻击的抛射物攻击
public class ActorStateProjectileAttack : ActorStateBase
{
    private int _startAttackTime = 0;
    private bool _hasAttack = false;
    private bool _hasAttackFinish = false;  // 当前攻击动作播放完毕，但是出于攻击间隔，尚未开始下一个攻击
    private bool _nextAttackAnimation = false;  // 如果攻击间隔过长，会出现播放多次攻击动画的现象
    protected Transform _transform;

    public override void OnEnter(params object[] param)
    {
        _transform = Owner.transform;
        //Owner.Position = Owner.transform.position;
        PlayAttack();
    }

    public override void OnExit()
    {
    }

    // 普通攻击
    private void PlayAttack()
    {
        if (Owner.TargetAttacking == null || Owner.TargetAttacking.IsDead)
        {
            Owner.Idle();
            return;
        }

        _hasAttack = false;
        _hasAttackFinish = false;
        _startAttackTime = BattleTime.GetTime();

        if (Owner.Cfg.TurnToTarget > 0)
        {
            // 攻击时转向目标
            Owner.TurnToTarget(Owner.TargetAttacking.Position);
        }

        if (!string.IsNullOrEmpty(Owner.AtkAnimation))
        {
            Owner.PlayAnimation(Owner.AtkAnimation);
            // TODO:播放攻击特效
            Owner.CallFunction("OnAttackPreview");
        }
    }

    // 发射抛射物
    private void LaunchProjectile()
    {
        // 抛物线攻击
        Projectile proj = BattleController.Instance.CreateProjectile(Owner.name + "_Proj");

        // 设置模型和运动参数
        proj.transform.position = Owner.ProjectileSpawnPoint.position;
        // 设置回调者
        proj.Owner = Owner;
        // 注意：这里是半径
        proj.Radius = Mathf.FloorToInt(Owner.Cfg.DamageRadius / 1000f * MapGrid.CellSize);
        proj.MultipleProjectiles = Owner.MultipleProjectiles;  // 抛射物数量
        proj.ProjectilesFormation = Owner.ProjectilesFormation;  // 抛射物队形
        // 设置目标(普通单位的攻击都是追踪目标的)
        proj.SetModel(Owner.Projectile);
        proj.SetTarget(Owner.TargetAttacking);
        proj.Speed = Owner.ProjectileSpeed;
        proj.Gravity = Owner.ProjectileGravity;
        proj.EnableTrack = Owner.ProjectileTrack;
        proj.Damage = Owner.GetDamage();

        // 运动轨迹
        if (Owner.ProjectileGravity > 0)
        {
            proj.ProjectileType = ProjectileType.PARABOLA;
        }
        else
        {
            proj.ProjectileType = ProjectileType.LINEAR;
        }

        // 现在所有的抛射物都是针对敌人造成伤害的
        proj.TeamFlag = TeamFlag.ENEMY;
        proj.TargetFlag = TargetFlag.ALL;
        
        // 发射
        proj.Launch();
    }

    public override void OnTick()
    {
        if (!_hasAttack)
        {
            // 攻击前摇时间到，出现弹道
            if (BattleTime.GetTime() - _startAttackTime >= Owner.GetAtkHitpoint())
            {
                LaunchProjectile();
                _hasAttack = true;
            }
        }
        else
        {
            // 大于攻击间隔
            if (BattleTime.GetTime() - _startAttackTime >= Owner.GetAtkInterval())
            {
                PlayAttack();
            }
            else
            {
                if (!_hasAttackFinish)
                {
                    // 如果当前动作播放完毕，则播放待机动作
                    if (Owner.IsCurrentAnimationFinish())
                    {
                        _hasAttackFinish = true;
                        Owner.PlayAnimation(AnimationName.Idle);
                    }
                }
            }
        }
    }

    // 渲染帧更新，平滑移动
    public override void OnUpdate(float delta)
    {
        _transform.position = Vector3.Slerp(_transform.position, Owner.Position, delta);
    }
}

