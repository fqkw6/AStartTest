﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class ProjectileFormationWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(ProjectileFormation));
		L.RegVar("NONE", get_NONE, null);
		L.RegVar("ARROW", get_ARROW, null);
		L.RegVar("FAN", get_FAN, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NONE(IntPtr L)
	{
		ToLua.Push(L, ProjectileFormation.NONE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ARROW(IntPtr L)
	{
		ToLua.Push(L, ProjectileFormation.ARROW);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FAN(IntPtr L)
	{
		ToLua.Push(L, ProjectileFormation.FAN);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		ProjectileFormation o = (ProjectileFormation)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}
