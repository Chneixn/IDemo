using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

namespace UnityGameObjectPool
{
    public class GameObjectPool
    {
        // Object 预制体
        public GameObject ItemPrefab;
        public int InitialPoolSize = 0;
        public readonly int MaxPoolSize = 500;
        private int _curCount = 0;
        // 空闲的 Object
        private readonly Stack<GameObject> objects;

        public GameObjectPool(GameObject itemPrefab, int initialPoolSize = 0, int maxPoolSize = 500)
        {
            ItemPrefab = itemPrefab;
            MaxPoolSize = maxPoolSize;
            InitialPoolSize = initialPoolSize;
            objects = new Stack<GameObject>(MaxPoolSize);
            _curCount = initialPoolSize;
            for (int i = 0; i < InitialPoolSize; i++)
            {
                GameObject obj = UnityEngine.Object.Instantiate(itemPrefab);
                if (obj != null)
                    objects.Push(obj);
            }
        }

        public GameObject Get()
        {
            GameObject item = objects.Count == 0 ? CreateObject() : objects.Pop();
            if (item.transform.parent != null) item.transform.parent = null;
            return item;
        }

        private GameObject CreateObject()
        {
            if (_curCount >= MaxPoolSize) return default;
            var obj = UnityEngine.Object.Instantiate(ItemPrefab);
            _curCount++;
            return obj;
        }

        /// <summary>
        /// 若返回false 可能有 1.item为空 2.item已经在池内
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Recycle(GameObject item)
        {
            if (item == null) return false;
            else if (!objects.Contains(item))
            {
                item.SetActiveSafe(false);
                objects.Push(item); // 当stack已满，会自动扩容，每次添加都会是O(n)操作
                return true;
            }
            return false;
        }


        public void Cleanup()
        {
            while (objects.Count != 0)
            {
                var obj = objects.Pop();
                UnityEngine.Object.Destroy(obj);
            }
            _curCount = 0;
            return;
        }
    }
}