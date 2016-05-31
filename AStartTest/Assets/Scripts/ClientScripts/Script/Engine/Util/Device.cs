using UnityEngine;
using System.Collections;

public class Device
{
    // 是否是手机平台
    public static bool IsMobilePlatform()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        return true;
#else
        return false;
#endif
    }
}
