using System;
using UnityEngine;

// TODO:接入对象池系统
public class Bullet : IPoolableObject
{
    public float bulletDamage;
    [SerializeField] private float delyTimeToDestroy = 2f;
    private Timer timer;

    public override void OnGet()
    {
        timer = TimerManager.CreateTimeOut(delyTimeToDestroy, () => GameObjectPoolManager.RecycleItem(name, this));
    }

    public override void OnRecycle()
    {
        timer = null;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(bulletDamage, DamageType.Bullet, transform.forward);
        }

        TimerManager.RemoveTimer(timer);
        GameObjectPoolManager.RecycleItem(name, this);
    }
}
