using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : IPoolableObject
{
    protected float damage;
    public float Damage { get; set; }
    protected float lifeTime = 2f;
    public float LifeTime
    {
        get { return lifeTime; }
        set
        {
            lifeTime = value;
            if (lifeTimer != null)
                TimerManager.RemoveTimer(lifeTimer);
            lifeTimer = TimerManager.CreateTimeOut(lifeTime, () => GameObjectPoolManager.RecycleItem(this));
        }
    }
    protected float velocity;

    public float Velocity
    {
        get { return velocity; }
        set
        {
            velocity = value;
            SetRigiBodyVelocity(Velocity);
        }
    }

    [Tooltip("检测碰撞的大小")]
    public float radius = 0.1f;
    protected Collider[] detects;
    protected const int CacheSize = 32;
    [SerializeField] protected LayerMask damagableLayer;
    protected Timer lifeTimer;
    protected new Rigidbody rigidbody;

    protected virtual void Awake() => detects = new Collider[CacheSize];

    protected virtual void Update()
    {
        ColliderDetect();
    }

    public override void OnGet()
    {
        rigidbody = GetComponent<Rigidbody>();
        lifeTimer = TimerManager.CreateTimeOut(lifeTime, () => GameObjectPoolManager.RecycleItem(this));
    }

    public override void OnRecycle()
    {
        if (lifeTimer != null) TimerManager.RemoveTimer(lifeTimer);
        lifeTimer = null;
    }

    protected virtual void SetRigiBodyVelocity(float v)
    {
        if (rigidbody == null) return;
        rigidbody.velocity = transform.forward * v;
        velocity = v;
    }

    /// <summary>
    /// 调用以监测碰撞, 当检测到物体时调用 OnColliderDetect()
    /// </summary>
    protected virtual int ColliderDetect()
    {
        var count = Physics.OverlapSphereNonAlloc(transform.position, radius, detects, damagableLayer);
        if (count > 0) OnColliderDetect(count);
        return count;
    }

    protected virtual void OnColliderDetect(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (detects[i].TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, DamageType.Bullet, transform.forward);
            }
            GameObjectPoolManager.RecycleItem(this);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (radius != 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
#endif

}
