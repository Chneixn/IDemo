using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式基类 同场景内使用
/// </summary>
/// <typeparam name="T">子类</typeparam> <summary>
public class SameSceneSingleMono<T> : MonoBehaviour where T : SameSceneSingleMono<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance != null)
            Debug.LogError(this + "出现多个单例!");
        Instance = (T)this;
    }

    protected void OnDestroy()
    {
        Destroy();
    }

    public void Destroy()
    {
        Instance = null;
    }
}