using System;
using UnityEngine;
using System.Collections;

// 倒计时时间，设置倒计时秒数，获取当前实时的倒计时
public struct RemainTime
{
    private float _cdTime;  // 倒计时时间
    private float _syncTime;    // 同步时间

    // 设置时间
    public void SetTime(float time)
    {
        _cdTime = time;
        _syncTime = Time.realtimeSinceStartup;
    }

    public void SetTimeMilliseconds(long time)
    {
        SetTime(Mathf.CeilToInt(time / 1000.0f));
    }

    // 重置
    public void Reset()
    {
        _cdTime = 0;
        _syncTime = 0;
    }

    // 是否已经启动
    public bool IsValid()
    {
        return _syncTime > 0;
    }

    // 获取当前时间
    public float GetTime()
    {
        if (Mathf.Abs(_cdTime) < 0.1f) {
            return 0;
        }

        return Mathf.Max(0, _cdTime - (Time.realtimeSinceStartup - _syncTime));
    }


    public override string ToString()
    {
        return Utils.GetCountDownTime(GetTime());
    }
}
