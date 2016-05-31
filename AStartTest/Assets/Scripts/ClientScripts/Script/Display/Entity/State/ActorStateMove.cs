using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pathfinding;

// 移动到目的地
public class ActorStateMove : ActorStateBase
{
    protected float _moveSpeed = 8; // 移动速度
    protected float _turningSpeed = 8; // 转向速度
    protected float _distanceError = 1f;

    protected int _lastScanTime = 0;  // 上次搜索敌人的时间
    protected bool _hasTargetPosition = false; // 是否需要移动
    protected float _verticalSpeed; // 当前移动速度
    protected Vector3 _currentMoveToPos; // 移动目的地

    protected Path _path;       // 当前的路径
    protected Seeker _seeker;   // 寻路组件
    protected Transform _transform;

    protected Vector3 currentWaypoint;
    public float pickNextWaypointDist = 2;  // 距离多少距离开始切换到下一个路径点
    public float forwardLook = 1;
    protected int currentWaypointIndex = 0; // 当前寻路点
    protected Quaternion nextRotation;  // 下一个路点转向

    public override void OnEnter(params object[] param)
    {
        EventDispatcher.AddEventListener(EventID.ACTOR_PROPERTY_CHANGE, OnActorPropertyChange);
        _transform = Owner.transform;
        _seeker = Owner.GetComponent<Seeker>();
        _moveSpeed = Owner.GetMoveSpeed() * GameConfig.FRAME_INTERVAL / 1000f;   // movespeed是每秒运行距离
        _hasTargetPosition = true;

        if (_seeker != null)
        {
            _seeker.pathCallback = OnPathComplete;
        }
    }

    public override void OnExit()
    {
        EventDispatcher.RemoveEventListener(EventID.ACTOR_PROPERTY_CHANGE, OnActorPropertyChange);
        if (_seeker == null)
        {
            return;
        }

        // 停止当前的寻路
        if (!_seeker.IsDone())
        {
            _seeker.GetCurrentPath().Error();
        }

        // 释放当前路径
        if (_path != null)
        {
            _path.Release(this);
            _path = null;
        }
    }
    
    // 逻辑帧更新
    public override void OnTick()
    {
        // 移动的时候也会搜索目标
        ScanTarget();

        // 移动
        if (_hasTargetPosition)
        {
            // 到目的地了
            if (Vector3.Distance(Owner.Position, _currentMoveToPos) <= _distanceError)
            {
                _hasTargetPosition = false;
                Owner.Idle();
            }
            else
            {
                // 跑步中
                if (!Owner.IsPlayingAnimation(Owner.MoveAnimation))
                {
                    Owner.PlayAnimation(Owner.MoveAnimation);
                }
            }

            // 新的方向
            Vector3 dir = currentWaypoint - Owner.Position;

            if (dir.magnitude <= _moveSpeed)
            {
                Owner.Position = currentWaypoint;
            }
            else
            {
                // 移动到目标点
                Owner.Position = Owner.Position + dir.normalized * _moveSpeed;
            }

            if ((Owner.Position - _transform.position) != Vector3.zero)
            nextRotation = Quaternion.LookRotation(Owner.Position - _transform.position);

            if (Vector3.Distance(Owner.Position, currentWaypoint) <= _distanceError)
            {
                // 到了一个寻路点，寻找下一个寻路点
                NextWaypoint();
            }
        }
    }

    // 搜索敌人，单位在移动的时候也会搜索新的攻击目标，直到开始攻击
    private void ScanTarget()
    {
        if (BattleTime.GetTime() - _lastScanTime > 1000)
        {
            _lastScanTime = BattleTime.GetTime();
            Actor target = Owner.ScanTarget();
            if (target != null)
            {
                // 新的目标或者新的路径 TODO 将来进行优化如果路径没有改变则不需要重新寻路
                _currentMoveToPos = target.Position;
                SearchPath();
            }
        }
    }

    // 开始计算路径
    protected void SearchPath()
    {
        //_seeker.StartPath(Owner.Position, _currentMoveToPos + Owner.PreviewOffset);
        _seeker.StartPath(Owner.Position, _currentMoveToPos);
    }

    // 寻找下一个寻路点
    private void NextWaypoint()
    {
        if (_path == null || _path.vectorPath == null || _path.vectorPath.Count == 0)
        {
            return;
        }

        ++currentWaypointIndex;

        int count = _path.vectorPath.Count;
        if (currentWaypointIndex >= count) { currentWaypointIndex = count - 1; }
        if (currentWaypointIndex <= 0) currentWaypointIndex = 0;

        currentWaypoint = _path.vectorPath[currentWaypointIndex];
    }

    // 渲染帧更新，平滑移动
    public override void OnUpdate(float delta)
    {
        _transform.position = Vector3.Slerp(_transform.position, Owner.Position, delta);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, nextRotation, delta);
    }

    // 当搜索路径完成的时候调用
    private void OnPathComplete(Path _p)
    {
        ABPath p = _p as ABPath;

        if (p == null)
        {
            return;
        }

        //Claim the new path
        p.Claim(this);

        // Path couldn't be calculated of some reason.
        // More info in p.errorLog (debug string)
        if (p.error)
        {
            p.Release(this);
            return;
        }

        //Release the previous path
        if (_path != null) _path.Release(this);

        //Replace the old path
        _path = p;

        // 从头开始寻路
        currentWaypointIndex = -1;
        NextWaypoint();
    }

    // TODO:速度会因 buff 改变(这里的一个改变会广播给所有的角色)
    public void OnActorPropertyChange()
    {
        _moveSpeed = Owner.GetMoveSpeed() * GameConfig.FRAME_INTERVAL / 1000f;   // movespeed是每秒运行距离
    }
}
