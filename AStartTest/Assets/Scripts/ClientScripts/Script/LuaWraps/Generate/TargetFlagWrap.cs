﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class TargetFlagWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(TargetFlag));
		L.RegVar("SOLDIER", get_SOLDIER, null);
		L.RegVar("BUILDING", get_BUILDING, null);
		L.RegVar("ALL", get_ALL, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_SOLDIER(IntPtr L)
	{
		ToLua.Push(L, TargetFlag.SOLDIER);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_BUILDING(IntPtr L)
	{
		ToLua.Push(L, TargetFlag.BUILDING);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ALL(IntPtr L)
	{
		ToLua.Push(L, TargetFlag.ALL);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		TargetFlag o = (TargetFlag)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}

