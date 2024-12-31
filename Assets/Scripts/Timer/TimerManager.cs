using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    protected static readonly List<Timer> timers = new();      //计时器主存储
    protected static readonly List<Timer> timersCache = new();  //计时器缓存
    protected static Queue<Timer> freeTimers = new();         //闲置Timer对象
    protected static ulong timerId = 1000;
    protected static bool inited = false;

    private void Awake()
    {
        StartCoroutine(UpdateTimers());
    }

    private IEnumerator UpdateTimers()
    {
        while (true)
        {
            // 将即将计时的计时器从缓存取出
            if (timersCache.Count != 0)
            {
                timers.AddRange(timersCache);
                timersCache.Clear();
            }
            // 循环调用未结束的计时器
            foreach (Timer timer in timers)
            {
                if (!timer.IsEnd && timer.IsActive)
                    timer.UpdateTimer();
            }
            // 回收所有空闲计时器
            var needRemove = timers.FindAll(timer => timer.ReadyToRemove == true);
            if (needRemove.Count > 0)
                RemoveTimers(needRemove);

            yield return null;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            foreach (Timer timer in timers) timer.PauseTimer();
        }
        else
        {
            foreach (Timer timer in timers) timer.ConnitueTimer();

        }
    }

    public static Timer CreateTimer()
    {
        Timer timer = freeTimers.Count > 0 ? freeTimers.Dequeue() : new();
        timer.Init(++timerId);
        timersCache.Add(timer);
        return timer;
    }

    /// <summary>
    /// 单次计时器，立即计时，完成计时后自动移除
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="onCompleted"></param>
    public static Timer CreateTimeOut(float delay, Action onCompleted, Action<float> onUpdate = null)
    {
        Timer timer = CreateTimer();
        timer.StartTiming(delay, repeateTime: 1, onUpdate: onUpdate, onCompleted: onCompleted, removeOnEnd: true);
        return timer;
    }

    public static Timer CreatCountingTimer(Action<float> onUpdate = null, bool isIgnoreTimeScale = true)
    {
        Timer timer = CreateTimer();
        timer.StartCounting(onUpdate, isIgnoreTimeScale);
        return timer;
    }

    #region Get Timer methods

    public static Timer GetTimer(ulong timerID)
    {
        return timers.FirstOrDefault(timer => timer.ID == timerID);
    }

    /// <summary>
    /// 通过回调的使用对象获取定时器
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static List<Timer> GetTimers(object target)
    {
        List<Timer> tmp1 = timers.FindAll(t => { return t.OnCompleted.Target == target; });
        List<Timer> tmp2 = timers.FindAll(t => { return t.OnUpdate.Target == target; });
        return tmp1.Union(tmp2).ToList();
    }

    #endregion


    #region Remove Timer methods

    /// <summary>
    /// 通过ID移除计时器
    /// </summary>
    /// <param name="timerID"></param>
    /// <returns>是否有计时器被清除</returns>
    public static bool RemoveTimer(ulong timerID)
    {
        int index = timers.FindIndex(t => t.ID == timerID);

        if (index != -1)
        {
            Timer timer = timers[index];
            RemoveTimers(new List<Timer>() { timer });
            return true;
        }

        return false;
    }

    /// <summary>
    /// 通过计时器本身移除计时器，会在下一帧时移除
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    public static bool RemoveTimer(Timer timer)
    {
        return RemoveTimer(timer.ID);
    }

    /// <summary>
    /// 回收所有回调中包括此类型的计时器
    /// </summary>
    public static void RemoveTimer<T>()
    {
        var type = typeof(T);
        var className = type.FullName;

        var matchTimers = timers.FindAll(t =>
        {
            if (null != t.OnCompleted && null != t.OnCompleted.Target || null != t.OnUpdate && null != t.OnUpdate.Target)
            {
                var fullname = t.OnCompleted.Target.GetType().FullName;
                var currentClassNameClip = fullname.Split('+');
                if (currentClassNameClip.Length > 0)
                {
                    if (currentClassNameClip[0] == className)
                    {
                        return true;
                    }
                }
            }
            return false;
        });

        RemoveTimers(matchTimers);
    }

    /// <summary>
    /// 批量清理定时器
    /// </summary>
    /// <param name="timersToRemove"></param>
    public static void RemoveTimers(List<Timer> timersToRemove)
    {
        timersToRemove?.ForEach(t =>
        {
            t.ClearTimer();
            timers.Remove(t);
            freeTimers.Enqueue(t);
        });
    }

    #endregion
}
