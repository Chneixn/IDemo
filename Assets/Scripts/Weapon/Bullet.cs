using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bullet : IPoolableObject
{
    protected float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    [SetProperty("LifeTime")] protected float lifeTime = 2f;
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

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        ColliderDetect();
    }

    public virtual void SetRigiBodyVelocity(float velocity)
    {
        rb.AddForce(transform.forward * velocity, ForceMode.Impulse);
        this.velocity = velocity;
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

            OnHit(hit);
            GameObjectPoolManager.RecycleItem(this);
        }
    }

    protected virtual void OnHit(RaycastHit hit)
    {
        if (decal != null)
        {
            var obj = Instantiate(decal, hit.point, Quaternion.LookRotation(-hit.normal));
            obj.transform.localPosition -= hit.normal * 0.01f; // 贴花偏移, 避免重叠时产生闪烁
        }

        if (hitVFX != null)
        {
            var obj = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(hitVFX, hit.point, Quaternion.identity);
            obj.system.Play();
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
