using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace UnityGameObjectPool
{
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
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static T GetItem<T>(IPoolableObject poolObject, Vector3 position, Quaternion rotation) where T : IPoolableObject
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
            result.transform.SetPositionAndRotation(position, rotation);
            result.gameObject.SetActiveSafe(true);
            result.OnGet();
            return result;
        }

        public static bool RecycleItem(IPoolableObject item)
        {
            if (item == null) return false;
            if (!item.name.Contains("Clone")) item.name += "(Clone)";

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
                CreatPool(item.gameObject, item.name, 0, 500);
                RecycleItem(item);
            }
            return false;
        }

    }
}