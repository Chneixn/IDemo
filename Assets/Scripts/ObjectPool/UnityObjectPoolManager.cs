using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnityObjectPoolManager
{
    public delegate T LoadFuncDel<out T>(string path);
    public LoadFuncDel<UnityEngine.Object> LoadFuncDelegate;

    private readonly Dictionary<string, UnityObjectPool> _pools = new();
    private const int DefaultPoolSize = 50;
    private const int DefaultPoolMaxSize = 500;

    private UnityObjectPool CreatPool(UnityEngine.Object obj, string poolName,
        Func<UnityEngine.Object> objectFactory, int initalPoolSize = 0, int maxPoolSize = 500,
        Action<UnityEngine.Object> onPushHandle = null, Action<UnityEngine.Object> onPopHandle = null)
    {
        var pool = new UnityObjectPool(obj, objectFactory, initalPoolSize, maxPoolSize, onPushHandle, onPopHandle);
        //_pools[poolName] = pool;
        _pools.Add(poolName, pool);
        return pool;
    }

    public T GetItem<T>(string itemName) where T : UnityEngine.Object
    {
        T result = null;

        if (_pools.TryGetValue(itemName, out UnityObjectPool pool))
        {
            result = pool.Get() as T;
        }
        else if (LoadFuncDelegate != null)
        {
            result = CreatPool(LoadFuncDelegate(itemName) as T, itemName, null, 0, 500).Get() as T;
        }

        return result;
    }

    public bool RecycleItem(string itemName, UnityEngine.Object returnObject)
    {
        if (_pools.TryGetValue(itemName, out var pool))
        {
            pool.Recycle(returnObject);
            return true;
        }

        return false;
    }
}
