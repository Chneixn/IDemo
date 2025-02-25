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

// public static class MonoSingleMonoRegister<T> where T : MonoBehaviour
// {
//     private static T m_Instance;
//     private static readonly object m_Locker = new();

//     public static T Instance
//     {
//         get
//         {
//             if (m_Instance == null)
//             {
//                 lock (m_Locker)
//                 {
//                     if (m_Instance == null)
//                     {
//                         GameObject gameObject = new(typeof(T).Name + "[单例]");
//                         GameObject.DontDestroyOnLoad(gameObject);
//                         m_Instance = gameObject.AddComponent<T>();
//                     }
//                 }
//             }
//             return m_Instance;
//         }
//     }
// }

// 用例
// public class Example : MonoBehaviour
// {
//     public static Example Instance { get => SingleMonoRegister<Example>.Instance; }
// }