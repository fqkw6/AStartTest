using UnityEngine;
using System.Collections.Generic;


public class TimerController
{
    public delegate void TimerHandler();

    private class TimerInfo
    {
        public int timerId = 0;
        public int startTime = 0;
        public int delayTime = 0;
        public TimerHandler handler;
        public bool loop = false;
        public bool needToRemove = false;
    }

    private int _currentTimerId = 0;
    private int _currentTime = 0;
    private List<TimerInfo> _allTimerInfos = new List<TimerInfo>();

    // 添加计时器
    public int AddTimer(int time, TimerHandler callback, bool loop = false)
    {
        TimerInfo info = new TimerInfo();
        info.timerId = ++_currentTimerId;
        info.startTime = _currentTime;
        info.delayTime = time;
        info.handler = callback;
        info.loop = loop;
        info.needToRemove = false;

        _allTimerInfos.Add(info);
        return info.timerId;
    }

    // 移除计时器
    public void RemoveTimer(int timerId)
    {
        _allTimerInfos.RemoveAll((x) => x.timerId == timerId);
    }

    public void RemoveAllTimer()
    {
        _allTimerInfos.Clear();
    }

    public void OnTick(int interval)
    {
        bool needToRemove = false;
        _currentTime += interval;

        // 不要用foreach，因为执行回调的时候可能会添加或者删除timer
        for (int i = 0; i < _allTimerInfos.Count; ++i) {
            var item = _allTimerInfos[i];
            if (_currentTime >= item.startTime + item.delayTime) {
                if (item.handler != null) {
                    item.handler();
                    if (!item.loop) {
                        item.needToRemove = true;
                        needToRemove = true;
                    }
                }
            }
        }

        if (needToRemove) {
            _allTimerInfos.RemoveAll((x) => x.needToRemove == true);
        }
    }
}


// 时间管理  同步服务器时间  暂停游戏等
public class Timer
{
    private static int _currentTime = 0;

    public static void OnTick(int ms)
    {
        _currentTime += ms;
	    _timerController.OnTick(ms);
	}

    // 获取当前时间（类似于Time.time）
    public static float GetTime()
    {
        return _currentTime;
    }

    // 获取服务器时间，秒
    public static float GetServerTime()
    {
        return 0;
    }

    // 与服务器进行时间同步
    public static void RequestSyncTime()
    {

    }

    static TimerController _timerController = new TimerController();
    // 添加计时器
    public static int AddTimer(int time, TimerController.TimerHandler callback, bool loop = false)
    {
        return _timerController.AddTimer(time, callback, loop);
    }

    // 异步调用
    public static void AsyncCall(TimerController.TimerHandler callback)
    {
        _timerController.AddTimer(10, callback, false);
    }

    // 移除计时器
    public static void RemoveTimer(int timerId)
    {
        _timerController.RemoveTimer(timerId);
    }

    public static void RemoveAllTimer()
    {
        _timerController.RemoveAllTimer();
    }
}
