using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

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
        _curCount = 0;
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
        if (item is GameObject obj)
        {
            obj.SetActive(true);
            obj.transform.parent = null;
        }
        return item;
    }

    private GameObject CreateObject()
    {
        if (_curCount >= MaxPoolSize) return default;

        var newObject = UnityEngine.Object.Instantiate(ItemPrefab);
        newObject.SetActiveSafe(true);
        _curCount++;
        return newObject;
    }

    public void Recycle(GameObject item)
    {
        if (item == null) return;
        if (!objects.Contains(item)) item.SetActiveSafe(false);
        objects.Push(item);
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
