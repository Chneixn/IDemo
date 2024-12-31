using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class UnityObjectPool : IObjectPool<Object>
{
    public Object ItemPrefab;
    public int InitialPoolSize = 0;
    public readonly int MaxPoolSize = 500;
    private int _curCount = 0;
    private readonly Func<Object> _objectFactory;
    private readonly Stack<Object> _objects;
    private Action<Object> _onPushHandle;
    private Action<Object> _onPopHandle;

    public UnityObjectPool(Object itemPrefab, Func<Object> objectFactory,
        int initialPoolSize = 0, int maxPoolSize = 500, Action<Object> onPushHandle = null,
        Action<Object> onPopHandle = null)
    {
        ItemPrefab = itemPrefab;
        _objectFactory = objectFactory;
        MaxPoolSize = maxPoolSize;
        InitialPoolSize = initialPoolSize;
        _onPushHandle = onPushHandle;
        _onPopHandle = onPopHandle;
        _objects = new Stack<Object>(MaxPoolSize);
        _curCount = 0;
        for (int i = 0; i < InitialPoolSize; i++)
        {
            Object obj = CreateObject();
            if (obj != null)
                _objects.Push(obj);
        }
    }

    private Object CreateObject()
    {
        if (_curCount >= MaxPoolSize) return default;
        var newObject = _objectFactory != null
            ? _objectFactory()
            : Object.Instantiate(ItemPrefab);
        if (newObject is GameObject obj)
        {
            if (obj.activeSelf != false)
            {
                obj.SetActive(false);
            }
        }
        _curCount++;
        _onPushHandle?.Invoke(newObject);
        OnPushHandle(newObject);
        return newObject;
    }

    public void Cleanup()
    {
        while (_objects.Count != 0)
        {
            var obj = _objects.Pop();
            Object.Destroy(obj);
        }
        _curCount = 0;
        return;
    }

    public Object Get()
    {
        Object item = _objects.Count == 0 ? CreateObject() : _objects.Pop();
        if (item is GameObject obj)
            obj.SetActive(true);
        _onPopHandle?.Invoke(item);
        OnPopHandle(item);
        return item;
    }

    public void OnPopHandle(Object item)
    {

    }

    public void OnPushHandle(Object item)
    {

    }

    public void Recycle(Object item)
    {
        if (item == null) return;
        if (!_objects.Contains(item))
        {
            if (item is GameObject obj)
            {
                if (obj.activeSelf != false)
                {
                    obj.SetActive(false);
                }
            }
            _onPopHandle?.Invoke(item);
            OnPopHandle(item);
            _objects.Push(item);
        }
    }
}
