using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 实现榴弹子弹；可弹跳；范围伤害；
/// </summary>
public class ExplosionBulletSet : MonoBehaviour
{
    [Header("References目标绑定")]
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private AudioClip[] explosionSounds;
    [SerializeField] private LayerMask whatIsEnemies;
    private Transform tr;
    private Rigidbody rb;
    private AudioSource audioSource;
    private MeshRenderer grenadeMesh;

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
    [SerializeField] private bool isExplosible;       //是否可爆炸
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

    int collisions; //当前碰撞次数
    PhysicMaterial _physicsMaterial;

    private void Start()
    {
        Setup();
    }

    /// <summary>
    /// 初始化子弹物理材质与重力设置
    /// </summary>
    private void Setup()
    {
        //创建新物理材质，初始化材质设置
        _physicsMaterial = new PhysicMaterial();
        _physicsMaterial.bounciness = bounciness;
        _physicsMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        _physicsMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        //PhysicMaterial.frictionCombine确定摩擦力的组合方式
        //PhysicMaterial.bounceCombine表面的弹性有多大？值为0时不会反弹。值为1将反弹而不会损失任何能量

        //应用物理材质设置
        GetComponent<Collider>().material = _physicsMaterial;
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        grenadeMesh = GetComponent<MeshRenderer>();

        //设置重力使用
        rb.useGravity = useGravity;
    }

    private void OnEnable()
    {
        if (grenadeMesh != null && grenadeMesh.enabled == false)
        {
            grenadeMesh.enabled = true;
        }
        StartCoroutine(DelyToExplode());
    }

    private void Update()
    {
        //追踪逻辑
        if (isTraceable)
        {
            if (target != null)
            {
                // 获取目标方向
                Vector3 _direction = (target.position - tr.position).normalized;
                //当前方向与目标方向的角度差
                float _angle = Vector3.Angle(tr.forward, _direction);
                float _needTime = _angle / turnSpeed;
                if (_needTime < 0.02f)
                    tr.forward = _direction;
                else
                    tr.forward = Vector3.Slerp(tr.forward, _direction, Time.deltaTime / _needTime).normalized;
            }
        }

    }

    private void FixedUpdate()
    {
        if (isTraceable)
        {
            //向前移动
            Vector3 _targetPos = tr.position + moveSpeed * Time.deltaTime * tr.forward;
            rb.MovePosition(_targetPos);
        }

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
        if (collision.collider.CompareTag("Enemy") && explodeOnTuch) Explode();
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
        //关闭子弹渲染
        grenadeMesh.enabled = false;
        //实例化爆炸效果
        if (explosionFX != null)
        {
            GameObject _vfx = GameObjectPoolManager.SpawnObject(explosionFX, tr.position, Quaternion.Euler(-90f, 0f, 0f));
            GameObjectPoolManager.DelyReturnToPoolBySeconds(_vfx, VFX_Time);
        }
        //播放爆炸声音
        PlayExplodeSound();

        //检测爆炸范围内是否有敌人，实现伤害
        Collider[] enemies = Physics.OverlapSphere(tr.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            //获取敌人脚本并调用其中的受伤函数中受爆炸伤害部分
            //if (enemies[i].GetComponent<Health>())
            //    enemies[i].GetComponent<Health>().TakeExplosionDamage(explosionDamage);

            //实现爆炸推动敌人或物体效果（如果存在rigidbody）
            if (enableExplosionForce)
            {
                if (enemies[i].GetComponent<Rigidbody>())
                    enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, tr.position, explosionRange);
            }
        }
        GameObjectPoolManager.DelyReturnToPoolBySeconds(gameObject, VFX_Time);
    }

    /// <summary>
    /// 绘制子弹伤害范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
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
}
