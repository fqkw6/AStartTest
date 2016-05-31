using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lockstep;

// 抛射物
public class Projectile : MonoBehaviour
{
    public System.Action<Projectile> OnProjectileHit;

    public long OwnerID; // 属于哪个玩家

    public bool IsDummyProjectile = false;  // 是否是多抛射物中的马甲抛射物
    public Actor Owner; // 发射这个抛射物的单位，如果是法术卡，则为空
    public Skill Skill; // 发射这个抛射物的技能 一般情况下这两个只有一个有效
    public bool NeedToRemove;

    public int Speed = 5; // 飞行速度
    public int Gravity; // 重力（影响抛物线）
    public int ParabolaHeight = 5; // 抛物线运动时高度
    public int Timeout = 10 * 1000; // 超时时间，超过此时间自动销毁
    public bool EnableTrack = false; // 是否允许追踪
    public TeamFlag TeamFlag; // 目标选择
    public TargetFlag TargetFlag;
    public ProjectileType ProjectileType = ProjectileType.LINEAR; // 抛射物的运动模式
    public int MultipleProjectiles = 1;  // 抛射物数量
    public ProjectileFormation ProjectilesFormation = ProjectileFormation.NONE;  // 抛射物队形

    public int Damage = 0; // 伤害
    public string TargetBuff; // 击中目标产生buff
    public int BuffTime; // buff持续时间
    public int Radius; // 伤害半径
    public bool Pushback; //  是否会击退目标
    public int MaxTargetCount; // 最多目标数目 0代表不限制数目
    public int CrownTowerDamagePercent; // 对塔造成伤害减少百分比 
    public string HitEffect; // 发生碰撞后的光效
    public bool ProcessHitInLua;

    public Actor Target;
    public Vector3 TargetPosition;

    public GameObject _model;
    private bool _isFlying = false;

    private float _distanceToTarget = 0;
    private Transform _transform;
    private float _moveSpeed;
    private Transform _dummytransform;

    /******fly01*****/
    public Quaternion _nextRot;
    public Vector3 _nextPos;
    private Vector3 SpeedVec;//初速度向量
    private Vector3 GravityVec;//重力向量
    private float flyTime = 0;//代表从A点出发到B经过的时长
    private float timePass = 0;
    /*******fly01*******/

    // 设置模型或粒子的prefab
    public void SetModel(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>("Model/Misc/" + prefabName);
        _model = Instantiate(prefab);
        _model.transform.SetParent(transform);
        _model.transform.localPosition = Vector3.zero;
        _transform = transform;
    }

    // 设置目标
    public void SetTarget(Actor target)
    {
        if (target != null)
        {
            Target = target;
            TargetPosition = Target.transform.position;
            transform.rotation = Quaternion.LookRotation(TargetPosition - _transform.position);
        }
    }

    // 设置目标位置，一般是法术牌
    public void SetTarget(Vector3 pos)
    {
        TargetPosition = pos;
        transform.rotation = Quaternion.LookRotation(TargetPosition - transform.position);
    }

    // 发射
    public void Launch()
    {
        if (Owner != null)
        {
            Gravity = Owner.ProjectileGravity;
            Speed = Owner.ProjectileSpeed;
        }
        _dummytransform = new GameObject("Dummy").transform;
        _dummytransform.position = _transform.position;
        _dummytransform.rotation = _transform.rotation;

        /*******fly01*******/
        Gravity = -10;
        flyTime = Vector3.Distance(_transform.position, TargetPosition) / (Speed / 65f * 6.5f);
        SpeedVec = new Vector3((TargetPosition.x - _transform.position.x) / flyTime,
          (TargetPosition.y - _transform.position.y) / flyTime - 0.5f * Gravity * flyTime, (TargetPosition.z - _transform.position.z) / flyTime);
        GravityVec = Vector3.zero;  // 重力初始速度为0
        _nextPos = _transform.position;
        _nextRot = _transform.rotation;
        /*******fly01*******/

        // Speed 为每秒移动格子数，65代表一格
        _moveSpeed = Speed / 65f * 6.5f * GameConfig.FRAME_INTERVAL / 1000f;
        _distanceToTarget = Vector3.Distance(_transform.position, TargetPosition);
        _isFlying = true;
    }

    public void OnTick()
    {
        if (!_isFlying)
        {
            return;
        }

        // 如果是单位发射的，且目标已经死亡
        if (Target == null && Owner != null)
        {
            // 通知战斗逻辑，清理资源
            if (OnProjectileHit != null)
            {
                OnProjectileHit(this);
            }

            NeedToRemove = true;

            // 稍微延迟一下销毁物体，战斗逻辑会清理需要删除的抛射物，不会对逻辑造成影响
            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
            return;
        }

        if (EnableTrack)
        {
            // 追踪目标
            TargetPosition = Target.transform.position + new Vector3(0, 5, 5);
        }

        switch (ProjectileType)
        {
            case ProjectileType.LINEAR:
                MoveLiner();
                break;
            case ProjectileType.PARABOLA:
                //ProjectileMove();
                break;
            case ProjectileType.BAZIER:
                break;
            default:
                break;
        }
        //ProjectileMove_01();

        //到了目标
        if (Vector3.Distance(_transform.position, TargetPosition) <= 1f)
        {
            ProcessHit();
        }
    }

    // 渲染帧更新
    public void OnUpdate(float delta)
    {
        if (!_isFlying || _transform == null) return;
        //_transform.position = Vector3.Lerp(_transform.position, _dummytransform.position, delta);
        //_transform.rotation = Quaternion.Lerp(_transform.rotation, _dummytransform.rotation, delta);

        /**********fly01**********/
        _transform.position = Vector3.Lerp(_transform.position, _nextPos, delta);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, _nextRot, delta);
        /**********fly01**********/
    }

    // 发生碰撞 (TODO 抛射物似乎都不是按物理碰撞的，要么是一对一的单体必中目标，要么是法术的指定目标)
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == Target.gameObject.tag)
        {
            // 处理受击
            //ProcessHit();
        }
    }

    // 处理击中后的效果，可以为爆炸 弹射 穿透 粘着  消失
    public void ProcessHit()
    {
        _isFlying = false;
        if (!IsDummyProjectile)
        {
            List<Actor> targets = new List<Actor>();

            if (Radius > 0)
            {
                // 范围目标
                targets.AddRange(BattleController.Instance.FindTargets(OwnerID, TargetPosition, Radius, TeamFlag.ENEMY, TargetFlag.ALL));
            }
            else
            {
                // 单体目标
                if (Target != null)
                {
                    targets.Add(Target);
                }
            }

            // 造成伤害
            foreach (var item in targets)
            {
                item.OnHit(Damage);
            }

            if (Owner != null)
            {
                // 单位的抛射物可以获取命中单位
                Owner.CallFunction("OnProjectileHitUnit", TargetPosition, targets.ToArray());
            }

            if (Skill != null)
            {
                // 技能的抛射物可以获取目标坐标
                Skill.CallFunction("OnProjectileHitUnit", TargetPosition, targets.ToArray());
            }
        }

        // 通知战斗逻辑，清理资源
        if (OnProjectileHit != null)
        {
            OnProjectileHit(this);
        }

        NeedToRemove = true;

        // 稍微延迟一下销毁物体，战斗逻辑会清理需要删除的抛射物，不会对逻辑造成影响
        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }

    // 逻辑帧更新抛射物位置
    private void ProjectileMove()
    {
        _dummytransform.LookAt(TargetPosition);
        float rotateAnglePerTurn = Mathf.Min(1, Vector3.Distance(_dummytransform.position, TargetPosition) / _distanceToTarget) * Gravity;
        _dummytransform.rotation *= Quaternion.Euler(Mathf.Clamp(-rotateAnglePerTurn, -Gravity, Gravity), 0, 0);
        float currentDistanceToTarget = Vector3.Distance(_dummytransform.position, TargetPosition);
        _dummytransform.position += (_dummytransform.forward * Mathf.Min(_moveSpeed, currentDistanceToTarget));

        _transform.position = _dummytransform.position;
        _transform.rotation = _dummytransform.rotation;
    }

    // 逻辑帧更新抛射物位置
    void ProjectileMove_01()
    {
        GravityVec.y = Gravity * (timePass += GameConfig.FRAME_INTERVAL / 1000f);

        float currentDistanceToTarget = Vector3.Distance(_transform.position, TargetPosition);
        float moveDistance = ((SpeedVec + GravityVec).normalized * _moveSpeed).magnitude;

        _nextPos += (SpeedVec + GravityVec).normalized * Mathf.Min(currentDistanceToTarget, moveDistance);
        //_nextPos += (SpeedVec + GravityVec) * GameConfig.FRAME_INTERVAL / 1000f;
        //_nextPos += (SpeedVec + GravityVec).normalized * _moveSpeed;
        _nextRot = Quaternion.LookRotation(_nextPos - _transform.position);
        _transform.position = _nextPos;
        _transform.rotation = _nextRot;
    }

    // 逻辑帧更新抛射物位置
    void MoveLiner()
    {
        float currentDistanceToTarget = Vector3.Distance(_transform.position, TargetPosition);
        float moveDistance = (_moveSpeed * (TargetPosition - _transform.position).normalized).magnitude;
        _nextPos += Mathf.Min(currentDistanceToTarget, moveDistance) * (TargetPosition - _transform.position).normalized;
    }
}