using System;
using System.Collections.Generic;

public class ObjectPoolManager
{
    private readonly Dictionary<Type, object> _pools = new();   //记录所有池
    private const int DefaultPoolSize = 2;  //创建新池时的初始化对象数
    private const int DefaultPoolMaxSize = 500; //创建新池的最大池容量
    private static ObjectPoolManager _instance;

    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ObjectPoolManager();
            return _instance;
        }
    }

    private ObjectPool<T> GetPool<T>(int poolSize = DefaultPoolSize) where T : new()
    {
        var type = typeof(T);
        if (!_pools.TryGetValue(type, out var pool))
        {
            pool = new ObjectPool<T>(poolSize, DefaultPoolMaxSize);
            _pools.Add(type, pool);
        }

        return pool as ObjectPool<T>;
    }

    public T GetItem<T>() where T : new()
    {
        return GetPool<T>().Get();
    }

    public void RecycleItem<T>(T item) where T : new()
    {
        GetPool<T>().Recycle(item);
    }
}

