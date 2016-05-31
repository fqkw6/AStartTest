using UnityEngine;
using System;
using System.Collections;

// Actor的事件和计时器部分
public partial class Actor
{
    private EventController _eventController = new EventController();
    private TimerController _timerController = new TimerController();

    private void UpdateTimer()
    {
        if (_timerController != null) {
            _timerController.OnTick(GameConfig.FRAME_INTERVAL);
        }
    }

    // 清理事件
    public void ClearEvent()
    {
        _eventController.Clear();
    }

    // 注册事件 状态开始的时候注册
    public void AddEventListener(string eventType, Action handler)
    {
        _eventController.AddEventListener(eventType, handler);
    }

    public void AddEventListener<T>(string eventType, Action<T> handler)
    {
        _eventController.AddEventListener(eventType, handler);
    }

    public void AddEventListener<T, TU>(string eventType, Action<T, TU> handler)
    {
        _eventController.AddEventListener(eventType, handler);
    }

    public void RemoveEventListener(string eventType, Action handler)
    {
        _eventController.RemoveEventListener(eventType, handler);
    }

    public void RemoveEventListener<T>(string eventType, Action<T> handler)
    {
        _eventController.RemoveEventListener(eventType, handler);
    }

    public void RemoveEventListener<T, TU>(string eventType, Action<T, TU> handler)
    {
        _eventController.RemoveEventListener(eventType, handler);
    }

    public void TriggerEvent(string eventType)
    {
        _eventController.TriggerEvent(eventType);
    }

    public void TriggerEvent<T>(string eventType, T arg1)
    {
        _eventController.TriggerEvent(eventType, arg1);
    }

    public void TriggerEvent<T, TU>(string eventType, T arg1, TU arg2)
    {
        _eventController.TriggerEvent(eventType, arg1, arg2);
    }

    // 添加计时器
    public int AddTimer(int time, TimerController.TimerHandler callback, bool loop = false)
    {
        return _timerController.AddTimer(time, callback, loop);
    }

    // 在这个实体上设置一个计时器（ a:标示字符,b:执行函数 c:延迟执行的时间 ）
    public void AddTimer(string key, object b, float time)
    {

    }

    // 移除计时器
    public void RemoveTimer(int timerId)
    {
        _timerController.RemoveTimer(timerId);
    }

    // 移除所有计时器
    public void RemoveAllTimer()
    {
        _timerController.RemoveAllTimer();
    }

    // 在几帧后执行事件 相当于异步执行
    public void AddFrameEvent(Action action, int frames = 3)
    {
        StartCoroutine(DoAddFrameEvent(action, frames));
    }

    private IEnumerator DoAddFrameEvent(Action action, int frames)
    {
        int frame = 0;
        while (frame < frames) {
            yield return new WaitForEndOfFrame();
            ++frame;
        }

        if (action != null) {
            action();
        }
    }
}
