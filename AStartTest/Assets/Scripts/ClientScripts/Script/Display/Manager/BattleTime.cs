using UnityEngine;
using System.Collections;

// 战斗时间，将来很多地方会用到，所以从BattleController独立出来，防止BattleController被大量引用
public class BattleTime
{
    // 当前时刻
    public static int GetTime()
    {
        return Mathf.FloorToInt(Time.realtimeSinceStartup*1000);
    }

    // 获取客户端当前的时间戳，即逻辑时间
    public static int GetTimestamp()
    {
        return BattleController.Instance.ClientTurnIndex*GameConfig.FRAME_INTERVAL;
    }
}
