using UnityEngine;
using System.Collections;

// 人物身体部件，与模型有关
public enum BodyPart
{
    HELMET = 1, // 头盔
    SHOULDERS = 2, // 护肩
    GLOVES = 3, // 手套，手臂
    CHEST = 4, // 胸甲
    BOOTS = 5, // 靴子
    PANTS = 6, // 护腿
    HAIR = 7, // 头发
}

// 绑点 用于额外添加物体到指定绑点
public enum AttachPoint
{
    RIGHT_HAND, // 右手
    LEFT_HAND, // 左手（用于双持）
    LEFT_ARM, // 左臂（盾牌）
}

// 卡牌类型
public enum CardType
{
    SOLDIER = 1,    // 部队
    SPELL = 2,  // 法术
    BUILDING = 3,   // 建筑
}

// 攻击属性
public enum DamageType
{
    PHYSIC, // 物理攻击
    MAGIC, // 魔法攻击
    FIRE, // 火焰伤害，后面的属性攻击不同的属性会触发不同的效果，如火焰伤害可以触发燃烧效果，闪电伤害可以触发感电效果
    LIGHTNING, // 闪电伤害
    COLD, // 冰冷伤害
    POISON, // 毒伤
}


// 动画action序号，对应动画controller中的action参数
public enum AnimationAction
{
    IDLE = 0, // 站立
    RUN = 1, // 跑步
    ATTACK = 2, // 攻击
    SKILL = 3, // 技能
    DIE = 4, // 死亡
    BLOCK = 10, // 格挡
    CORRUPTION = 11, // 被控制
    MOUNT = 12, // 骑马
    FISHING = 13, // 钓鱼
    FIDGET = 14, // 环顾周围
}

// 技能范围类型
public enum RangeType
{
    SINGLE, // 单体
    CIRCLE, // 圆形
    LINE, // 直线
    SECTOR, // 扇形
    FULL, // 全体
}

// 抛射物运动类型
public enum ProjectileType
{
    LINEAR, // 直线
    PARABOLA,// 抛物线
    BAZIER, // 复杂的贝塞尔曲线运动
}

// 技能目标阵营
public enum TeamFlag
{
    BOTH, //	全部
    ENEMY, //	敌人
    FRIENDLY, //	友军
}

// 技能目标额外标签
public enum TargetFlag
{
    SOLDIER, // 作用于士兵
    BUILDING,    // 作用于建筑
    ALL, // 作用于全部
}

// 可设置的状态(这个)
public enum StatusFlag
{
    ATTACK_IMMUNE, //	攻击免疫
    FROZEN, //	冰冻，只会暂停单位的动作
    INVISIBLE, //	隐形
    INVULNERABLE, //无敌，有这个效果的单位如果播放动画将无效
    SILENCED, //	沉默
    STUNNED, //	击晕

    MAX_COUNT,
}

// 寻找敌人的类型
public enum FindType
{
    FIND_ANY,   // 随便取一个
    FIND_CLOSEST, //    最近的那个
    FIND_FARTHEST, //   最远的那个
}

// 粒子附着点类型
public enum AttachType
{
    ABSORIGIN, // 附着于绝对原点
    ABSORIGIN_FOLLOW, // 附着于绝对原点并且跟随
    WORLDORIGIN, // 附着世界的原点
    CUSTOMORIGIN, // 附着于特定的绝对原点
    CUSTOMORIGIN_FOLLOW, // 附着于特定的绝对原点并且跟随    OVERHEAD_FOLLOW,// 附着于头部上方并且跟随（举个例子天使JJ的头顶光环）
    POINT, // 附着于绑点
    POINT_FOLLOW, // 附着于绑点并且跟随
    ROOTBONE_FOLLOW, // 附着脚底并且跟随（举个例子把天使JJ的头顶光环放到脚下........）
    EYES_FOLLOW, // 附着于眼部
    WORLDPOINT,  // 附着于世界坐标，不设置父物体
}

// 游戏的各项功能
public enum GameFunc
{
    NONE,
    BAG,    // 背包
}

// 抛射物队形
public enum ProjectileFormation
{
    NONE,  // 随机（万箭齐发
    ARROW,  // 箭型
    FAN,  // 扇形（公主
}
