using UnityEngine;
using System.Collections;

// 逝去的时间
public struct ElapseTime
{
    private float _elapseTime;  // 倒计时时间
    private float _syncTime;    // 同步时间

    public void SetTime(float time)
    {
        _elapseTime = time;
        _syncTime = Time.realtimeSinceStartup;
    }

    public void SetTimeMilliseconds(long time)
    {
        SetTime(Mathf.CeilToInt(time / 1000.0f));
    }

    // 重置
    public void Reset()
    {
        _elapseTime = 0;
        _syncTime = 0;
    }

    // 是否已经启动
    public bool IsValid()
    {
        return _syncTime > 0;
    }

    public float GetTime()
    {
        if (Mathf.Abs(_syncTime) <= 0.1f) {
            return 0;
        }

        return _elapseTime + (Time.realtimeSinceStartup - _syncTime);
    }

    public override string ToString()
    {
        return Utils.GetCountDownTime(GetTime());
    }
}
