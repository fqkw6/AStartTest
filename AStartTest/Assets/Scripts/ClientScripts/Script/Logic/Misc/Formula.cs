using UnityEngine;
using System.Collections;

// 公式
public class Formula
{
    public static int GetLevelUpQuickCost(int time)
    {
        return Mathf.CeilToInt(1.0f * time / GameConfig.QUICK_LEVELUP_TIME) * GameConfig.QUICK_LEVELUP_GOLD;
    }
}
