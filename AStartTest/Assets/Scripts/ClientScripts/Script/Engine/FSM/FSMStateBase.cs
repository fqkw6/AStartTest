using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FSMStateBase
{
    protected FSMStateMachine _fsm;

    // 设置状态机
    public void SetFSM(FSMStateMachine fsm)
    {
        _fsm = fsm;
    }

    // 覆盖以控制状态切换
    public virtual bool CheckTransition(StateID nextState)
    {
        return true;
    }

    // 进入此状态
    public virtual void OnEnter(params object[] param)
    {
    }
    
    // 在状态中刷新数据
    public virtual void OnRefresh(params object[] param)
    {
    }

    // 每帧刷新
    public virtual void OnUpdate(float delta)
    {
    }

    // 每逻辑帧更新
    public virtual void OnTick()
    {
        
    }

    // 离开此状态
    public virtual void OnExit()
    {
    }
}