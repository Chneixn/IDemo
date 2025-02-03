using System;
using UnityEngine;

public class Timer
{
    ulong id;
    Action<float> onUpdate;
    Action onCompleted;

    public Action<float> OnUpdate => onUpdate;
    public Action OnCompleted => onCompleted;

    #region 计时器设定
    float timeTarget;               //计时器目标时间
    float timeStart;                //开始计时的时刻
    float offsetTime;               //计时偏移值
    bool isTimerActive = false;     //是否正在计时
    bool isEnd = false;             //计时是否结束（标记从暂停恢复后需要继续计时的计时器）
    bool isIgnoreTimeScale = true;  //是否忽略时间速率
    int repeateTime;                //重复次数（1则只执行一次，小于等于0则无限循环）
    int repeateCount = 0;           //循环次数<=0则无限循环，默认无限循环
    float now;                      //正计时已计时间
    float _pauseTime;               //记录暂停时刻
    float downNow;                  //倒计时剩余时间
    bool isDownTimer = false;       //是否是倒计时
    bool removeOnEnd = false;       //是否在计时结束后移除计时器
    bool readyToRemove = false;     //可以移除的标记
    public bool ReadyToRemove => readyToRemove;
    #endregion

    #region Property 属性

    /// <summary>
    /// 剩余时间的百分比
    /// </summary>
    /// <value></value>
    public float Progress
    {
        get { return timeTarget != -1 ? 1.0f - Mathf.Clamp01(now / timeTarget) : 1f; }
    }

    /// <summary>
    /// 已计时时间
    /// </summary>
    /// <value></value>
    public float Elapsed
    {
        get
        {
            return timeTarget != -1 ? Mathf.Clamp(now, 0, timeTarget) : now;
        }
    }

    /// <summary>
    /// 剩余未计时时间
    /// </summary>
    /// <value></value>
    public float Remaining
    {
        get { return timeTarget != -1 ? Mathf.Clamp(timeTarget - now, 0, timeTarget) : Mathf.Infinity; }
    }

    public int RepeateLeft
    {
        get { return repeateCount; }
    }

    public ulong ID
    {
        get { return id; }
    }

    public bool IsActive
    {
        get { return isTimerActive; }
    }

    public bool IsEnd
    {
        get { return isEnd; }
    }

    private float TimeNow   //当前系统时刻
    {
        get { return isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time; }
    }

    #endregion

    public void Init(ulong id)
    {
        this.id = id;
    }

    /// <summary>
    /// 开始计时(repeateTime为0时无限循环,默认为0)
    /// </summary>
    /// <param name="timeTarget">计时时间长度</param>
    /// <param name="isDownTimer">是否倒计时</param>
    /// <param name="isIgnoreTimeScale">是否忽略Unity时间倍率</param>
    /// <param name="repeateTime">循环次数小于等于0则无限循环</param>
    /// <param name="offsetTime">延迟时间</param>
    /// <param name="isTimerActive">是否立刻开始计时</param>
    /// <param name="onCompleted">完成时的回调</param>
    /// <param name="onUpdate">每帧更新时的回调</param>
    /// <param name="removeOnEnd">计时器结束后回收</param>
    public void StartTiming(float timeTarget, bool isDownTimer = false,
        bool isIgnoreTimeScale = true, int repeateTime = 0,
        float offsetTime = 0f, bool isTimerActive = true,
        Action onCompleted = null, Action<float> onUpdate = null, bool removeOnEnd = false)
    {
        this.timeTarget = timeTarget;
        this.isIgnoreTimeScale = isIgnoreTimeScale;
        this.repeateTime = repeateTime;
        this.offsetTime = offsetTime;
        this.isTimerActive = isTimerActive;
        this.isDownTimer = isDownTimer;
        this.removeOnEnd = removeOnEnd;

        this.onCompleted = onCompleted;
        this.onUpdate = onUpdate;

        timeStart = TimeNow;
        isEnd = false;
    }

    public void StartTimeOut(float delay, Action onCompleted, Action<float> onUpdate = null, bool isIgnoreTimeScale = true)
    {
        StartTiming(delay, false, isIgnoreTimeScale, 1, 0, true, onCompleted, onUpdate, false);
    }

    /// <summary>
    /// 开始持续正向计时（仅计时）
    /// </summary>
    /// <param name="isIgnoreTimeScale"></param>
    public void StartCounting(Action<float> onUpdate = null, bool isIgnoreTimeScale = true)
    {
        StartTiming(-1, false, isIgnoreTimeScale, 0, 0, true, null, onUpdate, false);
    }

    /// <summary>
    /// 提前停止计时器, 立即触发 onCompleted 事件
    /// 若已经停止, 返回值为-1
    /// </summary>
    /// <returns>已计时的时间</returns>
    public float StopTimer()
    {
        if (!isTimerActive) return -1f;
        onCompleted?.Invoke();
        EndTimer();
        return now;
    }

    public void UpdateTimer()
    {
        now = TimeNow - offsetTime - timeStart;
        downNow = timeTarget - now;

        onUpdate?.Invoke(isDownTimer ? downNow : now);

        if (now >= timeTarget && timeTarget > 0)
        {
            repeateCount++;
            onCompleted?.Invoke();
            repeateTime--;
            if (repeateTime == 0)
                EndTimer();
            else
                ReStartTimer();
        }
    }

    /// <summary>
    /// 计时结束
    /// </summary>
    public void EndTimer()
    {
        isTimerActive = false;
        isEnd = true;
        if (removeOnEnd) readyToRemove = true;
    }

    /// <summary>
    /// 清除计时器信息
    /// </summary>
    public void ClearTimer()
    {
        id = 0;
        timeTarget = -1;
        offsetTime = 0;
        isTimerActive = false;
        isIgnoreTimeScale = false;
        isEnd = true;
        repeateCount = 0;
        onUpdate = null;
        onCompleted = null;
        readyToRemove = false;
        removeOnEnd = false;

        onCompleted = null;
        onUpdate = null;
    }

    /// <summary>
    /// 暂停计时
    /// </summary>
    /// <returns>已计时的时间</returns>
    public float PauseTimer()
    {
        if (isTimerActive)
        {
            isTimerActive = false;
            _pauseTime = TimeNow;
        }

        return now;
    }

    /// <summary>
    /// 在暂停之后恢复计时
    /// </summary>
    public void ConnitueTimer()
    {
        if (!isTimerActive && !isEnd)
        {
            offsetTime += (TimeNow - _pauseTime);
            isTimerActive = true;
        }
    }

    /// <summary>
    /// 重新计时
    /// </summary>
    public void ReStartTimer()
    {
        timeStart = TimeNow;
        offsetTime = 0;
    }

    /// <summary>
    /// 更改目标时间
    /// </summary>
    /// <param name="newTarget"></param>
    public void ChangeTargetTime(float newTarget)
    {
        timeTarget = newTarget;
        timeStart = TimeNow;
    }
}