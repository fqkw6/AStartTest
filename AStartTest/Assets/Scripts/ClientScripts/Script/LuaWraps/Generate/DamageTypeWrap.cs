﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class DamageTypeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(DamageType));
		L.RegVar("PHYSIC", get_PHYSIC, null);
		L.RegVar("MAGIC", get_MAGIC, null);
		L.RegVar("FIRE", get_FIRE, null);
		L.RegVar("LIGHTNING", get_LIGHTNING, null);
		L.RegVar("COLD", get_COLD, null);
		L.RegVar("POISON", get_POISON, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_PHYSIC(IntPtr L)
	{
		ToLua.Push(L, DamageType.PHYSIC);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_MAGIC(IntPtr L)
	{
		ToLua.Push(L, DamageType.MAGIC);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FIRE(IntPtr L)
	{
		ToLua.Push(L, DamageType.FIRE);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LIGHTNING(IntPtr L)
	{
		ToLua.Push(L, DamageType.LIGHTNING);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_COLD(IntPtr L)
	{
		ToLua.Push(L, DamageType.COLD);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_POISON(IntPtr L)
	{
		ToLua.Push(L, DamageType.POISON);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		DamageType o = (DamageType)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}

