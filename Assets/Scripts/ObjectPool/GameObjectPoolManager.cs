using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PooledGameObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new();
}

/// <summary>
/// 实现对象池的创建，存储，回收
/// </summary>
public class GameObjectPoolManager : MonoBehaviour
{
    public static List<PooledGameObjectInfo> objectPools = new();
    private static GameObject _objectPoolEmptyHolder;
    private static GameObject _gameObjectsEmpty;
    private static GameObject _particleSystemsEmpty;
    private static GameObject _bulletsEmpty;
    private static GameObject _timerEmpty;

    public enum PoolType
    {
        ParticleSystem,
        GameObject,
        Bullets,
        Timer,
        None
    }

    private void Awake()
    {
        SetupEmpties();
    }

    private void SetupEmpties()
    {
        _objectPoolEmptyHolder = new GameObject("Pool Object");
        _gameObjectsEmpty = new GameObject("GameObjects");
        _gameObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _particleSystemsEmpty = new GameObject("Particle Effect");
        _particleSystemsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _bulletsEmpty = new GameObject("Bullets");
        _bulletsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

        _timerEmpty = new GameObject("Timers");
        _timerEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="poolType"></param>
    /// <returns>对应类型的父物体</returns>
    private static GameObject GetParentObject(PoolType poolType)
    {
        return poolType switch
        {
            PoolType.ParticleSystem => _particleSystemsEmpty,
            PoolType.GameObject => _gameObjectsEmpty,
            PoolType.Bullets => _bulletsEmpty,
            PoolType.Timer => _timerEmpty,
            PoolType.None => null,
            _ => null,
        };
    }

    public static GameObject SpawnObject(string goName)
    {
        //在已经存在的对象池查找对象
        PooledGameObjectInfo pool = objectPools.Find(p => p.LookupString == goName);

        //如果该对象池不存在，即为第一次生成该对象，创建对象池
        if (pool == null)
        {
            pool = new PooledGameObjectInfo
            {
                LookupString = goName
            };
            objectPools.Add(pool);
        }

        //从对象池中获取对象
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj != null)
        {
            //对象池中存有对象时
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);   //Call OnEnable()
        }
        else
        {
            //首次生成对象或对象池已空
            //生成对象
            spawnableObj = new GameObject(goName);
        }
        return spawnableObj;
    }

    public static GameObject SpawnObject(string goName, PoolType poolType)
    {
        //在已经存在的对象池查找对象
        PooledGameObjectInfo pool;
        if (poolType == PoolType.Timer)
            pool = objectPools.Find(p => p.LookupString == "Timer");
        else
            pool = objectPools.Find(p => p.LookupString == goName);

        //如果该对象池不存在，即为第一次生成该对象，创建对象池
        if (pool == null)
        {
            pool = new();
            if (poolType == PoolType.Timer)
                pool.LookupString = "Timer";
            else
                pool.LookupString = goName;

            objectPools.Add(pool);
        }

        //从对象池中获取对象
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj != null)
        {
            //对象池中存有对象时
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);   //Call OnEnable()
        }
        else
        {
            //首次生成对象或对象池已空
            //生成对象
            spawnableObj = new GameObject(goName);
        }
        return spawnableObj;
    }

    /// <summary>
    /// 使用对象池方法替代原有的Instantiate方法，提高运行效率
    /// </summary>
    /// <param name="objToSpawn"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="rotation"></param>
    /// <param name="poolType"></param>
    /// <returns></returns>
    public static GameObject SpawnObject(GameObject objToSpawn, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.None)
    {
        //在已经存在的对象池查找对象
        PooledGameObjectInfo pool = objectPools.Find(p => p.LookupString == objToSpawn.name);

        //如果该对象池不存在，即为第一次生成该对象，创建对象池
        if (pool == null)
        {
            pool = new PooledGameObjectInfo
            {
                LookupString = objToSpawn.name
            };
            objectPools.Add(pool);
        }

        //从对象池中获取对象
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj != null)
        {
            //对象池中存有对象时
            spawnableObj.transform.SetPositionAndRotation(position, rotation);
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);   //Call OnEnable()
        }
        else
        {
            //首次生成对象或对象池已空
            //生成对象
            spawnableObj = Instantiate(objToSpawn, position, rotation);
            //获取父物体便于管理
            GameObject parentObject = GetParentObject(poolType);
            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }
        return spawnableObj;
    }

    public static GameObject SpawnObject(GameObject objToSpawn, Vector3 position, Quaternion rotation, Transform parentTransform)
    {
        //在已经存在的对象池查找对象
        PooledGameObjectInfo pool = objectPools.Find(p => p.LookupString == objToSpawn.name);

        //如果该对象池不存在，即为第一次生成该对象，创建对象池
        if (pool == null)
        {
            pool = new PooledGameObjectInfo();
            pool.LookupString = objToSpawn.name;
            objectPools.Add(pool);
        }

        //从对象池中获取对象
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj != null)
        {
            //对象池中存有对象时
            spawnableObj.transform.SetPositionAndRotation(position, rotation);
            spawnableObj.transform.SetParent(parentTransform);
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }
        else
        {
            //首次生成对象或对象池已空
            //生成对象
            spawnableObj = Instantiate(objToSpawn, position, rotation, parentTransform);
        }
        return spawnableObj;
    }

    /// <summary>
    /// 回收对象至对象池中，若不存在对象池则return
    /// </summary>
    /// <param name="obj">被回收的对象</param>
    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name[0..^7]; //删除对象名字带有的 (clone)

        PooledGameObjectInfo pool = objectPools.Find(p => p.LookupString == goName);

        if (pool != null)
        {
            if (obj.activeSelf != false)
            {
                obj.SetActive(false);
            }
            pool.InactiveObjects.Add(obj);
        }
        else
        {
            Debug.Log("尝试回收一个不存在对象池的对象：" + obj.name);
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="poolType"></param>
    public static void ReturnObjectToPool(GameObject obj, PoolType poolType)
    {
        string goName;
        if (poolType == PoolType.Timer)
        {
            obj.name = "Timer";
            goName = obj.name;
        }
        else goName = obj.name[0..^7]; //删除对象名字带有的 (clone)

        PooledGameObjectInfo pool = objectPools.Find(p => p.LookupString == goName);

        if (pool != null)
        {
            if (obj.activeSelf != false)
            {
                obj.SetActive(false);
            }
            pool.InactiveObjects.Add(obj);
        }
        else
        {
            Debug.Log("尝试回收一个不存在对象池的对象：" + obj.name);
            return;
        }
    }

    public static void DelyReturnToPoolBySeconds(GameObject obj, float t)
    {
        TimerManager.CreateTimeOut(t, () => { ReturnObjectToPool(obj); });
    }
}

