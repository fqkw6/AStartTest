using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using PropertySet = System.Collections.Generic.Dictionary<PropertyType, int>;

// 增益或者减益效果
public class Buff
{
    public Actor Owner;     // 这个buff的作用目标
    public Skill Skill;     // 生成这个buff的技能 buff都是由技能产生的，在皇室战争中技能只能通过卡牌生成

    public string Name;
    public string Script;    // buff的脚本文件，一般就是buff的名字

    public int CreateTime;    // 产生这个buff的时间
    public int RemainTime;    // 剩余时间
    private int _stackCount = 0;      // 这个buff的叠加层数

    public bool NeedToRemove = false; // 一个标志，actor会自动移除设为true的buff
    public bool PropertyDirty = false;  // 一个标志，表示Buff的属性已经修改，外部需要重新计算角色属性

    public int Duration; // 持续时间
    public bool IsPassive; // 是否是被动效果，被动效果不需要触发，一开始就生效
    public bool IsDebuff; // 是否是debuff
    public bool IsPurgable; // 是否可以被净化
    public string EffectName; // 生效时播放的光效
    public AttachPoint EffectAttachPoint;   // 特效绑点
    public string HeroEffectName;   // 产生的英雄粒子特效的名称
    public string StatusEffectName; // 状态光效（持续）

    // 光环和持续触发有点类似。光环处理的是角色的进入和离开。持续触发是每隔固定时间进行一次范围判定
    public bool IsAura; // 是否是光环，光环可以处理每帧进入和离开光环区域的单位。注意，
    public string AuraBuff; // 光环产生的二级buff的名字
    public int AuraRadius;  // 光环的半径
    public TeamFlag TeamFlag; // 光环作用阵营
    public TargetFlag TargetFlag; // 光环作用目标

    private int _intervalTime = 0;  // 思考间隔
    private int _lastThinkTime = 0; // 上次思考时间
    private List<Actor> _targetList = new List<Actor>();  // 进入光环的目标 
    private Dictionary<AffixCode, int> _cacheValues = new Dictionary<AffixCode, int>(); // 从脚本中读取的buff会影响的属性和其数值

    // buff被创建 可以获取caster或者owner，可以在这里保存一些数据以方便后面的逻辑事件调用。  注意，此时buff并没有加载到目标身上
    public void OnCreate()
    {
        //Owner.PlayEffect(EffectName);
        // 从脚本获取更新数据
        // 执行buff脚本
        LuaManager.Instance.DoFile("buff/" + Script);

        // 通知脚本buff被创建(创建完毕，但是尚未加载到目标角色 buff 列表中)
        CallFunction("OnCreate");
        Refresh();
    }

    // 刷新buff数据 (脚本中某些特殊事件修改了属性值，需要在脚本中调用Refresh以通知c#更新属性)
    public void Refresh()
    {
        // 通知脚本 buff 属性已经更新
        CallFunction("OnRefresh");
        // 从脚本获取加成数据
        UpdateProperty();

        // 重新计算角色属性
        PropertyDirty = true;
    }

    // 移除这个buff(在脚本中控制某些特殊条件下直接移除buff，比如达到最大触发次数限制)
    public void OnDestroy()
    {
        if (IsAura)
        {
            // 如果是光环的话，移除光环的时候清理buff
            foreach (var item in _targetList)
            {
                item.RemoveBuff(AuraBuff);
            }
        }

        if (Owner != null && Owner.IsDummy)
        {
            Owner.NeedToRemove = true;
            GameObject.Destroy(Owner.gameObject, 0.1f);
        }

        CallFunction("OnDestroy");
        NeedToRemove = true;
    }

    // 每逻辑帧更新（由马甲 Actor 的 tick 函数触发）
    public void OnTick()
    {
        // 处理光环内容
        ProcessAura();
        ProcessThink();
    }

    // 从脚本获取加成数据
    private void UpdateProperty()
    {
        object[] ret = CallFunction("GetProperty");
        if (ret == null)
        {
            return;
        }

        _cacheValues.Clear();

        LuaTable tbl = (LuaTable)ret[0];
        LuaDictTable dict = new LuaDictTable(tbl);
        foreach (var item in dict)
        {
            // lua 中 value 都为 double
            AffixCode code = (AffixCode)Enum.Parse(typeof(AffixCode), (string)item.Key);
            _cacheValues[code] = (int)(double)item.Value;
        }
    }

    // 从脚本更新最新状态
    private void UpdateStatus()
    {
        object[] ret = CallFunction("GetStatus");
        if (ret == null)
        {
            return;
        }

        LuaTable tbl = (LuaTable)ret[0];
        LuaDictTable dict = new LuaDictTable(tbl);
        foreach (var item in dict)
        {
            // lua中value都为double
            ProcessStatus((StatusFlag)item.Key, (bool)item.Value);
        }
    }

    // 调用脚本函数
    private object[] CallFunction(string funcName, params object[] args)
    {
        if (args.Length > 0)
        {
            // 有参数
            object[] param = new object[args.Length + 1];
            param[0] = this;
            for (int i = 0; i < args.Length; ++i)
            {
                param[i + 1] = args[i];
            }
            return LuaManager.Instance.CallFunction(Script, funcName, param);
        }
        else
        {
            // 无参数，只需要传递self
            return LuaManager.Instance.CallFunction(Script, funcName, this);
        }
    }

    // 叠加层数
    public int StackCount
    {
        get { return _stackCount; }
        set
        {
            _stackCount = value;
            if (_stackCount < 0)
            {
                _stackCount = 0;
            }
            // 修改层数后刷新数值
            Refresh();
        }
    }

    //开始buff的计时功能 (OnIntervalThink) ，根据给定的间隔 (float)。 值0代表每帧运行
    public void StartThink(int interval)
    {
        _intervalTime = interval;
        _lastThinkTime = 0;
    }

    // 停止计时器
    public void StopThink()
    {
        _intervalTime = 0;
        _lastThinkTime = 0;
    }

    // 处理每帧的回调
    private void ProcessThink()
    {
        if (_intervalTime <= 0)
        {
            return;
        }

        // 间隔触发的事件
        if (_intervalTime > 0 && BattleTime.GetTime() - _lastThinkTime >= _intervalTime)
        {
            // 通知脚本
            CallFunction("OnThink");
            _lastThinkTime = BattleTime.GetTime();
        }
    }

    // 处理光环内容，对进出光环的单位添加 buff
    private void ProcessAura()
    {
        if (Owner == null || !IsAura)
        {
            return;
        }

        List<Actor> allTargets = BattleController.Instance.GetActorList();
        long ownerID = Owner.UserID;
        Vector3 pos = Owner.Position;

        foreach (var item in allTargets)
        {
            // 判定目标是否受光环影响
            bool enableAura = false;
            if (TeamFlag == TeamFlag.ENEMY)
            {
                enableAura = ownerID != item.UserID;
            }
            else if (TeamFlag == TeamFlag.FRIENDLY)
            {
                enableAura = ownerID == item.UserID;
            }
            else
            {
                enableAura = true;
            }

            if (enableAura)
            {
                if (TargetFlag == TargetFlag.BUILDING && !item.IsBuilding())
                {
                    enableAura = false;
                }
                else if (TargetFlag == TargetFlag.SOLDIER && item.IsBuilding())
                {
                    enableAura = false;
                }
            }

            // 单位超出光环距离
            if (enableAura && Vector3.Distance(pos, item.Position) > AuraRadius)
            {
                enableAura = false;
            }

            // 单位没有在列表中，将单位添加到列表中
            if (_targetList.IndexOf(item) == -1)
            {
                if (enableAura)
                {
                    // 不重复添加buff
                    var buff = item.GetBuff(AuraBuff);
                    if (buff == null)
                    {
                        item.AddBuff(AuraBuff, Duration, Skill);
                    }
                    _targetList.Add(item);
                }
            }
            else
            {
                // 已经在列表中，如果离开了光环区域，则移除
                if (!enableAura)
                {
                    item.RemoveBuff(AuraBuff);
                    _targetList.Remove(item);
                }
            }
        }
    }

    // 处理属性变化
    public void ProcessProperty(PropertySet buffData)
    {
        PropertyDirty = false;
        foreach (var item in _cacheValues)
        {
            switch (item.Key)
            {
                case AffixCode.MoveSpeedPct:
                    // 移动速度
                    AddPropertyValue(buffData, PropertyType.MoveSpeedPct, item.Value);
                    break;
                case AffixCode.AtkSpeedPct:
                    // 攻击速度
                    AddPropertyValue(buffData, PropertyType.AtkSpeedPct, item.Value);
                    break;
                case AffixCode.SpawnSpeedPct:
                    // 召唤速度
                    AddPropertyValue(buffData, PropertyType.SpawnSpeedPct, item.Value);
                    break;
            }
        }
    }

    // 修改属性数据
    private void AddPropertyValue(PropertySet buffData, PropertyType prop, int value)
    {
        if (buffData.ContainsKey(prop))
        {
            buffData[prop] += value;
        }
        else
        {
            buffData[prop] = value;
        }
    }

    // 处理角色状态
    private void ProcessStatus(StatusFlag flag, bool value)
    {
        switch (flag)
        {
            case StatusFlag.STUNNED:
                // 眩晕
                Owner.SetStatus(flag, value);
                break;
        }
    }

}