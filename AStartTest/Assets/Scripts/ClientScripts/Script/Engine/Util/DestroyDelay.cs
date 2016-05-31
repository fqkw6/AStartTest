using UnityEngine;
using System.Collections;

// 一定时间后销毁
public class DestroyDelay : MonoBehaviour
{
    public float time;

    void Start ()
	{
	    Destroy(gameObject, time);
	}
}
