﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class ProjectileTypeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(ProjectileType));
		L.RegVar("LINEAR", get_LINEAR, null);
		L.RegVar("PARABOLA", get_PARABOLA, null);
		L.RegVar("BAZIER", get_BAZIER, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LINEAR(IntPtr L)
	{
		ToLua.Push(L, ProjectileType.LINEAR);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_PARABOLA(IntPtr L)
	{
		ToLua.Push(L, ProjectileType.PARABOLA);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_BAZIER(IntPtr L)
	{
		ToLua.Push(L, ProjectileType.BAZIER);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		ProjectileType o = (ProjectileType)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}
