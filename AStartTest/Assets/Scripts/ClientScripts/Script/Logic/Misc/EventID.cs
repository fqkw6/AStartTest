// 事件id 用于ui和逻辑之间的通信
public partial class EventID
{
    public const string EVENT_BATTLE_PAUSE = "EVENT_BATTLE_PAUSE";             // 战斗暂停
    public const string EVENT_BATTLE_RESUME = "EVENT_BATTLE_RESUME";            // 战斗继续
    public const string EVENT_BATTLE_RESULT_CLOSE = "EVENT_BATTLE_RESULT_CLOSE";      // 战斗结束确认完毕，回到主界面
    public const string EVENT_BATTLE_ON_SKILL_CLICK = "EVENT_BATTLE_ON_SKILL_CLICK";
    public const string EVENT_BATTLE_ON_SKILL_TOUCH_BEGIN = "EVENT_BATTLE_ON_SKILL_TOUCH_BEGIN";
    public const string EVENT_BATTLE_ON_SELECT_FLAG = "EVENT_BATTLE_ON_SELECT_FLAG";
    public const string EVENT_BATTLE_ON_SELECT_SKILL = "EVENT_BATTLE_ON_SELECT_SKILL";
    public const string EVENT_SHOW_COMBO_TEXT = "EVENT_SHOW_COMBO_TEXT";          // 显示连击数字
    public const string EVENT_SHOW_CRITICAL_TEXT = "EVENT_SHOW_CRITICAL_TEXT";       // 显示暴击文字
    public const string EVENT_SKILL_ACTION_HIT = "EVENT_SKILL_ACTION_HIT";             // 击中目标，可以进行相关处理，如添加buff
    public const string EVENT_SKILL_ACTION_BOMB = "EVENT_SKILL_ACTION_BOMB";            // 发生爆炸
    public const string EVENT_SKILL_ACTION_PROJECTILE_HIT = "EVENT_SKILL_ACTION_PROJECTILE_HIT";  // 飞行物碰到目标
    public const string EVENT_UI_CONNECT_LOGIN_SERVER_OK = "EVENT_UI_CONNECT_LOGIN_SERVER_OK";
    public const string EVENT_UI_ACCOUNT_LOGIN_OK = "EVENT_UI_ACCOUNT_LOGIN_OK";
    public const string EVENT_UI_REGISTER_OK = "EVENT_UI_REGISTER_OK";
    public const string EVENT_UI_CREATE_ROLE_OK = "EVENT_UI_CREATE_ROLE_OK";
    public const string EVENT_UI_LOGIN_OK = "EVENT_UI_LOGIN_OK";
    public const string EVENT_UI_MAIN_REFRESH_VALUE = "EVENT_UI_MAIN_REFRESH_VALUE";            // 主界面刷新角色金钱等级等信息
    public const string EVENT_UI_MAIN_NEW_FLAG = "EVENT_UI_MAIN_NEW_FLAG";                 // 主界面功能上的提醒图标
    public const string EVENT_UI_SHOW_SYSTEM_MSG = "EVENT_UI_SHOW_SYSTEM_MSG";
    public const string EVENT_UI_SHOW_CENTER_MSG = "EVENT_UI_SHOW_CENTER_MSG";
    public const string EVENT_UI_SHOW_FLOATING_MSG = "EVENT_UI_SHOW_FLOATING_MSG";
    public const string EVENT_UI_SHOW_MSGBOX = "EVENT_UI_SHOW_MSGBOX";                   // 显示确认
    public const string EVENT_UI_REFRESH_SKILL_POINT = "EVENT_UI_REFRESH_SKILL_POINT";           // 刷新技能点
    public const string EVENT_UI_REFRESH_SP = "EVENT_UI_REFRESH_SP";                    // 刷新体力
    public const string EVENT_UI_UPDATE_ITEMTIP_INFO = "EVENT_UI_UPDATE_ITEMTIP_INFO";           // 刷新物品tip界面的数据
    public const string EVENT_UI_ITEM_EQUIP_SUBVIEW_CLOSE = "EVENT_UI_ITEM_EQUIP_SUBVIEW_CLOSE";      // 装备合成和装备获取界面已关闭
    public const string EVENT_SHOP_REFRESH_SHOP = "EVENT_SHOP_REFRESH_SHOP";    // 刷新商店
    public const string EVENT_UI_REFRSEH_HERO_ATTR = "EVENT_UI_REFRSEH_HERO_ATTR"; // 刷新英雄数据（等级）
}

