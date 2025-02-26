using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

// 在 Unity 中对 GameObject 使用对象池不应使用接口

public class GameObjectPoolManager : MonoBehaviour
{
    private static readonly Dictionary<string, GameObjectPool> _pools = new();
    private const int DefaultPoolSize = 50;
    private const int DefaultPoolMaxSize = 500;

    private static GameObjectPool CreatPool(GameObject itemPrefab, string itemName, int initialPoolSize = DefaultPoolSize, int maxPoolSize = DefaultPoolMaxSize)
    {
        var pool = new GameObjectPool(itemPrefab, initialPoolSize, maxPoolSize);
        _pools.Add(itemName, pool);
        return pool;
    }

    public static T GetItem<T>(IPoolableObject poolObject) where T : IPoolableObject
    {
        GameObject obj = poolObject.gameObject;
        string name = obj.name + "(Clone)";
        T result;
        if (_pools.TryGetValue(name, out GameObjectPool pool))
        {
            result = pool.Get() as T;
        }
        else
        {
            result = CreatPool(obj, name, 0, 500).Get() as T;
        }
        result.OnGet();
        return result;
    }
    public static bool RecycleItem(string itemName, IPoolableObject returnObject)
    {
        if (returnObject == null) return false;

        if (_pools.TryGetValue(itemName, out var pool))
        {
            pool.Recycle(returnObject.gameObject);
            returnObject.OnRecycle();
            return true;
        }
        return false;
    }

}
