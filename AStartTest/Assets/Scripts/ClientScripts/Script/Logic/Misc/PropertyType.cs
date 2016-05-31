using UnityEngine;
using System.Collections;

// 属性类别，命名习惯跟AffixCode统一 需要受到b符文或者uff加成的定义在这里，不受加成的可以直接定义在actor中
public enum PropertyType
{
    Hp, //  增加生命值
    HpPct, //  增加生命值百分比
    Shield, // 护盾
    MoveSpeedPct, // 移动速度
    AtkSpeedPct, // 攻击速度
    SpawnSpeedPct,  // 召唤速度加成
    Dmg,    // 伤害
    DmgPct, // 伤害百分比

    // 对技能的影响属性 多数技能符文属性不需要定义在这里，直接在lua里面处理就可以了
    // 部分需要在c#端进行处理的才需要定义在这里。
    SummonCount, //"SummonCount";    // 增加召唤数量
    ProjectileCount, //"ProjectileCount";    // 增加抛射物数量
}