using System;
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

// 脚本管理（LuaState不支持多个）
public class LuaManager : Singleton<LuaManager>
{
    private LuaState _luaState;

    private LuaFunction updateFunc = null;
    private LuaFunction lateUpdateFunc = null;
    private LuaFunction fixedUpdateFunc = null;

    public LuaEvent UpdateEvent { get; private set; }

    public LuaEvent LateUpdateEvent { get; private set; }

    public LuaEvent FixedUpdateEvent { get; private set; }

    public void Init()
    {
        _luaState = new LuaState();
        _luaState.Start(); //启动LUAVM

        LuaBinder.Bind(_luaState);
        LuaCoroutine.Register(_luaState, this);

        // 初始化lua代码
        // 加载base脚本，提供了一些基础函数
        DoFile("base");
    }

    private void Update()
    {
        if (_luaState == null) {
            return;
        }

        if (updateFunc != null) {
            updateFunc.BeginPCall();
            updateFunc.Push(Time.deltaTime);
            updateFunc.Push(Time.unscaledDeltaTime);
            updateFunc.PCall();
            updateFunc.EndPCall();
        }
        _luaState.Collect();
#if UNITY_EDITOR
        _luaState.CheckTop();
#endif
    }

    private void LateUpdate()
    {
        if (lateUpdateFunc != null) {
            lateUpdateFunc.BeginPCall();
            lateUpdateFunc.PCall();
            lateUpdateFunc.EndPCall();
        }
    }

    private void FixedUpdate()
    {
        if (fixedUpdateFunc != null) {
            fixedUpdateFunc.BeginPCall();
            fixedUpdateFunc.Push(Time.fixedDeltaTime);
            fixedUpdateFunc.PCall();
            fixedUpdateFunc.EndPCall();
        }
    }

    private LuaEvent GetEvent(string eventName)
    {
        LuaTable table = _luaState.GetTable(eventName);
        LuaEvent e = new LuaEvent(table);
        table.Dispose();
        table = null;
        return e;
    }

    private void SafeRelease(ref LuaFunction luaRef)
    {
        if (luaRef != null) {
            luaRef.Dispose();
            luaRef = null;
        }
    }

    public object[] DoFile(string filename)
    {
        return _luaState.DoFile(filename);
    }

    // 设置全局对象的值
    public void SetValue(string moduleName, string varName, object value)
    {
        if (string.IsNullOrEmpty(moduleName)) {
            _luaState[varName] = value;
        } else {
            _luaState[moduleName + "." + varName] = value;
        }
    }

    // 获取全局对象的值
    public object GetValue(string moduleName, string varName)
    {
        if (string.IsNullOrEmpty(moduleName)) {
            return _luaState[varName];
        } else {
            return _luaState[moduleName + "." + varName];
        }
    }

    // 调用函数 注意，调用函数时传递的参数类型必须已导出给lua，否则无法成功调用函数
    public object[] CallFunction(string moduleName, string funcName, params object[] args)
    {
        LuaFunction func = null;

        if (string.IsNullOrEmpty(moduleName)) {
            func = _luaState.GetFunction(funcName, false);
        } else {
            func = _luaState.GetFunction(moduleName + "." + funcName, false);
        }

        if (func != null) {
            return func.Call(args);
        }
        return null;
    }

    // 获取函数
    public LuaFunction GetFunction(string moduleName, string funcName)
    {
        if (string.IsNullOrEmpty(moduleName)) {
            return _luaState.GetFunction(funcName);
        } else {
            return _luaState.GetFunction(moduleName + "." + funcName);
        }
    }

    public void LuaGC()
    {
        _luaState.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
    }

    public void Close()
    {
        SafeRelease(ref updateFunc);
        SafeRelease(ref lateUpdateFunc);
        SafeRelease(ref fixedUpdateFunc);

        if (UpdateEvent != null) {
            UpdateEvent.Dispose();
            UpdateEvent = null;
        }

        if (LateUpdateEvent != null) {
            LateUpdateEvent.Dispose();
            LateUpdateEvent = null;
        }

        if (FixedUpdateEvent != null) {
            FixedUpdateEvent.Dispose();
            FixedUpdateEvent = null;
        }
        _luaState.Dispose();
        _luaState = null;
    }
}