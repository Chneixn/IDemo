using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool<T> : IObjectPool<T> where T : new()
{
    private readonly Queue<T> _objects;  //使用队列存储对象
    private readonly int InitialPoolSize = 0;    //对象池生成时初始对象数
    private readonly int MaxPoolSize = 200; //对象池最大容量
    private int _curCount = 0;  //当前对象池容量

    public ObjectPool(int initialPoolSize = 0, int maxPoolSize = 200)
    {
        _objects = new Queue<T>();
        MaxPoolSize = maxPoolSize;
        InitialPoolSize = initialPoolSize;
        _curCount = 0;
        for (int i = 0; i < InitialPoolSize; i++)
        {
            T obj = CreateObject();
            if (obj != null)
                _objects.Enqueue(obj);
        }
    }

    /// <summary>
    /// 从对象池获取新对象，无则自动创建
    /// </summary>
    /// <returns></returns>
    public T Get()
    {
        T item = _objects.Count == 0 ? CreateObject() : _objects.Dequeue();
        if (item != null)
            OnPopHandle(item);
        return item;
    }

    public void Recycle(T item)
    {
        if (item == null)
            return;
        if (!_objects.Contains(item))
        {
            OnPushHandle(item);
            _objects.Enqueue(item);
        }
    }

    /// <summary>
    /// 自动创建
    /// </summary>
    /// <returns></returns>
    private T CreateObject()
    {
        if (_curCount >= MaxPoolSize) return default;
        var newObject = new T();
        _curCount++;
        OnPushHandle(newObject);
        return newObject;
    }

    /// <summary>
    /// 清除对象池
    /// </summary>
    public void Cleanup()
    {
        if (_objects.Count != 0)
        {
            _objects.Clear();
        }
        _curCount = 0;
        return;
    }

    public void OnPushHandle(T item)
    {
        if (item is IPoolObjectItem iPoolItem)
            iPoolItem.OnRecycleHandle();
        if (item is IList list)
            list.Clear();
        else if (item is IDictionary dictionary)
            dictionary.Clear();
    }

    public void OnPopHandle(T item)
    {
        if (item is IPoolObjectItem iPoolItem)
            iPoolItem.OnGetHandle();
    }
}
