using System;

public interface IObjectPool<T>
{
    T Get();
    void Recycle(T item);
    void Cleanup();
    void OnPushHandle(T item);
    void OnPopHandle(T item);
}

public interface IPoolObjectItem
{
    /// <summary>
    /// 获取时调用
    /// </summary>
    void OnGetHandle();
    /// <summary>
    /// 回收时调用
    /// </summary>
    void OnRecycleHandle();
}