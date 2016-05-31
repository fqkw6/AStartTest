﻿using UnityEngine;
using System;
using System.Collections.Generic;
using LuaInterface;

using BindType = ToLuaMenu.BindType;

public static class CustomSettings
{
    public static string saveDir = Application.dataPath + "/Script/LuaWraps/Generate/";
    public static string luaDir = LuaConst.luaDir;
    public static string toluaBaseType = Application.dataPath + "/Plugin/ToLua/BaseType/";
    public static string toluaLuaDir = Application.dataPath + "/Plugin/ToLua/ToLua/Lua";
     
    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>
    {        
        //typeof(UnityEngine.Application),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.Screen),
        typeof(UnityEngine.SleepTimeout),
        typeof(UnityEngine.Input),
        typeof(UnityEngine.Resources),
        typeof(UnityEngine.Physics),
        typeof(UnityEngine.RenderSettings),
        typeof(UnityEngine.QualitySettings),
    };

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList = 
    {        
        _DT(typeof(Action)),
        _DT(typeof(UnityEngine.Events.UnityAction)),       
        
        _DT(typeof(TestEventListener.OnClick)),
        _DT(typeof(TestEventListener.VoidDelegate)),
    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList = 
    {                
        _GT(typeof(Debugger)),                       
                                       
        _GT(typeof(Component)),
        _GT(typeof(Behaviour)),
        _GT(typeof(MonoBehaviour)),        
        _GT(typeof(GameObject)),
        _GT(typeof(Transform)),

        /* 
        _GT(typeof(Space)),
        _GT(typeof(Camera)),
        _GT(typeof(CameraClearFlags)),           
        _GT(typeof(Material)),
        _GT(typeof(Renderer)),        
        _GT(typeof(MeshRenderer)),
        _GT(typeof(SkinnedMeshRenderer)),
        _GT(typeof(Light)),
        _GT(typeof(LightType)),     
        _GT(typeof(ParticleEmitter)),
        _GT(typeof(ParticleRenderer)),
        _GT(typeof(ParticleAnimator)),   
        _GT(typeof(ParticleSystem)),                
        _GT(typeof(Physics)),
        _GT(typeof(Collider)),
        _GT(typeof(BoxCollider)),
        _GT(typeof(MeshCollider)),
        _GT(typeof(SphereCollider)),        
        _GT(typeof(CharacterController)),
        _GT(typeof(Animation)),             
        _GT(typeof(AnimationClip)),
        _GT(typeof(TrackedReference)),
        _GT(typeof(AnimationState)),  
        _GT(typeof(QueueMode)),  
        _GT(typeof(PlayMode)),                          
        _GT(typeof(AudioClip)),
        _GT(typeof(AudioSource)),                        
        _GT(typeof(Application)),
        _GT(typeof(Input)),              
        _GT(typeof(KeyCode)),             
        _GT(typeof(Screen)),
        _GT(typeof(Time)),
        _GT(typeof(RenderSettings)),
        _GT(typeof(SleepTimeout)),                        
        _GT(typeof(AsyncOperation)),
        _GT(typeof(AssetBundle)),   
        _GT(typeof(BlendWeights)),   
        _GT(typeof(QualitySettings)),          
        _GT(typeof(AnimationBlendMode)),  
        _GT(typeof(RenderTexture)),
        _GT(typeof(Rigidbody)), 
        _GT(typeof(CapsuleCollider)),
        _GT(typeof(WrapMode)),
        _GT(typeof(Texture)),
        _GT(typeof(Shader)),
        _GT(typeof(Texture2D)),
        _GT(typeof(WWW)),
        */
        // 游戏导出
        _GT(typeof(Actor)),
        _GT(typeof(Skill)),
        _GT(typeof(Buff)),
        _GT(typeof(Projectile)),
        _GT(typeof(BattleController)),
        _GT(typeof(BattleTime)),

        _GT(typeof(GameConfig)),
        
        _GT(typeof(DamageType)),
        _GT(typeof(RangeType)),
        _GT(typeof(ProjectileType)),
        _GT(typeof(TargetFlag)),
        _GT(typeof(TeamFlag)),
        _GT(typeof(StatusFlag)),
        _GT(typeof(FindType)),
        _GT(typeof(AttachType)),
        _GT(typeof(ProjectileFormation)),
    };

    public static List<Type> dynamicList = new List<Type>()
    {        
//        typeof(MeshRenderer),
//        typeof(ParticleEmitter),
//        typeof(ParticleRenderer),
//        typeof(ParticleAnimator),
//
//        typeof(BoxCollider),
//        typeof(MeshCollider),
//        typeof(SphereCollider),
//        typeof(CharacterController),
//        typeof(CapsuleCollider),
//
//        typeof(Animation),
//        typeof(AnimationClip),
//        typeof(AnimationState),        
//
//        typeof(BlendWeights),
//        typeof(RenderTexture),
//        typeof(Rigidbody),
    };

    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>()
    {
        
    };

    static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }    
}
