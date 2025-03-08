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

    /// <summary>
    /// 获取可池化物品
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="poolObject">预制体搭载的脚本</param>
    /// <param name="position">生成的世界位置</param>
    /// <param name="quaternion"></param>
    /// <returns></returns>
    public static T GetItem<T>(IPoolableObject poolObject, Vector3 position, Quaternion quaternion) where T : IPoolableObject
    {
        GameObject obj = poolObject.gameObject;
        string name = obj.name + "(Clone)";
        T result;
        if (_pools.TryGetValue(name, out GameObjectPool pool))
        {
            result = pool.Get().GetComponent<T>();
        }
        else
        {
            result = CreatPool(obj, name, 0, 500).Get().GetComponent<T>();
        }
        result.transform.SetPositionAndRotation(position, quaternion);
        result.gameObject.SetActiveSafe(true);
        result.OnGet();
        return result;
    }

    public static bool RecycleItem(IPoolableObject item)
    {
        if (item == null) return false;

        if (_pools.TryGetValue(item.name, out var pool))
        {
            if (pool.Recycle(item.gameObject))
            {
                item.OnRecycle();
                return true;
            }
        }
        else
        {
            var newPool = CreatPool(item.gameObject, item.name + "(Clone)", 0, 500);
            newPool.Recycle(item.gameObject);
            item.OnRecycle();
            return true;
        }
        return false;
    }

}
