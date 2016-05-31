using UnityEngine;
using System.Collections.Generic;

public class GameConfig
{
    public static int GAME_TIME = 3*60*1000;    // 比赛时长 毫秒
    public static int DOUBLE_TIME = 60*1000;    // 剩余多少毫秒开始双倍圣水
    public static int OVER_TIME = 60*1000;  // 加时赛时间
    public static int MAX_CARD_COUNT = 10;  // 卡组最大数量
    public static int MAX_HANDCARD_COUNT = 4;   // 最大手牌数量
    public static int GRID_CELL_X = 18; // 地图格子数目
    public static int GRID_CELL_Y = 34;
    public static int FRAME_INTERVAL = 100;   // 逻辑帧间隔 毫秒
    public static int START_MANA = 5;   // 初始圣水数量
    public static int MANA_REGEN = 10; // 每逻辑帧获得多少圣水（圣水量是0-1000的值，实际显示的数目为0-10）
    public static int MANA_MUL = 100;   // 配置的圣水量到实际圣水量的倍率
    public static int MANA_MAX = 1000;  // 最大圣水量

    public static float HP_LVL_MUL = 1.2f;  // 血量受等级的影响倍率
    public static float ATK_LVL_MUL = 1.2f;    // 攻击受等级影响的倍率

    public static int KING_TOWER_ID = 1001;   // 国王塔配置ID
    public static int TOWER_ID = 1002;    // 公主塔配置ID
    public static int MIRROR_ID = 207;  // 镜像法术的配置id

    public static int BASE_HERO_ID = 10000;     // ID大于这个的单位为英雄
    public static float ARRIVE_DISTANCE = 0.5f;
    public static float ATTACK_INTERVAL = 5;    // 默认速度为1的情况下，5秒钟攻击一次

    public static int MAX_PVE_HERO_COUNT = 3;   // PVE 最大的上场英雄数目
    public static int MAX_HERO_STAR = 5;        //最大五星
    public static int MAX_HERO_QUALITY = 10;    // 进阶的最大阶数

    public static int MAX_SKILL_POINT = 10;     // 最大保留10个技能点
    public static int SKILL_POINT_GET_INTERVAL = 60;   // 每10分钟获得一个技能点(TODO改成600，现在测试时用一分钟来测试)
    public static int SKILL_UNLOCK2 = 20;        // 解锁第二个技能需要的等级
    public static int SKILL_UNLOCK3 = 40;
    public static int SKILL_UNLOCK4 = 70;

    public static int SCREEN_WIDTH = 1136;
    public static int SCREEN_HEIGHT = 640;

    public static int QUICK_LEVELUP_GOLD = 1;           // 每20秒，消耗1黄金
    public static int QUICK_LEVELUP_TIME = 20;
    public static int QUICK_LEVELUP_FREE_TIME = 300;        // 5分钟内免费
    public static float CITY_BUILDING_CANCEL_BACK = 0.5f;   // 取消升级建筑返还资源比例
    public static int BUY_MONEY_UNIT_COST = 1000;           //每一元宝可以购买多少黄金
    public static int BUY_WOOD_UNIT_COST = 1000;            //每一元宝可以购买多少木材
    public static int BUY_STONE_UNIT_COST = 1000;           //每一元宝可以购买多少石头

    public static int MIN_ACCOUNT_SIZE = 4;
    public static int MAX_ACCOUNT_SIZE = 32;
    public static int MIN_PASSWORD_SIZE = 4;
    public static int MAX_PASSWORD_SIZE = 12;
    public static int MIN_NAME_SIZE = 4;
    public static int MAX_NAME_SIZE = 12;

    public static int ITEM_CONFIG_ID_MONEY = 1;
    public static int ITEM_CONFIG_ID_WOOD = 2;
    public static int ITEM_CONFIG_ID_STONE = 3;
    public static int ITEM_CONFIG_ID_GOLD = 4;

    public static int ITEM_CONFIG_ID_EXP_1 = 11;
    public static int ITEM_CONFIG_ID_EXP_2 = 12;
    public static int ITEM_CONFIG_ID_EXP_3 = 13;
    public static int ITEM_CONFIG_ID_EXP_4 = 14;
    public static int ITEM_CONFIG_ID_EXP_5 = 15;

    public static int BUY_SP_COST = 50; // 50元宝获得120体力，可以超过上限
    public static int BUY_SP_GET = 120;

    public static int PRODUCE_REWARD_INTERVAL = 10;   // 资源收获间隔
    public static int WORLD_CITY_REFRESH_TIME = 12 * 3600;    // 资源城距离创建多长时间可以刷新
    public static int WORLD_RES_TOWN_CONQUER_TIME = 24 * 3600;    // 资源城最多可以占领多少时间

    public static int SP_GET_INTERVAL = 60; // 每隔1分钟恢复1点体力

    public static int LUCK_DRAW_MAX_FREE_COUNT = 5; // 酒馆抽奖中最大的银两免费次数
    public static float LUCK_DRAW_MONEY_FREE_CD = 5 * 60; // 免费抽奖的cd时间
    public static int LUCK_DRAW_GOLD_COUNT = 10;    // 元宝抽奖中必得武将的累计次数
    public static float LUCK_DRAW_GOLD_FREE_CD = 48*3600;// 元宝抽奖的cd时间
    public static int LUCK_DRAW_MONEY_1_COST = 10000;
    public static int LUCK_DRAW_MONEY_10_COST = 90000;
    public static int LUCK_DRAW_GOLD_1_COST = 100;
    public static int LUCK_DRAW_GOLD_10_COST = 900;
    public static int LUCK_DRAW_MONEY_ITEM_ID = 11; // 金币抽卡获得的经验书id
    public static int LUCK_DRAW_GOLD_ITEM_ID = 13;

    public static int PVE_MAX_QUICK_FIGHT_COUNT = 10; // pve最大扫荡次数
    public static int PVE_RESET_FIGHT_COUNT_COST = 50;  // pve重置挑战次数的消耗

    public static int PVP_GOLD_PER_ATTACK_COUNT = 10;   // 每个购买的攻击次数消耗的钻石
    public static int PVP_MAX_ATTACK_COUNT = 5; // 每天最大挑战次数
    public static int PVP_SCORE_WIN_ADD = 2;    // 胜利积分奖励
    public static int PVP_SCORE_LOSE_ADD = 1; // 失败积分奖励

    public static int SHOP_REFRESH_COST = 5;    // 手动刷新商店消耗的元宝基数
    public static int SHOP_REFRESH_MULTIPLE = 2;// 每次刷新是上一次的多少倍

    public static int MAIL_MAX_COST = 50;

    public static int CHANGE_NAME_COST = 100;   // 修改名字的钻石消耗

    public static string SERVER_IP = "112.74.131.129";  // 游戏服务器的ip和端口
    public static int SERVER_PORT = 9113;

    public static readonly string LOCAL_PATH = System.IO.Path.Combine(Application.persistentDataPath, "update");      // 本地下载的资源路径
    public static readonly string VERSION_FILE = "version.txt";
    public static readonly string FILE_LIST = "filelist.txt";
    public static readonly string ASSETBUNDLE = ".data";

    public static string ResourceListPath = "resourcelist.txt";
    public static List<string> DefaultAssetBundle = new List<string> { "shader", "config" };
}