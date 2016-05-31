using UnityEngine;
using System.Collections;

// 场景中塔的占位标志
public class TowerPlaceholder : MonoBehaviour
{
    public bool IsEnemy;    // 是否是敌人，敌人的塔在上面
    public bool IsKingTower;    // 是否是国王塔，国王塔被击毁后游戏结束，玩家只有一个国王塔

	// Use this for initialization
	void Start () {
	
	}
}
