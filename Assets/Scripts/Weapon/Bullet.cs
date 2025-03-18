using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityGameObjectPool;

public class Bullet : IPoolableObject
{
    [SerializeField] protected float damage;
    public float Damage => damage;
    [SerializeField] protected float lifeTime = 2f;
    public float LifeTime => lifeTime;
    [SerializeField] protected float velocity;
    public float Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    [SerializeField] protected LayerMask damagableLayer;

    [Tooltip("检测碰撞的大小")]
    public float radius = 0.1f;

    protected Rigidbody rb;
    protected Timer lifeTimer;

    public bool EnableHitEffect = false;
    [SerializeField] protected IPoolableParticleSystem hitVFX;
    [SerializeField] protected DecalProjector decal;
    [SerializeField] protected float EffectLifeTime = 3f;

    public virtual void SetDamage(float damage) => this.damage = damage;

    public virtual void SetLifeTime(float time)
    {
        lifeTime = time;
        if (lifeTimer != null)
            TimerManager.RemoveTimer(lifeTimer);
        lifeTimer = TimerManager.CreateTimeOut(lifeTime, () => GameObjectPoolManager.RecycleItem(this));
    }

    public virtual void SetVelocity(float velocity)
    {
        rb.AddForce(transform.forward * velocity, ForceMode.Impulse);
        this.velocity = velocity;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update() { }

    // TODO: 依据教程修改子弹的移动方法和碰撞检测方法，使其不依赖于 Unity 的物理系统
    protected virtual void OnFixedUpdate()
    {
        ColliderDetect();
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage, DamageType.Bullet, transform.forward);
        }
        var contact = other.contacts[0];
        OnHit(contact.point, contact.normal);
        GameObjectPoolManager.RecycleItem(this);
    }

    public override void OnGet()
    {
        lifeTimer = TimerManager.CreateTimeOut(lifeTime, () => GameObjectPoolManager.RecycleItem(this));
    }

    public override void OnRecycle()
    {
        if (lifeTimer != null) TimerManager.RemoveTimer(lifeTimer);
        lifeTimer = null;
        rb.velocity = Vector3.one;
    }

    /// <summary>
    /// 调用以监测碰撞, 当检测到物体时调用 OnColliderDetect()
    /// </summary>
    protected virtual void ColliderDetect()
    {
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, 0, damagableLayer))
        {
            if (hit.collider.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, DamageType.Bullet, transform.forward);
            }

            OnHit(hit.point, hit.normal);
            GameObjectPoolManager.RecycleItem(this);
        }
    }

    protected virtual void OnHit(Vector3 point, Vector3 normal)
    {
        if (decal != null)
        {
            var obj = Instantiate(decal, point, Quaternion.LookRotation(-normal));
            obj.transform.localPosition -= normal * 0.01f; // 贴花偏移, 避免重叠时产生闪烁
        }

        if (hitVFX != null)
        {
            var obj = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(hitVFX, point, Quaternion.identity);
            obj.particle.Play();
            TimerManager.CreateTimeOut(EffectLifeTime, () => GameObjectPoolManager.RecycleItem(obj));
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
