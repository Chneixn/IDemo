using System;
using UnityEngine;

// TODO:接入对象池系统
public class NormalBulletSet : MonoBehaviour, IPoolObjectItem
{
    public float bulletDamage;
    [SerializeField] private float delyTimeToDestroy;
    private Timer timer;

    private void OnEnable()
    {
        timer = TimerManager.CreateTimeOut(delyTimeToDestroy, Destroy);
    }

    public void OnGetHandle()
    {
        //timer = TimerManager.CreateTimeOut(delyTimeToDestroy, Destroy);
    }

    public void OnRecycleHandle()
    {
        Destroy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(bulletDamage, DamageType.Bullet, transform.forward);
        }
        //UnityObjectPoolManager.RecycleItem(name, this.gameObject);
        timer.StopTimer();
    }

    private void Destroy()
    {
        timer = null;
        //GameObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
