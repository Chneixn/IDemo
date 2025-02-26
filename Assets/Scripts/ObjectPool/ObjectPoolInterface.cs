using System;

public interface IObjectPool<T>
{
    T Get();
    void Recycle(T item);
    void Cleanup();
    void OnPushHandle(T item);
    void OnPopHandle(T item);
}

/// <summary>
/// Use for non unity object
/// </summary>
public interface IPoolObject
{
    void OnGet();
    void OnRecycle();
}