using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 装备、buff等功能中的词缀属性，在配置文件和buff的lua脚本中会以文本的形式引用它
public enum AffixCode
{
    Hp, //  增加生命值
    HpPct, //  增加生命值百分比
    Shield, // 护盾
    MoveSpeed, // 移动速度
    MoveSpeedPct, // 移动速度
    AtkSpeed, // 攻击速度
    AtkSpeedPct, // 攻击速度
    SpawnSpeedPct,  // 召唤速度
    Dmg,    // 伤害
    DmgPct, // 伤害百分比

    // 对技能的影响属性 多数技能符文属性不需要定义在这里，直接在lua里面处理就可以了
    // 部分需要在c#端进行处理的才需要定义在这里。
    SummonCount, //"SummonCount";    // 增加召唤数量
    ProjectileCount, //"ProjectileCount";    // 增加抛射物数量
}
