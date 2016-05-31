using UnityEngine;
using System.Collections;

// 居民随机移动的移动点
public class WanderPoint : MonoBehaviour
{
    public int CitizenCount = 0;

	// Use this for initialization
	void Start () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
