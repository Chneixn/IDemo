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

    public bool isPhysicBullet = false;
    protected Rigidbody rb;
    protected Timer lifeTimer;

    protected float startTime = 0;
    protected Vector3 startPosition;
    protected Vector3 startDirection;

    [Header("Hit Effect")]
    public bool EnableHitEffect = false;
    [SerializeField] protected IPoolableParticleSystem hitVFX;
    [SerializeField] protected IPoolableDecal decal;
    [SerializeField] protected float EffectLifeTime = 3f;

    #region Init
    public virtual void Initialization(Transform muzzle, float damage, float velocity, float lifeTime = 2f, bool isPhysicBullet = false)
    {
        transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
        startTime = Time.time;
        startPosition = transform.position;
        startDirection = transform.forward;
        this.damage = damage;
        this.velocity = velocity;

        this.lifeTime = lifeTime;
        if (lifeTimer != null)
            TimerManager.RemoveTimer(lifeTimer);
        lifeTimer = TimerManager.CreateTimeOut(lifeTime, () => GameObjectPoolManager.RecycleItem(this));

        if (isPhysicBullet)
        {
            this.isPhysicBullet = isPhysicBullet;
            if (TryGetComponent(out Rigidbody rb)) this.rb = rb;
            else rb = gameObject.AddComponent<Rigidbody>();
            rb.velocity = transform.forward * velocity;
        }
    }

    #endregion

    public override void OnGet() { }

    public override void OnRecycle()
    {
        if (lifeTimer != null) TimerManager.RemoveTimer(lifeTimer);
        lifeTimer = null;
        if (rb != null) rb.velocity = Vector3.zero;
    }

    protected virtual void Update() { }

    protected virtual void FixedUpdate()
    {
        if (!isPhysicBullet) UpdatePosition();
    }

    // Unity 物理检测
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (!isPhysicBullet) return;
        if (other.collider.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage, DamageType.Bullet, transform.forward);
        }
        var contact = other.contacts[0];
        OnHit(contact.point, contact.normal);
        GameObjectPoolManager.RecycleItem(this);
    }

    protected virtual void UpdatePosition()
    {
        var curTime = Time.time - startTime;
        var preTime = curTime - Time.fixedDeltaTime;
        // var nextTime = curTime + Time.fixedDeltaTime;
        var curPoint = CalculatePosition(curTime);

        transform.position = curPoint;

        if (preTime > 0)
        {
            var prePoint = CalculatePosition(preTime);
            if (ColliderDetect(prePoint, curPoint)) GameObjectPoolManager.RecycleItem(this);
        }

        // var nextPoint = CalculatePoint(nextTime);
    }

    public virtual Vector3 CalculatePosition(float time)
    {
        // 可以重写该函数实现弹道, 添加风速影响等
        Vector3 point = startPosition + time * velocity * startDirection;
        return point;
    }

    protected virtual bool ColliderDetect(Vector3 prePoint, Vector3 nextPoint)
    {
        if (Physics.Linecast(prePoint, nextPoint, out RaycastHit hit, damagableLayer))
        {
            OnHit(hit.point, hit.normal, hit.collider.transform);
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, DamageType.Bullet, transform.forward);
            }
            return true;
        }
        return false;
    }

    protected virtual void OnHit(Vector3 point, Vector3 normal, Transform parent = null)
    {
        if (decal != null)
        {
            var obj = GameObjectPoolManager.GetItem<IPoolableDecal>(decal, point, Quaternion.LookRotation(-normal));
            if (parent != null) obj.transform.SetParent(parent);
            obj.transform.localPosition -= normal * 0.001f; // 贴花偏移, 避免重叠时产生闪烁
        }

        if (hitVFX != null)
        {
            var obj = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(hitVFX, point, Quaternion.identity);
            obj.particle.Play();
            TimerManager.CreateTimeOut(EffectLifeTime, () => GameObjectPoolManager.RecycleItem(obj));
        }
    }
}
