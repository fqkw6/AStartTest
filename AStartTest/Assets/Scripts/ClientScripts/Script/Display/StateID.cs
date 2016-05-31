
// 状态id
public enum StateID
{
    // 角色的状态
    // 角色的状态
    STATE_NONE,
    STATE_PREVIEW,  // 在手指上预览，松开手指后进入predeploy状态，等待服务器确认并经过固定延迟时间后，进入deploy状态，部署时间过后开始ai流程
    STATE_DEPLOY,   // 角色一般都有1秒钟的放置时间，放置会有一个动画表现，放置时间到后，角色才会开始ai
    STATE_IDLE, // 待机
    STATE_MOVE_TO_POSITION, // 移动到目的地
    STATE_MOVE_TO_TARGET, // 移动到目标
    STATE_DIE, // 死亡
    STATE_KNOCKBACK,    // 受击击退
    STATE_REVIVE, // 复活状态
    STATE_ATTACK, // 攻击
    STATE_PROJECTILE_ATTACK,    // 远程攻击
    STATE_WIN, // 胜利
    STATE_STUN,
    STATE_CONTROLLED, // 被控制状态  如冰冻 眩晕  状态内部根据人物status决定该做什么表现

    // 游戏的状态
    GAME_STATE_LOGO,        // 显示splash screen
    GAME_STATE_LOGIN,       // 登陆状态(更新游戏资源 登录 注册)
    GAME_STATE_NORMAL,        // 主界面
    GAME_STATE_BATTLE,      // 战斗状态
}