using System.Collections;
using UnityEngine;

/// <summary>
/// 实现榴弹子弹；可弹跳；范围伤害；
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ExplosionBulletSet : IPoolableObject
{
    [Header("References目标绑定")]
    [SerializeField] private IPoolableParticleSystem explosionFX;
    [SerializeField] private AudioClip[] explosionSounds;
    [SerializeField] private LayerMask interactableLayer;
    private Rigidbody rb;
    private AudioSource audioSource;

    [Header("BulletBounciness子弹弹力")]
    [Range(0f, 1f)]
    [SerializeField] private float bounciness;        //子弹弹力
    [SerializeField] private bool useGravity;         //是否使用重力

    [Header("TrackSet追踪设置")]
    [SerializeField] private bool isTraceable;        //是否可追踪
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Transform target;

    [Header("爆炸设置")]
    [SerializeField] private float explosionDamage;   //爆炸伤害
    [SerializeField] private float explosionRange;    //爆炸范围
    [SerializeField] private bool enableExplosionForce;
    [SerializeField] private float explosionForce;    //爆炸推力 = 1200

    [Header("Lifetime子弹存在时间")]
    [SerializeField] private bool allowCollisionsToExplode;  //是否允许碰撞次数引爆
    [SerializeField] private int maxCollisions;       //最大弹跳次数
    [SerializeField] private float maxLifetime;       //最大存在时间
    [SerializeField] private bool explodeOnTuch;      //是否碰撞即爆炸
    [SerializeField] private float VFX_Time;

    int collisions = 0; //当前碰撞次数
    PhysicMaterial _physicsMaterial;

    readonly Collider[] colliders = new Collider[20];

    private void Start() => Setup();

    private void Setup()
    {
        //创建新物理材质，初始化材质设置
        _physicsMaterial = new PhysicMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        //PhysicMaterial.frictionCombine确定摩擦力的组合方式
        //PhysicMaterial.bounceCombine表面的弹性有多大？值为0时不会反弹。值为1将反弹而不会损失任何能量

        //应用物理材质设置
        GetComponent<Collider>().material = _physicsMaterial;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        //设置重力使用
        rb.useGravity = useGravity;
    }

    public override void OnGet()
    {
        StartCoroutine(DelyToExplode());
    }

    public override void OnRecycle()
    {
        audioSource.Stop();
        target = null;
        collisions = 0;
    }

    private void Update()
    {
        //追踪逻辑
        if (isTraceable)
        {
            if (target != null)
            {
                // 获取目标方向
                Vector3 _direction = (target.position - transform.position).normalized;
                //当前方向与目标方向的角度差
                float _angle = Vector3.Angle(transform.forward, _direction);
                float _needTime = _angle / turnSpeed;
                if (_needTime < 0.02f)
                    transform.forward = _direction;
                else
                    transform.forward = Vector3.Slerp(transform.forward, _direction, Time.deltaTime / _needTime).normalized;
            }
        }

    }

    private void FixedUpdate()
    {
        // if (isTraceable && target != null)
        // {
        //     //向前移动
        //     Vector3 _targetPos = target.position + moveSpeed * Time.fixedDeltaTime * transform.forward;
        //     rb.MovePosition(_targetPos);
        // }

        //爆炸方式
        //碰撞次数达到最大
        if (allowCollisionsToExplode)
        {
            if (collisions > maxCollisions && !isTraceable)
            {
                Explode();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //记录每次碰撞
        collisions++;

        //碰撞到敌人立刻爆炸，当碰撞爆炸设置为true时
        if (collision.collider.TryGetComponent(out IDamageable _) && explodeOnTuch) Explode();
    }

    private IEnumerator DelyToExplode()
    {
        yield return new WaitForSeconds(maxLifetime);
        Explode();
    }

    /// <summary>
    /// 实现子弹的爆炸特效、子弹的爆炸伤害、子弹的销毁
    /// </summary>
    private void Explode()
    {
        PlayVFX();
        PlayExplodeSound();

        // 爆炸范围检测
        Physics.OverlapSphereNonAlloc(transform.position, explosionRange, colliders, interactableLayer);
        foreach (var col in colliders)
        {
            // 爆炸伤害
            //if (enemies[i].GetComponent<Health>())
            //    enemies[i].GetComponent<Health>().TakeExplosionDamage(explosionDamage);

            //实现爆炸推动
            if (enableExplosionForce)
            {
                if (col.TryGetComponent(out Rigidbody rb))
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
                if (col.TryGetComponent(out CharacterControl cc))
                {
                    // 让你飞起来！
                    var dir = cc.Motor.CharacterTransformToCapsuleCenter - transform.position;
                    cc.InternalVelocity = dir.normalized * explosionForce;
                }
            }
        }

        GameObjectPoolManager.RecycleItem(name, this);
    }

    private void PlayVFX()
    {
        if (explosionFX != null)
        {
            var vfx = GameObjectPoolManager.GetItem<IPoolableParticleSystem>(explosionFX);
            vfx.transform.position = transform.position;
            TimerManager.CreateTimeOut(VFX_Time, () => GameObjectPoolManager.RecycleItem(explosionFX.name, vfx));
        }
    }

    public void GetTarget(Transform target)
    {
        this.target = target;
    }

    private void PlayExplodeSound()
    {
        audioSource.clip = explosionSounds[Random.Range(0, explosionSounds.Length)];
        audioSource.Play();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
#endif
}
