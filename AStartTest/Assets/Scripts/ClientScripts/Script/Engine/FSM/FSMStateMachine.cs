using System;
using System.Collections.Generic;
using UnityEngine;

// 角色基础状态机
public class FSMStateMachine
{
    private Dictionary<StateID, FSMStateBase> _allStates = new Dictionary<StateID, FSMStateBase>();  // 状态列表
    private FSMStateBase _currentState;

    private StateID _currentStateId = StateID.STATE_NONE;
    private Dictionary<StateID, List<StateID>> _connections = new Dictionary<StateID, List<StateID>>();

    public object Owner;

    public FSMStateMachine(object owner)
    {
        Owner = owner;
    }

    // 添加状态
    public void AddState(StateID stateId, FSMStateBase state)
    {
        state.SetFSM(this);  // 设置当前状态机
        _allStates[stateId] = state;
    }

    // 清除状态
    public void ClearAllState()
    {
        _allStates.Clear();

        // TODO 看看是否需要OnExit
        _currentState = null;
        _currentStateId = StateID.STATE_NONE;
    }

    // 切换状态
    public void ChangeState(StateID nextState, params object[] param)
    {
        if (nextState == _currentStateId) {
            // 是同一状态，刷新状态数据
            if (_currentState != null) {
                _currentState.OnRefresh(param);
            }
            return;
        }

        // 不能进行状态切换
        if (_currentState != null && !_currentState.CheckTransition(nextState)) {
            return;
        }

        List<StateID> list;
        if (_connections.TryGetValue(nextState, out list)) {  // 获取与指定的键相关联的值
            // 如果目标状态有添加链接，则判定当前状态是否有链接到目标状态，如果没有的话，则禁止迁移状态
            if (list.IndexOf(_currentStateId) == -1) {
                return;
            }
        }

        FSMStateBase state;
        if (_allStates.TryGetValue(nextState, out state)) {
            if (_currentState != null) {
                _currentState.OnExit();
                _currentState = null;
            }

            _currentStateId = nextState;
            _currentState = state;
            _currentState.OnEnter(param);
        } else {
            Debug.LogError("State Not Found: " + nextState);
        }
    }

    // 获取当前状态
    public StateID CurrentStateID
    {
        get { return _currentStateId; }
    }

    // 是否处于某个状态
    public bool IsInState(StateID stateID)
    {
        return _currentStateId == stateID;
    }

    // 外部调用 更新状态
    public void UpdateFSM(float delta)
    {
        if (_currentState != null) {
            _currentState.OnUpdate(delta);
        }
    }

    // 更新逻辑状态
    public void UpdateFSM()
    {
        if (_currentState != null) {
            _currentState.OnTick();
        }
    }

    // 添加链接，凡是添加了链接的状态（to），只有指定的状态（from）可以切换到目标状态
    public void AddConnection(StateID from, StateID to)
    {
        List<StateID> list;
        if (!_connections.TryGetValue(to, out list)) {
            list = new List<StateID>();
            _connections[to] = list;
        }

        list.Add(from);
    }

    // 获取当前状态
    public FSMStateBase GetCueerneState()
    {
        return _currentState;
    }
}
